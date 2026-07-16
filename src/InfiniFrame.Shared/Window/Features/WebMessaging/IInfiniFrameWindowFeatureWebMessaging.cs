// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowFeatureWebMessaging {
    /// <summary>
    ///     Sends a message to the native window's browser control JavaScript context.
    /// </summary>
    /// <param name="message">The message to send as a string.</param>
    void SendWebMessage(string message);

    /// <summary>
    ///     Sends a message asynchronously to the native window's browser control JavaScript context.
    /// </summary>
    /// <param name="message">The message to send as a string.</param>
    /// <param name="ct">A cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask SendWebMessageAsync(string message, CancellationToken ct = default);
}
