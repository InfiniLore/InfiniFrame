// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Fluent extension methods for the taskbar feature on <see cref="IInfiniFrameWindow"/>.
/// </summary>
public static class ITaskbarInfiniFrameWindowFeatureExtensions {
    /// <summary>
    ///     Sets the taskbar progress indicator and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="state">The visual state of the progress indicator.</param>
    /// <param name="current">The current progress value.</param>
    /// <param name="total">The total progress value.</param>
    /// <returns>The <see cref="IInfiniFrameWindow"/> for method chaining.</returns>
    public static IInfiniFrameWindow SetTaskbarProgress(this IInfiniFrameWindow window, TaskbarProgressState state, ulong current, ulong total) {
        window.Features.Taskbar.SetProgress(state, current, total);
        return window;
    }

    /// <summary>
    ///     Clears the taskbar progress indicator and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <returns>The <see cref="IInfiniFrameWindow"/> for method chaining.</returns>
    public static IInfiniFrameWindow ClearTaskbarProgress(this IInfiniFrameWindow window) {
        window.Features.Taskbar.ClearProgress();
        return window;
    }

    /// <summary>
    ///     Flashes the taskbar icon and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="mode">The flash mode to use.</param>
    /// <param name="count">The number of times to flash.</param>
    /// <returns>The <see cref="IInfiniFrameWindow"/> for method chaining.</returns>
    public static IInfiniFrameWindow FlashTaskbar(this IInfiniFrameWindow window, TaskbarFlashMode mode, uint count = 0) {
        window.Features.Taskbar.SetFlash(mode, count);
        return window;
    }

    /// <summary>
    ///     Stops the taskbar icon from flashing and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <returns>The <see cref="IInfiniFrameWindow"/> for method chaining.</returns>
    public static IInfiniFrameWindow StopTaskbarFlash(this IInfiniFrameWindow window) {
        window.Features.Taskbar.StopFlash();
        return window;
    }
}
