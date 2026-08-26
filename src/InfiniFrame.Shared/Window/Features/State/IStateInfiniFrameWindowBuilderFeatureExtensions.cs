// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class IStateInfiniFrameWindowBuilderFeatureExtensions {
    /// <summary>
    ///     Sets whether the window should start in full-screen mode and returns the builder for chaining.
    /// </summary>
    /// <param name="builder">The window builder instance.</param>
    /// <param name="fullScreen">Whether to start in full-screen mode.</param>
    /// <returns>The <see cref="IInfiniFrameWindowBuilder" /> for method chaining.</returns>
    public static IInfiniFrameWindowBuilder SetFullScreen(this IInfiniFrameWindowBuilder builder, bool fullScreen) {
        builder.Features.State.SetFullScreen(fullScreen);
        return builder;
    }

    /// <summary>
    ///     Sets whether the window should start maximized and returns the builder for chaining.
    /// </summary>
    /// <param name="builder">The window builder instance.</param>
    /// <param name="maximized">Whether to start maximized.</param>
    /// <returns>The <see cref="IInfiniFrameWindowBuilder" /> for method chaining.</returns>
    public static IInfiniFrameWindowBuilder SetMaximized(this IInfiniFrameWindowBuilder builder, bool maximized) {
        builder.Features.State.SetMaximized(maximized);
        return builder;
    }

    /// <summary>
    ///     Sets whether the window should start minimized and returns the builder for chaining.
    /// </summary>
    /// <param name="builder">The window builder instance.</param>
    /// <param name="minimized">Whether to start minimized.</param>
    /// <returns>The <see cref="IInfiniFrameWindowBuilder" /> for method chaining.</returns>
    public static IInfiniFrameWindowBuilder SetMinimized(this IInfiniFrameWindowBuilder builder, bool minimized) {
        builder.Features.State.SetMinimized(minimized);
        return builder;
    }

    /// <summary>
    ///     Sets whether the window should start as top-most and returns the builder for chaining.
    /// </summary>
    /// <param name="builder">The window builder instance.</param>
    /// <param name="topMost">Whether to start as top-most.</param>
    /// <returns>The <see cref="IInfiniFrameWindowBuilder" /> for method chaining.</returns>
    public static IInfiniFrameWindowBuilder SetTopMost(this IInfiniFrameWindowBuilder builder, bool topMost) {
        builder.Features.State.SetTopMost(topMost);
        return builder;
    }

    /// <summary>
    ///     Sets the initial zoom factor for the window content and returns the builder for chaining.
    /// </summary>
    /// <param name="builder">The window builder instance.</param>
    /// <param name="zoom">The zoom factor percentage.</param>
    /// <returns>The <see cref="IInfiniFrameWindowBuilder" /> for method chaining.</returns>
    public static IInfiniFrameWindowBuilder SetZoomFactor(this IInfiniFrameWindowBuilder builder, int zoom) {
        builder.Features.State.SetZoomFactor(zoom);
        return builder;
    }

    /// <summary>
    ///     Sets whether zoom is enabled for the window content and returns the builder for chaining.
    /// </summary>
    /// <param name="builder">The window builder instance.</param>
    /// <param name="zoomEnabled">Whether zoom should be enabled.</param>
    /// <returns>The <see cref="IInfiniFrameWindowBuilder" /> for method chaining.</returns>
    public static IInfiniFrameWindowBuilder EnableZoom(this IInfiniFrameWindowBuilder builder, bool zoomEnabled) {
        builder.Features.State.EnableZoom(zoomEnabled);
        return builder;
    }
}
