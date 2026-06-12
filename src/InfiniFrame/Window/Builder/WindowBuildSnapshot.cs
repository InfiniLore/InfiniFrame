namespace InfiniFrame;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal sealed record WindowBuildSnapshot {
    public InfiniFrameEventsStore EventsStore { get; init; } = new();
}
