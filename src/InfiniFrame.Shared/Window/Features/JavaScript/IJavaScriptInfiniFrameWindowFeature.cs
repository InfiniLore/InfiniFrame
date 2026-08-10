// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Provides the ability to execute arbitrary JavaScript in the window's browser control from the C# side.
/// </summary>
public interface IJavaScriptInfiniFrameWindowFeature {
    /// <summary>
    ///     Executes a JavaScript expression in the browser control and returns the result as a JSON string.
    /// </summary>
    /// <param name="script">The JavaScript expression to evaluate.</param>
    /// <param name="ct">A cancellation token to cancel the operation.</param>
    /// <returns>The JSON-encoded result of the evaluation, or <c>null</c> if the script returns <c>undefined</c>.</returns>
    ValueTask<string?> ExecuteJavaScriptAsync(string script, CancellationToken ct = default);

    /// <summary>
    ///     Executes a JavaScript expression in the browser control and deserializes the result to the specified type.
    /// </summary>
    /// <typeparam name="T">The expected return type.</typeparam>
    /// <param name="script">The JavaScript expression to evaluate.</param>
    /// <param name="ct">A cancellation token to cancel the operation.</param>
    /// <returns>The deserialized result, or <c>default</c> if the script returns <c>null</c>.</returns>
    ValueTask<T?> ExecuteJavaScriptAsync<T>(string script, CancellationToken ct = default);

    /// <summary>
    ///     Sends a JavaScript evaluation request to the browser without waiting for the result.
    ///     The result will be sent back via a separate message.
    /// </summary>
    /// <param name="script">The JavaScript expression to evaluate.</param>
    /// <param name="requestId">An optional request identifier for correlating the result.</param>
    void SendEvalToBrowser(string script, string? requestId = null);
}
