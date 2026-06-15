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
    /// <inheritdoc cref="IInfiniFrameWindowFeatureWebMessaging.SendWebMessage"/>
    public void SendWebMessage(string message) {
        if (window.IsClosedOrClosing()) return;

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.SendWebMessage,
            message
        );
    }

    // ReSharper disable once ConvertIfStatementToReturnStatement
    /// <inheritdoc cref="IInfiniFrameWindowFeatureWebMessaging.SendWebMessageAsync"/>
    public ValueTask SendWebMessageAsync(string message, CancellationToken ct = default) {
        if (window.IsClosedOrClosing()) return ValueTask.CompletedTask;
        if (ct.IsCancellationRequested) return ValueTask.FromCanceled(ct);

        return new ValueTask(Task.Run(action: () => SendWebMessage(message), ct));
    }
}
