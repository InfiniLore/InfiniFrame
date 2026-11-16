// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Fluent extension methods for <see cref="IStateInfiniFrameWindowFeature" /> on <see cref="IInfiniFrameWindow" />.
/// </summary>
public static class IStateInfiniFrameWindowFeatureExtension {
    /// <param name="window">The window instance.</param>
    extension(IInfiniFrameWindow window) {
        /// <summary>
        ///     Sets whether the window is maximized and returns the window for chaining.
        /// </summary>
        /// <param name="maximized">Whether to maximize the window.</param>
        /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
        public IInfiniFrameWindow SetMaximized(bool maximized = true) {
            window.Features.State.SetMaximized(maximized);
            return window;
        }
        /// <summary>
        ///     Toggles the maximized state of the window and returns the window for chaining.
        /// </summary>
        /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
        public IInfiniFrameWindow ToggleMaximized() {
            window.Features.State.ToggleMaximized();
            return window;
        }
        /// <summary>
        ///     Sets whether the window is minimized and returns the window for chaining.
        /// </summary>
        /// <param name="minimized">Whether to minimize the window.</param>
        /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
        public IInfiniFrameWindow SetMinimized(bool minimized = true) {
            window.Features.State.SetMinimized(minimized);
            return window;
        }
        /// <summary>
        ///     Sets whether the window is in full-screen mode and returns the window for chaining.
        /// </summary>
        /// <param name="fullScreen">Whether to enter full-screen mode.</param>
        /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
        public IInfiniFrameWindow SetFullScreen(bool fullScreen = true) {
            window.Features.State.SetFullScreen(fullScreen);
            return window;
        }
        /// <summary>
        ///     Sets focus to the window and returns the window for chaining.
        /// </summary>
        /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
        public IInfiniFrameWindow SetFocused() {
            window.Features.State.SetFocused();
            return window;
        }
        /// <summary>
        ///     Sets the zoom factor for the window content and returns the window for chaining.
        /// </summary>
        /// <param name="zoom">The zoom factor percentage.</param>
        /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
        public IInfiniFrameWindow SetZoomFactor(int zoom) {
            window.Features.State.SetZoomFactor(zoom);
            return window;
        }
        /// <summary>
        ///     Sets whether zoom is enabled for the window content and returns the window for chaining.
        /// </summary>
        /// <param name="zoomEnabled">Whether zoom should be enabled.</param>
        /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
        public IInfiniFrameWindow EnableZoom(bool zoomEnabled = true) {
            window.Features.State.EnableZoom(zoomEnabled);
            return window;
        }
        /// <summary>
        ///     Sets whether the window is top-most and returns the window for chaining.
        /// </summary>
        /// <param name="topMost">Whether the window should be top-most.</param>
        /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
        public IInfiniFrameWindow SetTopMost(bool topMost = true) {
            window.Features.State.SetTopMost(topMost);
            return window;
        }
    }

}
