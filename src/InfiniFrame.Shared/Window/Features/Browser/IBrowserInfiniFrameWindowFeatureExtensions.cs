// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class IBrowserInfiniFrameWindowFeatureExtensions {
    /// <summary>
    ///     Enables or disables the status bar (URL hover indicator) for the window.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="enabled">Whether the status bar should be enabled.</param>
    /// <returns>The window instance for chaining.</returns>
    public static IInfiniFrameWindow EnableStatusBar(this IInfiniFrameWindow window, bool enabled = true) {
        window.Features.Browser.EnableStatusBar(enabled);
        return window;
    }

    /// <summary>
    ///     Enables or disables the context menu for the window.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="enabled">Whether the context menu should be enabled.</param>
    /// <returns>The window instance for chaining.</returns>
    public static IInfiniFrameWindow EnableContextMenu(this IInfiniFrameWindow window, bool enabled = true) {
        window.Features.Browser.EnableContextMenu(enabled);
        return window;
    }

    /// <summary>
    ///     Enables or disables media autoplay for the window.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="enabled">Whether media autoplay should be enabled.</param>
    /// <returns>The window instance for chaining.</returns>
    public static IInfiniFrameWindow EnableMediaAutoplay(this IInfiniFrameWindow window, bool enabled = true) {
        window.Features.Browser.EnableMediaAutoplay(enabled);
        return window;
    }

    /// <summary>
    ///     Sets the user agent string for the browser.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="userAgent">The user agent string to set.</param>
    /// <returns>The window instance for chaining.</returns>
    public static IInfiniFrameWindow SetUserAgent(this IInfiniFrameWindow window, string? userAgent) {
        window.Features.Browser.SetUserAgent(userAgent);
        return window;
    }

    /// <summary>
    ///     Sets the WebView2 runtime path on Windows.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="data">The WebView2 runtime path.</param>
    /// <returns>The window instance for chaining.</returns>
    public static IInfiniFrameWindow Win32SetWebView2Path(this IInfiniFrameWindow window, string data) {
        window.Features.Browser.Win32SetWebView2Path(data);
        return window;
    }

    /// <summary>
    ///     Clears the browser auto-fill data.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <returns>The window instance for chaining.</returns>
    public static IInfiniFrameWindow ClearBrowserAutoFill(this IInfiniFrameWindow window) {
        window.Features.Browser.ClearBrowserAutoFill();
        return window;
    }
}