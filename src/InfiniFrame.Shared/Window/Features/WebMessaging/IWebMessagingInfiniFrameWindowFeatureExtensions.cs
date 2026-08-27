// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Fluent extension methods for <see cref="IWebMessagingInfiniFrameWindowFeature"/> on <see cref="IInfiniFrameWindow"/>.
/// </summary>
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
    /// <returns>A <see cref="ValueTask" /> representing the asynchronous operation.</returns>
    public static ValueTask SendWebMessageAsync(this IInfiniFrameWindow window, string message, CancellationToken ct = default) => window.Features.WebMessaging.SendWebMessageAsync(message, ct);

    /// <summary>
    ///     Sends a web message and waits for the JavaScript router to acknowledge receipt.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="message">The message to send as a string.</param>
    /// <param name="ct">A cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> that completes when the message has been acknowledged.</returns>
    public static Task SendWebMessageWithAcknowledgementAsync(
        this IInfiniFrameWindow window,
        string message,
        CancellationToken ct = default
    ) => window.Features.WebMessaging.SendWebMessageWithAcknowledgementAsync(message, ct);
}
