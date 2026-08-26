// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Provides taskbar progress and flash functionality for the window.
///     On Windows, uses ITaskbarList3 and FlashWindowEx.
///     On macOS, uses NSDockTile badge and NSRequestUserAttention.
///     On Linux, uses D-Bus StatusNotifierItem or Unity LauncherEntry where available.
/// </summary>
public interface ITaskbarInfiniFrameWindowFeature {
    /// <summary>
    ///     Gets whether the taskbar progress and flash APIs are supported on the current platform.
    ///     Windows: always true. macOS: progress via dock badge, flash via requestUserAttention.
    ///     Linux: depends on desktop environment D-Bus support.
    /// </summary>
    bool IsSupported { get; }

    /// <summary>
    ///     Gets the capabilities of the taskbar feature on the current platform.
    /// </summary>
    InfiniFrameTaskbarCapabilities Capabilities { get; }

    /// <summary>
    ///     Gets the current taskbar progress state.
    /// </summary>
    TaskbarProgressState CurrentProgressState { get; }

    /// <summary>
    ///     Sets the taskbar progress indicator with the specified state, current value, and total value.
    /// </summary>
    /// <param name="state">The visual state of the progress indicator.</param>
    /// <param name="current">The current progress value.</param>
    /// <param name="total">The total progress value.</param>
    void SetProgress(TaskbarProgressState state, ulong current, ulong total);

    /// <summary>
    ///     Clears the taskbar progress indicator, removing it from the taskbar button.
    /// </summary>
    void ClearProgress();

    /// <summary>
    ///     Flashes the taskbar icon using the specified mode and count.
    /// </summary>
    /// <param name="mode">The flash mode to use.</param>
    /// <param name="count">
    ///     The number of times to flash (ignored for <see cref="TaskbarFlashMode.All" /> and
    ///     <see cref="TaskbarFlashMode.Stop" />).
    /// </param>
    void SetFlash(TaskbarFlashMode mode, uint count);

    /// <summary>
    ///     Stops the taskbar icon from flashing.
    /// </summary>
    void StopFlash();
}
