// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IWebMessagingInfiniFrameWindowFeature {
    /// <summary>
    ///     Sends a message to the native window's browser control JavaScript context.
    /// </summary>
    /// <param name="message">The message to send as a string.</param>
    void SendWebMessage(string message);

    /// <summary>
    ///     Enqueues a message on the owning native UI loop. Completion means local browser submission only.
    /// </summary>
    /// <param name="message">The message to send as a string.</param>
    /// <param name="ct">A cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask SendWebMessageAsync(string message, CancellationToken ct = default);

    /// <summary>
    ///     Sends an InfiniFrame envelope and waits until the JavaScript message router acknowledges receipt.
    /// </summary>
    Task SendWebMessageWithAcknowledgementAsync(string message, CancellationToken ct = default);
}
