// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge;
using Microsoft.Extensions.Logging;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowFeatureWebMessaging(
    IInfiniFrameWindow window,
    ILogger<InfiniFrameWindowFeatureWebMessaging> logger
) : IInfiniFrameWindowFeatureWebMessaging {
    /// <inheritdoc cref="IInfiniFrameWindowFeatureWebMessaging.SendWebMessage" />
    public void SendWebMessage(string message) {
        if (window.IsClosedOrClosing()) return;

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.SendWebMessage,
            message
        );
    }

    // ReSharper disable once ConvertIfStatementToReturnStatement
    /// <inheritdoc cref="IInfiniFrameWindowFeatureWebMessaging.SendWebMessageAsync" />
    public ValueTask SendWebMessageAsync(string message, CancellationToken ct = default) {
        if (ct.IsCancellationRequested) return ValueTask.FromCanceled(ct);
        if (window.IsClosedOrClosing()) return ValueTask.CompletedTask;

        // The native operation queues the message with the platform WebView; there is no
        // completion callback to await. Do not consume a worker thread just to make this look
        // asynchronous.
        SendWebMessage(message);
        return ValueTask.CompletedTask;
    }
}
