// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Fluent extension methods for <see cref="IPositionInfiniFrameWindowBuilderFeature"/> on <see cref="IInfiniFrameWindowBuilder"/>.
/// </summary>
public static class IPositionInfiniFrameWindowBuilderFeatureExtensions {
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Sets the position of the window using pixel coordinates and returns the builder for chaining.
    /// </summary>
    /// <param name="builder">The window builder instance.</param>
    /// <param name="left">The left coordinate.</param>
    /// <param name="top">The top coordinate.</param>
    /// <returns>The <see cref="IInfiniFrameWindowBuilder" /> for method chaining.</returns>
    public static IInfiniFrameWindowBuilder SetLocation(this IInfiniFrameWindowBuilder builder, int left, int top) {
        builder.Features.Position.SetLocation(left, top);
        return builder;
    }

    /// <summary>
    ///     Sets the position of the window using a <see cref="Point" /> and returns the builder for chaining.
    /// </summary>
    /// <param name="builder">The window builder instance.</param>
    /// <param name="location">The location point.</param>
    /// <returns>The <see cref="IInfiniFrameWindowBuilder" /> for method chaining.</returns>
    public static IInfiniFrameWindowBuilder SetLocation(this IInfiniFrameWindowBuilder builder, Point location) {
        builder.Features.Position.SetLocation(location);
        return builder;
    }

    /// <summary>
    ///     Sets the left edge position of the window and returns the builder for chaining.
    /// </summary>
    /// <param name="builder">The window builder instance.</param>
    /// <param name="left">The left coordinate.</param>
    /// <returns>The <see cref="IInfiniFrameWindowBuilder" /> for method chaining.</returns>
    public static IInfiniFrameWindowBuilder SetLeft(this IInfiniFrameWindowBuilder builder, int left) {
        builder.Features.Position.SetLeft(left);
        return builder;
    }

    /// <summary>
    ///     Sets the top edge position of the window and returns the builder for chaining.
    /// </summary>
    /// <param name="builder">The window builder instance.</param>
    /// <param name="top">The top coordinate.</param>
    /// <returns>The <see cref="IInfiniFrameWindowBuilder" /> for method chaining.</returns>
    public static IInfiniFrameWindowBuilder SetTop(this IInfiniFrameWindowBuilder builder, int top) {
        builder.Features.Position.SetTop(top);
        return builder;
    }

    /// <summary>
    ///     Sets whether the window should use the OS default location and returns the builder for chaining.
    /// </summary>
    /// <param name="builder">The window builder instance.</param>
    /// <param name="enabled">Whether to use the OS default location.</param>
    /// <returns>The <see cref="IInfiniFrameWindowBuilder" /> for method chaining.</returns>
    public static IInfiniFrameWindowBuilder UseOsDefaultLocation(this IInfiniFrameWindowBuilder builder, bool enabled = true) {
        builder.Features.Position.UseOsDefaultLocation(enabled);
        return builder;
    }

    /// <summary>
    ///     Sets whether the window should be centered on the main monitor and returns the builder for chaining.
    /// </summary>
    /// <param name="builder">The window builder instance.</param>
    /// <param name="enabled">Whether to center on the main monitor.</param>
    /// <returns>The <see cref="IInfiniFrameWindowBuilder" /> for method chaining.</returns>
    public static IInfiniFrameWindowBuilder CenteredOnMainMonitor(this IInfiniFrameWindowBuilder builder, bool enabled = true) {
        builder.Features.Position.CenteredOnMainMonitor(enabled);
        return builder;
    }
}
