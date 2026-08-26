// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge;
using InfiniFrame.Utilities;
using Microsoft.Extensions.Logging;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InvokeInfiniFrameWindowFeature(
    IInfiniFrameWindow window,
    ILogger<InvokeInfiniFrameWindowFeature> logger
) : IInvokeInfiniFrameWindowFeature {
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(15);

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public InfiniFrameDispatchResult Invoke(Action callback) {
        ArgumentNullException.ThrowIfNull(callback);
        if (window.Features.Lifecycle.IsClosedOrClosing()) return InfiniFrameDispatchResult.WindowClosed;

        try {
            // This is deliberately the only blocking API. Most callers should use DispatchAsync instead.
            bool callbackStarted = false;
            NativeInvoke.InvokeSyncWithValidation(logger, window, window.ManagedThreadId, callback: () => {
                callbackStarted = true;
                callback();
            });
            if (callbackStarted) return InfiniFrameDispatchResult.Completed;

            logger.LogWarning("Synchronous native-window dispatch returned without invoking its callback. WindowId={WindowId}", window.Id);
            return window.Features.Lifecycle.IsClosedOrClosing()
                ? InfiniFrameDispatchResult.WindowClosed
                : InfiniFrameDispatchResult.TimedOut;
        }
        catch (ObjectDisposedException) when (window.Features.Lifecycle.IsClosedOrClosing()) {
            return InfiniFrameDispatchResult.WindowClosed;
        }
        catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) {
            logger.LogError(exception, "Synchronous native-window dispatch failed. WindowId={WindowId}", window.Id);
            return window.Features.Lifecycle.IsClosedOrClosing()
                ? InfiniFrameDispatchResult.WindowClosed
                : InfiniFrameDispatchResult.Failed;
        }
    }

    public ValueTask<InfiniFrameDispatchResult> DispatchAsync(
        Action callback,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default
    ) {
        ArgumentNullException.ThrowIfNull(callback);
        if (timeout is {} value && value <= TimeSpan.Zero)
            return new ValueTask<InfiniFrameDispatchResult>(InfiniFrameDispatchResult.TimedOut);
        if (cancellationToken.IsCancellationRequested)
            return new ValueTask<InfiniFrameDispatchResult>(InfiniFrameDispatchResult.Cancelled);
        if (window.Features.Lifecycle.IsClosedOrClosing())
            return new ValueTask<InfiniFrameDispatchResult>(InfiniFrameDispatchResult.WindowClosed);

        var operation = new InfiniDispatchOperation(window, logger, callback, timeout ?? DefaultTimeout, cancellationToken);
        operation.Start();
        return new ValueTask<InfiniFrameDispatchResult>(operation.Task);
    }
}
