// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Interop;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Represents an error response for a get message interop call.
/// </summary>
internal sealed class InteropGetMessageErrorResponse {
    /// <summary>
    ///     Gets the request identifier associated with the error.
    /// </summary>
    public string? RequestId { get; init; }
    /// <summary>
    ///     Gets whether the operation was successful. Always <c>false</c> for error responses.
    /// </summary>
    public bool Success { get; init; }
    /// <summary>
    ///     Gets the error message describing the failure.
    /// </summary>
    public string? Error { get; init; }
}