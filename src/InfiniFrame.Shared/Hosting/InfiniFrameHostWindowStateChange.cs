// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Hosting;

/// <summary>A requested native state change for <see cref="IInfiniFrameWindowHostState.SetStateAsync" />.</summary>
public enum InfiniFrameHostWindowStateChange {
    /// <summary>Leave minimized or maximized state.</summary>
    Restore,

    /// <summary>Minimize.</summary>
    Minimize,

    /// <summary>Maximize.</summary>
    Maximize,

    /// <summary>Enter full screen.</summary>
    EnterFullScreen,

    /// <summary>Leave full screen.</summary>
    ExitFullScreen,

    /// <summary>Request keyboard focus (<see cref="InfiniFrameHostCapabilities.Focus" />).</summary>
    Focus
}
