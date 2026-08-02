// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>Represents the deterministic managed/native lifetime of a window.</summary>
public enum InfiniFrameWindowLifecycleState {
    Created = 0,
    Creating = 1,
    Initializing = Creating,
    Ready = 2,
    Running = Ready,
    CloseRequested = 3,
    ClosingRequested = CloseRequested,
    NativeClosed = 4,
    TeardownPending = 5,
    TeardownComplete = 6,
    NativeHandleReleased = 7,
    Disposed = 8
}