// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IStateInfiniFrameWindowBuilderFeature : IInfiniFrameWindowBuilderFeature {
    /// <summary>
    ///     Gets whether the window should start in full-screen mode.
    /// </summary>
    bool StartFullScreen { get; }

    /// <summary>
    ///     Gets whether the window should start maximized.
    /// </summary>
    bool StartMaximized { get; }

    /// <summary>
    ///     Gets whether the window should start minimized.
    /// </summary>
    bool StartMinimized { get; }

    /// <summary>
    ///     Gets whether the window should start as top-most.
    /// </summary>
    bool StartTopMost { get; }

    /// <summary>
    ///     Gets the initial zoom factor for the window content.
    /// </summary>
    int ZoomFactor { get; }

    /// <summary>
    ///     Gets whether zoom is enabled for the window.
    /// </summary>
    bool IsZoomEnabled { get; }

    /// <summary>
    ///     Sets whether the window should start in full-screen mode.
    /// </summary>
    /// <param name="fullScreen">Whether to start in full-screen mode.</param>
    void SetFullScreen(bool fullScreen);

    /// <summary>
    ///     Sets whether the window should start maximized.
    /// </summary>
    /// <param name="maximized">Whether to start maximized.</param>
    void SetMaximized(bool maximized);

    /// <summary>
    ///     Sets whether the window should start minimized.
    /// </summary>
    /// <param name="minimized">Whether to start minimized.</param>
    void SetMinimized(bool minimized);

    /// <summary>
    ///     Sets whether the window should start as top-most.
    /// </summary>
    /// <param name="topMost">Whether to start as top-most.</param>
    void SetTopMost(bool topMost);

    /// <summary>
    ///     Sets the initial zoom factor for the window content.
    /// </summary>
    /// <param name="zoom">The zoom factor percentage.</param>
    void SetZoomFactor(int zoom);

    /// <summary>
    ///     Sets whether zoom is enabled for the window.
    /// </summary>
    /// <param name="zoomEnabled">Whether zoom should be enabled.</param>
    void EnableZoom(bool zoomEnabled);
}
