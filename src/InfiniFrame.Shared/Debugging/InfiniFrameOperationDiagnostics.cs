// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Debugging;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>Describes a managed/native operation associated with a window.</summary>
public sealed record InfiniFrameOperationDiagnostics {
    /// <summary>
    ///     Gets the name of the operation.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    ///     Gets the unique identifier for the operation.
    /// </summary>
    public required ulong Id { get; init; }

    /// <summary>
    ///     Gets the UTC timestamp when the operation started.
    /// </summary>
    public required DateTimeOffset StartedUtc { get; init; }

    /// <summary>
    ///     Gets the UTC timestamp when the operation completed, if it has finished.
    /// </summary>
    public DateTimeOffset? CompletedUtc { get; init; }

    /// <summary>
    ///     Gets the final state of the operation (e.g. "Completed", "Failed").
    /// </summary>
    public required string FinalState { get; init; }

    /// <summary>
    ///     Gets the native error code, if the operation failed with a native error.
    /// </summary>
    public int? NativeCode { get; init; }

    /// <summary>
    ///     Gets a human-readable description of the failure, if applicable.
    /// </summary>
    public string? FailureReason { get; init; }
}
