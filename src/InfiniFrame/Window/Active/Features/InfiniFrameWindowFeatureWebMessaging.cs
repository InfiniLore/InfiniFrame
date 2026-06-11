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
    /// <summary>
    /// Sends a message to the native window's native browser control's JavaScript context.
    /// </summary>
    /// <remarks>
    /// In JavaScript, messages can be received via <code>window.infiniframe.host.receiveCallback(callback)</code>.
    /// </remarks>
    /// <exception cref="ApplicationException">
    /// Thrown when the window is not initialized.
    /// </exception>
    /// <param name="message">The message to be sent as a string.</param>
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

    /// <summary>
    /// Sends a message asynchronously to the native window's native browser control's JavaScript context.
    /// </summary>
    /// <remarks>
    /// This method allows interaction with JavaScript in the context of the associated browser control.
    /// </remarks>
    /// <exception cref="OperationCanceledException">
    /// Thrown if the operation is canceled through the provided <paramref name="ct"/>.
    /// </exception>
    /// <param name="message">The message to send as a string.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>
    /// A <see cref="ValueTask"/> representing the asynchronous operation.
    /// </returns>
    public ValueTask SendWebMessageAsync(string message, CancellationToken ct = default) {
        if (window.IsClosedOrClosing()) return ValueTask.CompletedTask;
        if (ct.IsCancellationRequested) return ValueTask.FromCanceled(ct);
        
        Task.Run(action: () => SendWebMessage(message), ct);
        return ValueTask.CompletedTask; 
    }
}