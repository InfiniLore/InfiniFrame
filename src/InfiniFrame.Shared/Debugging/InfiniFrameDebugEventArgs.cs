// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Debugging;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class InfiniFrameDebugEventArgs : EventArgs {
    public required InfiniFrameDebugEventKind Kind { get; init; }
    public string? Message { get; init; }
    public string? Level { get; init; }
    public string? Uri { get; init; }
    public int? StatusCode { get; init; }
    public required DateTime TimestampUtc { get; init; }
    public string? PlatformPayload { get; init; }
}
