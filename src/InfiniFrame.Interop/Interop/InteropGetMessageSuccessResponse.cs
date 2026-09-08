// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Interop;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Represents a successful response for a get message interop call.
/// </summary>
internal sealed class InteropGetMessageSuccessResponse {
    /// <summary>
    ///     Gets the request identifier associated with the response.
    /// </summary>
    public string? RequestId { get; init; }
    /// <summary>
    ///     Gets whether the operation was successful. Always <c>true</c> for success responses.
    /// </summary>
    public bool Success { get; init; }
    /// <summary>
    ///     Gets the data payload returned by the response.
    /// </summary>
    public string? Data { get; init; }
}
