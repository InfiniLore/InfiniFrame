// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using InfiniFrame.NativeBridge;
using InfiniFrame.NativeBridge.Handles;
using InfiniFrame.Utilities;
using Microsoft.Extensions.Logging;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal sealed class InfiniNotificationOperation {
    private static long _nextId;
    private static readonly InfiniFrameNative.OperationCompletedCallback CompletionCallback = Complete;
    private readonly CancellationToken _cancellationToken;
    private readonly TaskCompletionSource<InfiniFrameNotificationActivation> _completion =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly string? _diagnosticKey;
    private readonly ILogger _logger;
    private readonly InfiniFrameNotificationOptions _options;

    private readonly IInfiniFrameWindow _window;
    private int _cancellationDispatchStarted;
    private CancellationTokenRegistration _cancellationRegistration;
    private int _cancellationRequested;
    private int _completed;
    private NativeHandleLease? _lease;
    private int _nativeStarted;
    private GCHandle _selfHandle;

    public InfiniNotificationOperation(
        IInfiniFrameWindow window,
        ILogger logger,
        InfiniFrameNotificationOptions options,
        CancellationToken cancellationToken
    ) {
        _window = window;
        _logger = logger;
        _options = options;
        _cancellationToken = cancellationToken;
        _diagnosticKey = (window as InfiniFrameWindow)?.BeginDiagnosticOperation("ShowNotification", Id);
    }

    public ulong Id { get; } = unchecked((ulong)Interlocked.Increment(ref _nextId));
    public Task<InfiniFrameNotificationActivation> Task => _completion.Task;

    public async Task StartAsync() {
        try {
            _cancellationRegistration = _cancellationToken.Register(
                callback: static state => ((InfiniNotificationOperation)state!).OnCancellationRequested(), this
            );
            await _window.WaitForReadyAsync(_cancellationToken).ConfigureAwait(false);
            _lease = _window.AcquireNativeHandle();
            _selfHandle = GCHandle.Alloc(this);
            IntPtr context = GCHandle.ToIntPtr(_selfHandle);
            InfiniFrameDispatchResult dispatch = await _window.DispatchAsync(() => {
                InfiniFrameNativeInteropStatus status = InfiniFrameNative.BeginShowNotification(
                    _lease.Handle, Id,
                    _options.Title, _options.Body,
                    _options.IconPath ?? string.Empty,
                    (int)_options.Urgency,
                    _options.Tag ?? string.Empty,
                    CompletionCallback, context
                );
                if (status != InfiniFrameNativeInteropStatus.Success)
                    throw new InfiniFrameNativeInteropException(
                        InfiniFrameNative.GetLastErrorMessage() ?? "Could not show native notification."
                    );
            }).ConfigureAwait(false);

            if (dispatch != InfiniFrameDispatchResult.Completed) {
                Finish(new InfiniFrameNotificationActivation(InfiniFrameNotificationResult.Dismissed));
                return;
            }

            Volatile.Write(ref _nativeStarted, 1);
            if (Volatile.Read(ref _cancellationRequested) != 0)
                StartCancellationDispatch();
        }
        catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) {
            _logger.LogError(exception, "Asynchronous notification {OperationId} failed.", Id);
            Finish(new InfiniFrameNotificationActivation(InfiniFrameNotificationResult.Failed));
        }
    }

    private void OnCancellationRequested() {
        Volatile.Write(ref _cancellationRequested, 1);
        if (Volatile.Read(ref _nativeStarted) != 0)
            StartCancellationDispatch();
    }

    private void StartCancellationDispatch() {
        if (Volatile.Read(ref _completed) != 0) return;

        if (Interlocked.Exchange(ref _cancellationDispatchStarted, 1) == 0)
            _ = RequestCancellationAsync();
    }

    private async Task RequestCancellationAsync() {
        try {
            await _window.DispatchAsync(() => {
                if (_lease is null) return;

                InfiniFrameNativeInteropStatus status = InfiniFrameNative.CancelNotification(_lease.Handle, Id, out _);
                if (status != InfiniFrameNativeInteropStatus.Success)
                    throw new InfiniFrameNativeInteropException(
                        InfiniFrameNative.GetLastErrorMessage() ?? "Could not cancel native notification."
                    );
            }).ConfigureAwait(false);
        }
        catch (ObjectDisposedException) {
            // Window teardown requests cancellation for every registered notification.
        }
        catch (InfiniFrameNativeInteropException exception) {
            _logger.LogWarning(exception, "Native notification cancellation for {OperationId} failed.", Id);
        }
    }

    private static void Complete(
        IntPtr context,
        ulong operationId,
        int result,
        int nativeCode,
        IntPtr failureUtf8
    ) {
        if (!TryGet(context, out InfiniNotificationOperation? operation) || operation.Id != operationId)
            return;

        InfiniFrameNotificationResult notificationResult = result == 0
            ? (InfiniFrameNotificationResult)nativeCode
            : InfiniFrameNotificationResult.Failed;

        operation.Finish(new InfiniFrameNotificationActivation(notificationResult));
    }

    private void Finish(InfiniFrameNotificationActivation activation) {
        if (Interlocked.Exchange(ref _completed, 1) != 0) return;

        (_window as InfiniFrameWindow)?.CompleteDiagnosticOperation(
            _diagnosticKey, _cancellationToken.IsCancellationRequested ? "Cancelled" : activation.Result.ToString()
        );
        _completion.TrySetResult(activation);
        ThreadPool.QueueUserWorkItem(callBack: static state => state.Cleanup(), this, false);
    }

    private void Cleanup() {
        _cancellationRegistration.Dispose();
        if (_selfHandle.IsAllocated) _selfHandle.Free();
        Interlocked.Exchange(ref _lease, null)?.Dispose();
    }

    private static bool TryGet(IntPtr context, [NotNullWhen(true)] out InfiniNotificationOperation? operation) {
        operation = null;
        if (context == IntPtr.Zero) return false;

        try {
            operation = GCHandle.FromIntPtr(context).Target as InfiniNotificationOperation;
            return operation is not null;
        }
        catch (InvalidOperationException) {
            return false;
        }
    }
}
