// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Represents the result of a page navigation operation.
/// </summary>
/// <param name="OperationId">The unique identifier for the navigation operation.</param>
/// <param name="Status">The outcome of the navigation.</param>
/// <param name="Uri">The URI that was navigated to, if available.</param>
/// <param name="NativeErrorCode">The native error code, if the navigation failed.</param>
/// <param name="FailureReason">A human-readable description of the failure, if applicable.</param>
public sealed record NavigationResult(
    ulong OperationId,
    NavigationStatus Status,
    Uri? Uri = null,
    int NativeErrorCode = 0,
    string? FailureReason = null
);
