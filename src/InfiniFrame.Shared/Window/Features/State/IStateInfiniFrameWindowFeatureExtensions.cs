// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class IStateInfiniFrameWindowFeatureExtension {
    /// <summary>
    ///     Sets whether the window is maximized and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="maximized">Whether to maximize the window.</param>
    /// <returns>The <see cref="IInfiniFrameWindow"/> for method chaining.</returns>
    public static IInfiniFrameWindow SetMaximized(this IInfiniFrameWindow window, bool maximized = true) {
        window.Features.State.SetMaximized(maximized);
        return window;
    }

    /// <summary>
    ///     Toggles the maximized state of the window and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <returns>The <see cref="IInfiniFrameWindow"/> for method chaining.</returns>
    public static IInfiniFrameWindow ToggleMaximized(this IInfiniFrameWindow window) {
        window.Features.State.ToggleMaximized();
        return window;
    }

    /// <summary>
    ///     Sets whether the window is minimized and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="minimized">Whether to minimize the window.</param>
    /// <returns>The <see cref="IInfiniFrameWindow"/> for method chaining.</returns>
    public static IInfiniFrameWindow SetMinimized(this IInfiniFrameWindow window, bool minimized = true) {
        window.Features.State.SetMinimized(minimized);
        return window;
    }

    /// <summary>
    ///     Sets whether the window is in full-screen mode and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="fullScreen">Whether to enter full-screen mode.</param>
    /// <returns>The <see cref="IInfiniFrameWindow"/> for method chaining.</returns>
    public static IInfiniFrameWindow SetFullScreen(this IInfiniFrameWindow window, bool fullScreen = true) {
        window.Features.State.SetFullScreen(fullScreen);
        return window;
    }

    /// <summary>
    ///     Sets focus to the window and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <returns>The <see cref="IInfiniFrameWindow"/> for method chaining.</returns>
    public static IInfiniFrameWindow SetFocused(this IInfiniFrameWindow window) {
        window.Features.State.SetFocused();
        return window;
    }

    /// <summary>
    ///     Sets the zoom factor for the window content and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="zoom">The zoom factor percentage.</param>
    /// <returns>The <see cref="IInfiniFrameWindow"/> for method chaining.</returns>
    public static IInfiniFrameWindow SetZoomFactor(this IInfiniFrameWindow window, int zoom) {
        window.Features.State.SetZoomFactor(zoom);
        return window;
    }

    /// <summary>
    ///     Sets whether zoom is enabled for the window content and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="zoomEnabled">Whether zoom should be enabled.</param>
    /// <returns>The <see cref="IInfiniFrameWindow"/> for method chaining.</returns>
    public static IInfiniFrameWindow EnableZoom(this IInfiniFrameWindow window, bool zoomEnabled = true) {
        window.Features.State.EnableZoom(zoomEnabled);
        return window;
    }

    /// <summary>
    ///     Sets whether the window is top-most and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="topMost">Whether the window should be top-most.</param>
    /// <returns>The <see cref="IInfiniFrameWindow"/> for method chaining.</returns>
    public static IInfiniFrameWindow SetTopMost(this IInfiniFrameWindow window, bool topMost = true) {
        window.Features.State.SetTopMost(topMost);
        return window;
    }
}