// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class IWebMessagingInfiniFrameWindowFeatureExtensions {
    /// <summary>
    ///     Sends a message to the native window's browser control JavaScript context.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="message">The message to send as a string.</param>
    public static void SendWebMessage(this IInfiniFrameWindow window, string message) {
        window.Features.WebMessaging.SendWebMessage(message);
    }
    /// <summary>
    ///     Sends a message asynchronously to the native window's browser control JavaScript context.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="message">The message to send as a string.</param>
    /// <param name="ct">A cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    public static ValueTask SendWebMessageAsync(this IInfiniFrameWindow window, string message, CancellationToken ct = default) {
        return window.Features.WebMessaging.SendWebMessageAsync(message, ct);
    }

    public static Task SendWebMessageWithAcknowledgementAsync(
        this IInfiniFrameWindow window, string message, CancellationToken ct = default
    ) => window.Features.WebMessaging.SendWebMessageWithAcknowledgementAsync(message, ct);
}
