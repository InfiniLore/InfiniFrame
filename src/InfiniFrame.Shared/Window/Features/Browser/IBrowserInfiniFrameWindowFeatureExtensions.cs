// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Fluent extension methods for <see cref="IBrowserInfiniFrameWindowFeature" /> on <see cref="IInfiniFrameWindow" />.
/// </summary>
public static class IBrowserInfiniFrameWindowFeatureExtensions {
    /// <param name="window">The window instance.</param>
    extension(IInfiniFrameWindow window) {
        /// <summary>
        ///     Enables or disables the status bar (URL hover indicator) for the window.
        /// </summary>
        /// <param name="enabled">Whether the status bar should be enabled.</param>
        /// <returns>The window instance for chaining.</returns>
        public IInfiniFrameWindow EnableStatusBar(bool enabled = true) {
            window.Features.Browser.EnableStatusBar(enabled);
            return window;
        }
        /// <summary>
        ///     Enables or disables browser keyboard shortcuts (e.g. Ctrl+T, Ctrl+W, F11) for the window.
        /// </summary>
        /// <param name="enabled">Whether browser shortcuts should be enabled.</param>
        /// <returns>The window instance for chaining.</returns>
        public IInfiniFrameWindow EnableBrowserShortcuts(bool enabled = true) {
            window.Features.Browser.EnableBrowserShortcuts(enabled);
            return window;
        }
        /// <summary>
        ///     Enables or disables the context menu for the window.
        /// </summary>
        /// <param name="enabled">Whether the context menu should be enabled.</param>
        /// <returns>The window instance for chaining.</returns>
        public IInfiniFrameWindow EnableContextMenu(bool enabled = true) {
            window.Features.Browser.EnableContextMenu(enabled);
            return window;
        }
        /// <summary>
        ///     Enables or disables media autoplay for the window.
        /// </summary>
        /// <param name="enabled">Whether media autoplay should be enabled.</param>
        /// <returns>The window instance for chaining.</returns>
        public IInfiniFrameWindow EnableMediaAutoplay(bool enabled = true) {
            window.Features.Browser.EnableMediaAutoplay(enabled);
            return window;
        }
        /// <summary>
        ///     Sets the user agent string for the browser.
        /// </summary>
        /// <param name="userAgent">The user agent string to set.</param>
        /// <returns>The window instance for chaining.</returns>
        public IInfiniFrameWindow SetUserAgent(string? userAgent) {
            window.Features.Browser.SetUserAgent(userAgent);
            return window;
        }
        /// <summary>
        ///     Sets the WebView2 runtime path on Windows.
        /// </summary>
        /// <param name="data">The WebView2 runtime path.</param>
        /// <returns>The window instance for chaining.</returns>
        public IInfiniFrameWindow Win32SetWebView2Path(string data) {
            window.Features.Browser.Win32SetWebView2Path(data);
            return window;
        }
        /// <summary>
        ///     Clears the browser auto-fill data.
        /// </summary>
        /// <returns>The window instance for chaining.</returns>
        public IInfiniFrameWindow ClearBrowserAutoFill() {
            window.Features.Browser.ClearBrowserAutoFill();
            return window;
        }
    }

}
