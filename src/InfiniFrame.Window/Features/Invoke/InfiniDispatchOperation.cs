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
internal sealed class InfiniDispatchOperation {
    private static long _nextOperationId;
    private static readonly InfiniFrameNative.ContextAction InvokeCallback = Invoke;
    private static readonly InfiniFrameNative.OperationCompletedCallback CompletionCallback = Complete;
    private readonly Action _callback;
    private readonly CancellationToken _cancellationToken;
    private readonly TaskCompletionSource<InfiniFrameDispatchResult> _completion =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly string? _diagnosticKey;
    private readonly ILogger _logger;
    private readonly TimeSpan _timeout;

    private readonly IInfiniFrameWindow _window;
    private Exception? _callbackException;
    private CancellationTokenRegistration _cancellationRegistration;
    private int _cleanupQueued;
    private int _completed;
    private NativeHandleLease? _lease;
    private int _pendingCancellation = -1;
    private GCHandle _selfHandle;
    private Timer? _timeoutTimer;

    public InfiniDispatchOperation(
        IInfiniFrameWindow window,
        ILogger logger,
        Action callback,
        TimeSpan timeout,
        CancellationToken cancellationToken
    ) {
        _window = window;
        _logger = logger;
        _callback = callback;
        _timeout = timeout;
        _cancellationToken = cancellationToken;
        _diagnosticKey = (window as InfiniFrameWindow)?.BeginDiagnosticOperation("Dispatch", Id);
    }

    public ulong Id { get; } = unchecked((ulong)Interlocked.Increment(ref _nextOperationId));
    public Task<InfiniFrameDispatchResult> Task => _completion.Task;

    public void Start() {
        try {
            _lease = _window.AcquireNativeHandle();
            _selfHandle = GCHandle.Alloc(this);
            IntPtr context = GCHandle.ToIntPtr(_selfHandle);

            _timeoutTimer = new Timer(
                callback: static state => ((InfiniDispatchOperation)state!).Cancel(InfiniFrameDispatchResult.TimedOut),
                this,
                _timeout,
                Timeout.InfiniteTimeSpan
            );
            _cancellationRegistration = _cancellationToken.Register(
                callback: static state => ((InfiniDispatchOperation)state!).Cancel(InfiniFrameDispatchResult.Cancelled), this
            );

            InfiniFrameNativeInteropStatus status = InfiniFrameNative.BeginInvoke(
                _lease.Handle, Id, InvokeCallback, context, CompletionCallback, context
            );
            if (status != InfiniFrameNativeInteropStatus.Success)
                Finish(InfiniFrameDispatchResult.Failed, GetNativeFailure("queue asynchronous dispatch"));
            else {
                int pendingCancellation = Volatile.Read(ref _pendingCancellation);
                if (pendingCancellation >= 0)
                    Cancel((InfiniFrameDispatchResult)pendingCancellation);
            }
        }
        catch (ObjectDisposedException) when (_window.Features.Lifecycle.IsClosedOrClosing()) {
            Finish(InfiniFrameDispatchResult.WindowClosed);
        }
        catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) {
            Finish(InfiniFrameDispatchResult.Failed, exception);
        }
    }

    private void Cancel(InfiniFrameDispatchResult result) {
        Volatile.Write(ref _pendingCancellation, (int)result);
        NativeHandleLease? lease = Volatile.Read(ref _lease);
        if (lease is null || Volatile.Read(ref _completed) != 0)
            return;

        try {
            InfiniFrameNative.CancelOperation(lease.Handle, Id, (int)result);
        }
        catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) {
            _logger.LogDebug(exception, "Could not cancel native dispatch {OperationId}.", Id);
        }
    }

    private static void Invoke(IntPtr context) {
        if (!TryGet(context, out InfiniDispatchOperation? operation))
            return;

        try {
            operation._callback();
        }
        catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) {
            operation._callbackException = exception;
        }
    }

    private static void Complete(IntPtr context, ulong operationId, int result, int nativeCode, IntPtr failureUtf8) {
        if (!TryGet(context, out InfiniDispatchOperation? operation) || operation.Id != operationId)
            return;

        InfiniFrameDispatchResult dispatchResult = Enum.IsDefined(typeof(InfiniFrameDispatchResult), result)
            ? (InfiniFrameDispatchResult)result
            : InfiniFrameDispatchResult.Failed;
        Exception? exception = operation._callbackException;
        if (exception is not null)
            dispatchResult = InfiniFrameDispatchResult.Failed;
        else if (dispatchResult == InfiniFrameDispatchResult.Failed) {
            string failure = Marshal.PtrToStringUTF8(failureUtf8) ?? "Native dispatch failed.";
            exception = new ApplicationException($"{failure} Native code: {nativeCode}.");
        }

        operation.Finish(dispatchResult, exception);
    }

    private void Finish(InfiniFrameDispatchResult result, Exception? exception = null) {
        if (Interlocked.Exchange(ref _completed, 1) != 0)
            return;

        if (exception is not null)
            _logger.LogError(exception, "Native-window dispatch {OperationId} failed. WindowId={WindowId}", Id, _window.Id);
        else if (result is not InfiniFrameDispatchResult.Completed)
            _logger.LogDebug("Native-window dispatch {OperationId} ended with {DispatchResult}. WindowId={WindowId}", Id, result, _window.Id);

        (_window as InfiniFrameWindow)?.CompleteDiagnosticOperation(
            _diagnosticKey, result.ToString(), failureReason: exception?.Message
        );

        _completion.TrySetResult(result);
        QueueCleanupAfterReverseCallback();
    }

    private void QueueCleanupAfterReverseCallback() {
        if (Interlocked.Exchange(ref _cleanupQueued, 1) != 0)
            return;

        ThreadPool.QueueUserWorkItem(callBack: static state => state.Cleanup(), this, false);
    }

    private void Cleanup() {
        _cancellationRegistration.Dispose();
        _timeoutTimer?.Dispose();
        _timeoutTimer = null;
        if (_selfHandle.IsAllocated)
            _selfHandle.Free();
        Interlocked.Exchange(ref _lease, null)?.Dispose();
    }

    private Exception GetNativeFailure(string operation) {
        int error = Marshal.GetLastPInvokeError();
        string message = InfiniFrameNative.GetLastErrorMessage() ?? "No native error message provided.";
        return new ApplicationException($"Could not {operation}. Error #{error}. {message}");
    }

    private static bool TryGet(IntPtr context, [NotNullWhen(true)] out InfiniDispatchOperation? operation) {
        operation = null;
        if (context == IntPtr.Zero)
            return false;

        try {
            operation = GCHandle.FromIntPtr(context).Target as InfiniDispatchOperation;
            return operation is not null;
        }
        catch (InvalidOperationException) {
            return false;
        }
    }
}
