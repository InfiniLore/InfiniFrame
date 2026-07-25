// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class ISizeInfiniFrameWindowBuilderFeatureExtensions {
    /// <summary>
    ///     Sets the size of the window using width and height values and returns the builder for chaining.
    /// </summary>
    /// <param name="builder">The window builder instance.</param>
    /// <param name="width">The width in pixels.</param>
    /// <param name="height">The height in pixels.</param>
    /// <returns>The <see cref="IInfiniFrameWindowBuilder"/> for method chaining.</returns>
    public static IInfiniFrameWindowBuilder SetSize(this IInfiniFrameWindowBuilder builder, int width, int height) {
        builder.Features.Size.SetSize(width, height);
        return builder;
    }
    
    /// <summary>
    ///     Sets the size of the window using a <see cref="Size"/> value and returns the builder for chaining.
    /// </summary>
    /// <param name="builder">The window builder instance.</param>
    /// <param name="size">The size to set.</param>
    /// <returns>The <see cref="IInfiniFrameWindowBuilder"/> for method chaining.</returns>
    public static IInfiniFrameWindowBuilder SetSize(this IInfiniFrameWindowBuilder builder, Size size) {
        builder.Features.Size.SetSize(size);
        return builder;
    }
    
    /// <summary>
    ///     Sets the height of the window and returns the builder for chaining.
    /// </summary>
    /// <param name="builder">The window builder instance.</param>
    /// <param name="height">The height in pixels.</param>
    /// <returns>The <see cref="IInfiniFrameWindowBuilder"/> for method chaining.</returns>
    public static IInfiniFrameWindowBuilder SetHeight(this IInfiniFrameWindowBuilder builder, int height) {
        builder.Features.Size.SetHeight(height);
        return builder;
    }
    
    /// <summary>
    ///     Sets the width of the window and returns the builder for chaining.
    /// </summary>
    /// <param name="builder">The window builder instance.</param>
    /// <param name="width">The width in pixels.</param>
    /// <returns>The <see cref="IInfiniFrameWindowBuilder"/> for method chaining.</returns>
    public static IInfiniFrameWindowBuilder SetWidth(this IInfiniFrameWindowBuilder builder, int width) {
        builder.Features.Size.SetWidth(width);
        return builder;
    }
    
    /// <summary>
    ///     Sets the maximum size of the window and returns the builder for chaining.
    /// </summary>
    /// <param name="builder">The window builder instance.</param>
    /// <param name="maxWidth">The maximum width in pixels.</param>
    /// <param name="maxHeight">The maximum height in pixels.</param>
    /// <returns>The <see cref="IInfiniFrameWindowBuilder"/> for method chaining.</returns>
    public static IInfiniFrameWindowBuilder SetMaxSize(this IInfiniFrameWindowBuilder builder, int maxWidth, int maxHeight) {
        builder.Features.Size.SetMaxSize(maxWidth, maxHeight);
        return builder;
    }
    
    /// <summary>
    ///     Sets the maximum size of the window using a <see cref="Size"/> value and returns the builder for chaining.
    /// </summary>
    /// <param name="builder">The window builder instance.</param>
    /// <param name="size">The maximum size.</param>
    /// <returns>The <see cref="IInfiniFrameWindowBuilder"/> for method chaining.</returns>
    public static IInfiniFrameWindowBuilder SetMaxSize(this IInfiniFrameWindowBuilder builder, Size size) {
        builder.Features.Size.SetMaxSize(size);
        return builder;
    }
    
    /// <summary>
    ///     Sets the maximum height of the window and returns the builder for chaining.
    /// </summary>
    /// <param name="builder">The window builder instance.</param>
    /// <param name="maxHeight">The maximum height in pixels.</param>
    /// <returns>The <see cref="IInfiniFrameWindowBuilder"/> for method chaining.</returns>
    public static IInfiniFrameWindowBuilder SetMaxHeight(this IInfiniFrameWindowBuilder builder, int maxHeight) {
        builder.Features.Size.SetMaxHeight(maxHeight);
        return builder;
    }
    
    /// <summary>
    ///     Sets the maximum width of the window and returns the builder for chaining.
    /// </summary>
    /// <param name="builder">The window builder instance.</param>
    /// <param name="maxWidth">The maximum width in pixels.</param>
    /// <returns>The <see cref="IInfiniFrameWindowBuilder"/> for method chaining.</returns>
    public static IInfiniFrameWindowBuilder SetMaxWidth(this IInfiniFrameWindowBuilder builder, int maxWidth) {
        builder.Features.Size.SetMaxWidth(maxWidth);
        return builder;
    }
    
    /// <summary>
    ///     Sets the minimum size of the window and returns the builder for chaining.
    /// </summary>
    /// <param name="builder">The window builder instance.</param>
    /// <param name="minWidth">The minimum width in pixels.</param>
    /// <param name="minHeight">The minimum height in pixels.</param>
    /// <returns>The <see cref="IInfiniFrameWindowBuilder"/> for method chaining.</returns>
    public static IInfiniFrameWindowBuilder SetMinSize(this IInfiniFrameWindowBuilder builder, int minWidth, int minHeight) {
        builder.Features.Size.SetMinSize(minWidth, minHeight);
        return builder;
    }
    
    /// <summary>
    ///     Sets the minimum size of the window using a <see cref="Size"/> value and returns the builder for chaining.
    /// </summary>
    /// <param name="builder">The window builder instance.</param>
    /// <param name="size">The minimum size.</param>
    /// <returns>The <see cref="IInfiniFrameWindowBuilder"/> for method chaining.</returns>
    public static IInfiniFrameWindowBuilder SetMinSize(this IInfiniFrameWindowBuilder builder, Size size) {
        builder.Features.Size.SetMinSize(size);
        return builder;
    }
    
    /// <summary>
    ///     Sets the minimum height of the window and returns the builder for chaining.
    /// </summary>
    /// <param name="builder">The window builder instance.</param>
    /// <param name="minHeight">The minimum height in pixels.</param>
    /// <returns>The <see cref="IInfiniFrameWindowBuilder"/> for method chaining.</returns>
    public static IInfiniFrameWindowBuilder SetMinHeight(this IInfiniFrameWindowBuilder builder, int minHeight) {
        builder.Features.Size.SetMinHeight(minHeight);
        return builder;
    }
    
    /// <summary>
    ///     Sets the minimum width of the window and returns the builder for chaining.
    /// </summary>
    /// <param name="builder">The window builder instance.</param>
    /// <param name="minWidth">The minimum width in pixels.</param>
    /// <returns>The <see cref="IInfiniFrameWindowBuilder"/> for method chaining.</returns>
    public static IInfiniFrameWindowBuilder SetMinWidth(this IInfiniFrameWindowBuilder builder, int minWidth) {
        builder.Features.Size.SetMinWidth(minWidth);
        return builder;
    }
    
    /// <summary>
    ///     Sets whether the window should start with the OS default size and returns the builder for chaining.
    /// </summary>
    /// <param name="builder">The window builder instance.</param>
    /// <param name="enabled">Whether to use the OS default size.</param>
    /// <returns>The <see cref="IInfiniFrameWindowBuilder"/> for method chaining.</returns>
    public static IInfiniFrameWindowBuilder UseOsDefaultSize(this IInfiniFrameWindowBuilder builder, bool enabled = true) {
        builder.Features.Size.UseOsDefaultSize(enabled);
        return builder;
    }
    
    /// <summary>
    ///     Sets whether the window is resizable and returns the builder for chaining.
    /// </summary>
    /// <param name="builder">The window builder instance.</param>
    /// <param name="enabled">Whether the window should be resizable.</param>
    /// <returns>The <see cref="IInfiniFrameWindowBuilder"/> for method chaining.</returns>
    public static IInfiniFrameWindowBuilder SetResizable(this IInfiniFrameWindowBuilder builder, bool enabled = true) {
        builder.Features.Size.SetResizable(enabled);
        return builder;
    }
}
