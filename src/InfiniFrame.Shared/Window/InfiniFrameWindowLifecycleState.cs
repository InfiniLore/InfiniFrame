// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>Represents the deterministic managed/native lifetime of a window.</summary>
public enum InfiniFrameWindowLifecycleState {
    /// <summary>
    ///     The window has been created in managed code but native initialization has not started.
    /// </summary>
    Created = 0,

    /// <summary>
    ///     The native window is being created and initialized.
    /// </summary>
    Creating = 1,

    /// <summary>
    ///     Alias for <see cref="Creating" />.
    /// </summary>
    Initializing = Creating,

    /// <summary>
    ///     The window is ready for interaction.
    /// </summary>
    Ready = 2,

    /// <summary>
    ///     Alias for <see cref="Ready" />.
    /// </summary>
    Running = Ready,

    /// <summary>
    ///     A close has been requested but not yet processed.
    /// </summary>
    CloseRequested = 3,

    /// <summary>
    ///     Alias for <see cref="CloseRequested" />.
    /// </summary>
    ClosingRequested = CloseRequested,

    /// <summary>
    ///     The native window has been closed.
    /// </summary>
    NativeClosed = 4,

    /// <summary>
    ///     Teardown is pending after the native window closed.
    /// </summary>
    TeardownPending = 5,

    /// <summary>
    ///     Teardown has completed.
    /// </summary>
    TeardownComplete = 6,

    /// <summary>
    ///     The native window handle has been released.
    /// </summary>
    NativeHandleReleased = 7,

    /// <summary>
    ///     The window has been fully disposed.
    /// </summary>
    Disposed = 8
}
