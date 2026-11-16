// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Fluent extension methods for <see cref="IStateInfiniFrameWindowBuilderFeature"/> on <see cref="IInfiniFrameWindowBuilder"/>.
/// </summary>
public static class IStateInfiniFrameWindowBuilderFeatureExtensions {
    /// <param name="builder">The window builder instance.</param>
    extension(IInfiniFrameWindowBuilder builder) {
        /// <summary>
        ///     Sets whether the window should start in full-screen mode and returns the builder for chaining.
        /// </summary>
        /// <param name="fullScreen">Whether to start in full-screen mode.</param>
        /// <returns>The <see cref="IInfiniFrameWindowBuilder" /> for method chaining.</returns>
        public IInfiniFrameWindowBuilder SetFullScreen(bool fullScreen) {
            builder.Features.State.SetFullScreen(fullScreen);
            return builder;
        }
        /// <summary>
        ///     Sets whether the window should start maximized and returns the builder for chaining.
        /// </summary>
        /// <param name="maximized">Whether to start maximized.</param>
        /// <returns>The <see cref="IInfiniFrameWindowBuilder" /> for method chaining.</returns>
        public IInfiniFrameWindowBuilder SetMaximized(bool maximized) {
            builder.Features.State.SetMaximized(maximized);
            return builder;
        }
        /// <summary>
        ///     Sets whether the window should start minimized and returns the builder for chaining.
        /// </summary>
        /// <param name="minimized">Whether to start minimized.</param>
        /// <returns>The <see cref="IInfiniFrameWindowBuilder" /> for method chaining.</returns>
        public IInfiniFrameWindowBuilder SetMinimized(bool minimized) {
            builder.Features.State.SetMinimized(minimized);
            return builder;
        }
        /// <summary>
        ///     Sets whether the window should start as top-most and returns the builder for chaining.
        /// </summary>
        /// <param name="topMost">Whether to start as top-most.</param>
        /// <returns>The <see cref="IInfiniFrameWindowBuilder" /> for method chaining.</returns>
        public IInfiniFrameWindowBuilder SetTopMost(bool topMost) {
            builder.Features.State.SetTopMost(topMost);
            return builder;
        }
        /// <summary>
        ///     Sets the initial zoom factor for the window content and returns the builder for chaining.
        /// </summary>
        /// <param name="zoom">The zoom factor percentage.</param>
        /// <returns>The <see cref="IInfiniFrameWindowBuilder" /> for method chaining.</returns>
        public IInfiniFrameWindowBuilder SetZoomFactor(int zoom) {
            builder.Features.State.SetZoomFactor(zoom);
            return builder;
        }
        /// <summary>
        ///     Sets whether zoom is enabled for the window content and returns the builder for chaining.
        /// </summary>
        /// <param name="zoomEnabled">Whether zoom should be enabled.</param>
        /// <returns>The <see cref="IInfiniFrameWindowBuilder" /> for method chaining.</returns>
        public IInfiniFrameWindowBuilder EnableZoom(bool zoomEnabled) {
            builder.Features.State.EnableZoom(zoomEnabled);
            return builder;
        }
    }

}
