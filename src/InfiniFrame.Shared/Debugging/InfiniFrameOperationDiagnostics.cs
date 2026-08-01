// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Debugging;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>Describes a managed/native operation associated with a window.</summary>
public sealed record InfiniFrameOperationDiagnostics {
    public required string Name { get; init; }
    public required ulong Id { get; init; }
    public required DateTimeOffset StartedUtc { get; init; }
    public DateTimeOffset? CompletedUtc { get; init; }
    public required string FinalState { get; init; }
    public int? NativeCode { get; init; }
    public string? FailureReason { get; init; }
}
