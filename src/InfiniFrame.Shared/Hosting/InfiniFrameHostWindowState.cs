// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Hosting;

/// <summary>Observable native window state.</summary>
[Flags]
public enum InfiniFrameHostWindowState {
    /// <summary>Restored, not focused.</summary>
    Normal = 0,

    /// <summary>Minimized.</summary>
    Minimized = 1,

    /// <summary>Maximized.</summary>
    Maximized = 2,

    /// <summary>Full screen.</summary>
    FullScreen = 4,

    /// <summary>Has keyboard focus.</summary>
    Focused = 8
}
