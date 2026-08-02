// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge;
using InfiniFrame.NativeBridge.Handles;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal sealed class InfiniNavigationOperation {
    private const int NativeOperationResultSuperseded = 5;
    private static long _nextId;
    private static readonly InfiniFrameNative.OperationCompletedCallback CompletionCallback = Complete;

    private readonly IInfiniFrameWindow _window;
    private readonly ILogger _logger;
    private readonly string _value;
    private readonly Uri? _uri;
    private readonly bool _rawString;
    private readonly CancellationToken _cancellationToken;
    private readonly string? _diagnosticKey;
    private readonly TaskCompletionSource<NavigationResult> _completion =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    private NativeHandleLease? _lease;
    private GCHandle _selfHandle;
    private CancellationTokenRegistration _cancellationRegistration;
    private int _completed;
    private int _cleanupQueued;

    public ulong Id { get; } = unchecked((ulong)Interlocked.Increment(ref _nextId));
    public Task<NavigationResult> Task => _completion.Task;

    public InfiniNavigationOperation(
        IInfiniFrameWindow window,
        ILogger logger,
        string value,
        Uri? uri,
        bool rawString,
        CancellationToken cancellationToken
    ) {
        _window = window;
        _logger = logger;
        _value = value;
        _uri = uri;
        _rawString = rawString;
        _cancellationToken = cancellationToken;
        _diagnosticKey = (window as InfiniFrameWindow)?.BeginDiagnosticOperation(
            rawString ? "LoadRawString" : "Load", Id
        );
    }

    public async Task StartAsync() {
        try {
            await _window.WaitForReadyAsync(_cancellationToken).ConfigureAwait(false);
            _lease = _window.AcquireNativeHandle();
            _selfHandle = GCHandle.Alloc(this);
            IntPtr context = GCHandle.ToIntPtr(_selfHandle);

            InfiniFrameDispatchResult dispatch = await _window.DispatchAsync(() => {
                InfiniFrameNativeInteropStatus status = _rawString
                    ? InfiniFrameNative.BeginNavigateToString(_lease.Handle, Id, _value, CompletionCallback, context)
                    : InfiniFrameNative.BeginNavigateToUrl(_lease.Handle, Id, _value, CompletionCallback, context);
                if (status != InfiniFrameNativeInteropStatus.Success)
                    throw new ApplicationException(InfiniFrameNative.GetLastErrorMessage() ?? "Native navigation registration failed.");
            }, cancellationToken: _cancellationToken).ConfigureAwait(false);

            if (dispatch == InfiniFrameDispatchResult.Cancelled) {
                FinishCancelled();
                return;
            }
            if (dispatch != InfiniFrameDispatchResult.Completed) {
                Finish(new NavigationResult(
                    Id,
                    dispatch == InfiniFrameDispatchResult.WindowClosed ? NavigationStatus.WindowClosed : NavigationStatus.Failed,
                    _uri,
                    FailureReason: $"Navigation dispatch ended with {dispatch}."
                ));
                return;
            }

            CancellationTokenRegistration registration = _cancellationToken.Register(
                static state => ((InfiniNavigationOperation)state!).RequestCancellation(), this
            );
            _cancellationRegistration = registration;
            // A backend is allowed to complete synchronously while BeginNavigate is returning.
            // In that race cleanup may have run before this registration was assigned.
            if (Volatile.Read(ref _completed) != 0) {
                registration.Dispose();
                return;
            }
            if (_cancellationToken.IsCancellationRequested)
                RequestCancellation();
        }
        catch (OperationCanceledException) when (_cancellationToken.IsCancellationRequested) {
            FinishCancelled();
        }
        catch (ObjectDisposedException) {
            Finish(new NavigationResult(Id, NavigationStatus.WindowClosed, _uri));
        }
        catch (Exception exception) {
            _logger.LogError(exception, "Navigation {OperationId} failed to start.", Id);
            Finish(new NavigationResult(Id, NavigationStatus.Failed, _uri, FailureReason: exception.Message));
        }
    }

    private void RequestCancellation() {
        NativeHandleLease? lease = Volatile.Read(ref _lease);
        if (lease is null || Volatile.Read(ref _completed) != 0) {
            FinishCancelled();
            return;
        }

        // Best-effort cancellation dispatch: faults are observed via ContinueWith to avoid
        // dropping exceptions on a background thread.
        _ = _window.DispatchAsync(() => InfiniFrameNative.CancelNavigation(lease.Handle, Id))
            .AsTask()
            .ContinueWith(
                t => _logger.LogWarning(t.Exception, "Unhandled error while cancelling navigation {OperationId}.", Id),
                CancellationToken.None,
                TaskContinuationOptions.OnlyOnFaulted,
                TaskScheduler.Default
            );
    }

    private static void Complete(IntPtr context, ulong operationId, int result, int nativeCode, IntPtr failureUtf8) {
        if (!TryGet(context, out InfiniNavigationOperation? operation) || operation.Id != operationId)
            return;

        switch (result) {
            case (int)InfiniFrameDispatchResult.Completed:
                operation.Finish(new NavigationResult(operation.Id, NavigationStatus.Succeeded, operation._uri));
                break;
            case (int)InfiniFrameDispatchResult.Cancelled:
                operation.FinishCancelled();
                break;
            case (int)InfiniFrameDispatchResult.WindowClosed:
                operation.Finish(new NavigationResult(operation.Id, NavigationStatus.WindowClosed, operation._uri));
                break;
            case NativeOperationResultSuperseded:
                operation.Finish(new NavigationResult(operation.Id, NavigationStatus.Superseded, operation._uri));
                break;
            default:
                operation.Finish(new NavigationResult(
                    operation.Id,
                    NavigationStatus.Failed,
                    operation._uri,
                    nativeCode,
                    Marshal.PtrToStringUTF8(failureUtf8) ?? "Navigation failed."
                ));
                break;
        }
    }

    private void FinishCancelled() {
        if (Interlocked.Exchange(ref _completed, 1) != 0) return;
        (_window as InfiniFrameWindow)?.CompleteDiagnosticOperation(_diagnosticKey, "Cancelled");
        _completion.TrySetCanceled(_cancellationToken.IsCancellationRequested
            ? _cancellationToken
            : new CancellationToken(true));
        QueueCleanup();
    }

    private void Finish(NavigationResult result) {
        if (Interlocked.Exchange(ref _completed, 1) != 0) return;
        (_window as InfiniFrameWindow)?.CompleteDiagnosticOperation(
            _diagnosticKey, result.Status.ToString(), result.NativeErrorCode, result.FailureReason
        );
        _completion.TrySetResult(result);
        QueueCleanup();
    }

    private void QueueCleanup() {
        if (Interlocked.Exchange(ref _cleanupQueued, 1) != 0) return;
        ThreadPool.QueueUserWorkItem(static state => state.Cleanup(), this, false);
    }

    private void Cleanup() {
        _cancellationRegistration.Dispose();
        if (_selfHandle.IsAllocated) _selfHandle.Free();
        Interlocked.Exchange(ref _lease, null)?.Dispose();
    }

    private static bool TryGet(
        IntPtr context,
        [NotNullWhen(true)] out InfiniNavigationOperation? operation
    ) {
        operation = null;
        if (context == IntPtr.Zero) return false;
        try {
            operation = GCHandle.FromIntPtr(context).Target as InfiniNavigationOperation;
            return operation is not null;
        }
        catch (InvalidOperationException) {
            return false;
        }
    }
}