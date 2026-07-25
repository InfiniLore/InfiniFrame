// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IStateInfiniFrameWindowFeature {
    /// <summary>
    ///     Gets whether the window is currently in full-screen mode.
    /// </summary>
    bool IsFullScreen { get; }

    /// <summary>
    ///     Gets whether the window is currently maximized.
    /// </summary>
    bool IsMaximized { get; }

    /// <summary>
    ///     Gets whether the window is currently minimized.
    /// </summary>
    bool IsMinimized { get; }

    /// <summary>
    ///     Gets whether the window is currently top-most.
    /// </summary>
    bool IsTopMost { get; }

    /// <summary>
    ///     Gets whether the window currently has focus.
    /// </summary>
    bool IsFocused { get; }

    /// <summary>
    ///     Gets the current zoom factor of the window content.
    /// </summary>
    int ZoomFactor { get; }

    /// <summary>
    ///     Gets whether zoom is currently enabled for the window.
    /// </summary>
    bool IsZoomEnabled { get; }

    /// <summary>
    ///     Gets or sets the cached bounds of the window before entering full-screen mode.
    /// </summary>
    Rectangle CachedPreFullScreenBounds { get; set; }

    /// <summary>
    ///     Gets or sets the cached bounds of the window before being maximized.
    /// </summary>
    Rectangle CachedPreMaximizedBounds { get; set; }

    /// <summary>
    ///     Sets whether the window is maximized.
    /// </summary>
    /// <param name="maximized">Whether to maximize the window.</param>
    void SetMaximized(bool maximized = true);

    /// <summary>
    ///     Toggles the maximized state of the window.
    /// </summary>
    void ToggleMaximized();

    /// <summary>
    ///     Sets whether the window is minimized.
    /// </summary>
    /// <param name="minimized">Whether to minimize the window.</param>
    void SetMinimized(bool minimized = true);

    /// <summary>
    ///     Sets whether the window is in full-screen mode.
    /// </summary>
    /// <param name="fullScreen">Whether to enter full-screen mode.</param>
    void SetFullScreen(bool fullScreen = true);

    /// <summary>
    ///     Sets focus to the window.
    /// </summary>
    void SetFocused();

    /// <summary>
    ///     Sets the zoom factor for the window content.
    /// </summary>
    /// <param name="zoom">The zoom factor percentage.</param>
    void SetZoomFactor(int zoom);

    /// <summary>
    ///     Sets whether zoom is enabled for the window.
    /// </summary>
    /// <param name="zoomEnabled">Whether zoom should be enabled.</param>
    void EnableZoom(bool zoomEnabled = true);

    /// <summary>
    ///     Sets whether the window is top-most.
    /// </summary>
    /// <param name="topMost">Whether the window should be top-most.</param>
    void SetTopMost(bool topMost = true);
}
