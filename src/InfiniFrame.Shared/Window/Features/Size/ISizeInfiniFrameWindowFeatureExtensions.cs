// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Fluent extension methods for <see cref="ISizeInfiniFrameWindowFeature"/> on <see cref="IInfiniFrameWindow"/>.
/// </summary>
public static class ISizeInfiniFrameWindowFeatureExtensions {
    /// <summary>
    ///     Sets the size of the window using width and height values and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="width">The width in pixels.</param>
    /// <param name="height">The height in pixels.</param>
    /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
    public static IInfiniFrameWindow SetSize(this IInfiniFrameWindow window, int width, int height) {
        window.Features.Size.SetSize(width, height);
        return window;
    }

    /// <summary>
    ///     Sets the size of the window using a <see cref="Size" /> value and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="size">The size to set.</param>
    /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
    public static IInfiniFrameWindow SetSize(this IInfiniFrameWindow window, Size size) {
        window.Features.Size.SetSize(size);
        return window;
    }

    /// <summary>
    ///     Sets the height of the window and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="height">The height in pixels.</param>
    /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
    public static IInfiniFrameWindow SetHeight(this IInfiniFrameWindow window, int height) {
        window.Features.Size.SetHeight(height);
        return window;
    }

    /// <summary>
    ///     Sets the maximum size of the window and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="maxWidth">The maximum width in pixels.</param>
    /// <param name="maxHeight">The maximum height in pixels.</param>
    /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
    public static IInfiniFrameWindow SetMaxSize(this IInfiniFrameWindow window, int maxWidth, int maxHeight) {
        window.Features.Size.SetMaxSize(maxWidth, maxHeight);
        return window;
    }

    /// <summary>
    ///     Sets the maximum size of the window using a <see cref="Size" /> value and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="size">The maximum size.</param>
    /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
    public static IInfiniFrameWindow SetMaxSize(this IInfiniFrameWindow window, Size size) {
        window.Features.Size.SetMaxSize(size);
        return window;
    }

    /// <summary>
    ///     Sets the maximum height of the window and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="maxHeight">The maximum height in pixels.</param>
    /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
    public static IInfiniFrameWindow SetMaxHeight(this IInfiniFrameWindow window, int maxHeight) {
        window.Features.Size.SetMaxHeight(maxHeight);
        return window;
    }

    /// <summary>
    ///     Sets the maximum width of the window and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="maxWidth">The maximum width in pixels.</param>
    /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
    public static IInfiniFrameWindow SetMaxWidth(this IInfiniFrameWindow window, int maxWidth) {
        window.Features.Size.SetMaxWidth(maxWidth);
        return window;
    }

    /// <summary>
    ///     Sets the minimum size of the window and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="minWidth">The minimum width in pixels.</param>
    /// <param name="minHeight">The minimum height in pixels.</param>
    /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
    public static IInfiniFrameWindow SetMinSize(this IInfiniFrameWindow window, int minWidth, int minHeight) {
        window.Features.Size.SetMinSize(minWidth, minHeight);
        return window;
    }

    /// <summary>
    ///     Sets the minimum size of the window using a <see cref="Size" /> value and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="size">The minimum size.</param>
    /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
    public static IInfiniFrameWindow SetMinSize(this IInfiniFrameWindow window, Size size) {
        window.Features.Size.SetMinSize(size);
        return window;
    }

    /// <summary>
    ///     Sets the minimum height of the window and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="minHeight">The minimum height in pixels.</param>
    /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
    public static IInfiniFrameWindow SetMinHeight(this IInfiniFrameWindow window, int minHeight) {
        window.Features.Size.SetMinHeight(minHeight);
        return window;
    }

    /// <summary>
    ///     Sets the minimum width of the window and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="minWidth">The minimum width in pixels.</param>
    /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
    public static IInfiniFrameWindow SetMinWidth(this IInfiniFrameWindow window, int minWidth) {
        window.Features.Size.SetMinWidth(minWidth);
        return window;
    }

    /// <summary>
    ///     Sets the width of the window and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="width">The width in pixels.</param>
    /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
    public static IInfiniFrameWindow SetWidth(this IInfiniFrameWindow window, int width) {
        window.Features.Size.SetWidth(width);
        return window;
    }

    /// <summary>
    ///     Resizes the window by the specified offsets from the given origin and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="widthOffset">The width offset in pixels.</param>
    /// <param name="heightOffset">The height offset in pixels.</param>
    /// <param name="origin">The origin point for the resize operation.</param>
    /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
    public static IInfiniFrameWindow Resize(this IInfiniFrameWindow window, int widthOffset, int heightOffset, ResizeOrigin origin) {
        window.Features.Size.Resize(widthOffset, heightOffset, origin);
        return window;
    }

    /// <summary>
    ///     Sets whether the window is resizable and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="resizable">Whether the window should be resizable.</param>
    /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
    public static IInfiniFrameWindow SetResizable(this IInfiniFrameWindow window, bool resizable = true) {
        window.Features.Size.SetResizable(resizable);
        return window;
    }
}
