// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowBuilderFeatureSize : IInfiniFrameWindowBuilderFeature{
    /// <summary>
    ///     Gets the configured height of the window.
    /// </summary>
    int Height { get; } 

    /// <summary>
    ///     Gets the configured width of the window.
    /// </summary>
    int Width { get; }

    /// <summary>
    ///     Gets the configured maximum height of the window.
    /// </summary>
    int MaxHeight { get; }

    /// <summary>
    ///     Gets the configured maximum width of the window.
    /// </summary>
    int MaxWidth { get; }

    /// <summary>
    ///     Gets the configured minimum height of the window.
    /// </summary>
    int MinHeight { get; }

    /// <summary>
    ///     Gets the configured minimum width of the window.
    /// </summary>
    int MinWidth { get; }

    /// <summary>
    ///     Gets whether the window is resizable.
    /// </summary>
    bool IsResizable { get; }

    /// <summary>
    ///     Gets whether the window should start with the OS default size.
    /// </summary>
    bool StartWithOsDefaultSize { get; }
    
    /// <summary>
    ///     Sets the size of the window using width and height values.
    /// </summary>
    /// <param name="width">The width in pixels.</param>
    /// <param name="height">The height in pixels.</param>
    void SetSize(int width, int height);

    /// <summary>
    ///     Sets the size of the window using a <see cref="Size"/> value.
    /// </summary>
    /// <param name="size">The size to set.</param>
    void SetSize(Size size);

    /// <summary>
    ///     Sets the height of the window.
    /// </summary>
    /// <param name="height">The height in pixels.</param>
    void SetHeight(int height);

    /// <summary>
    ///     Sets the width of the window.
    /// </summary>
    /// <param name="width">The width in pixels.</param>
    void SetWidth(int width);

    /// <summary>
    ///     Sets the maximum size of the window.
    /// </summary>
    /// <param name="maxWidth">The maximum width in pixels.</param>
    /// <param name="maxHeight">The maximum height in pixels.</param>
    void SetMaxSize(int maxWidth, int maxHeight);

    /// <summary>
    ///     Sets the maximum size of the window using a <see cref="Size"/> value.
    /// </summary>
    /// <param name="size">The maximum size.</param>
    void SetMaxSize(Size size);

    /// <summary>
    ///     Sets the maximum height of the window.
    /// </summary>
    /// <param name="maxHeight">The maximum height in pixels.</param>
    void SetMaxHeight(int maxHeight);

    /// <summary>
    ///     Sets the maximum width of the window.
    /// </summary>
    /// <param name="maxWidth">The maximum width in pixels.</param>
    void SetMaxWidth(int maxWidth);

    /// <summary>
    ///     Sets the minimum size of the window.
    /// </summary>
    /// <param name="minWidth">The minimum width in pixels.</param>
    /// <param name="minHeight">The minimum height in pixels.</param>
    void SetMinSize(int minWidth, int minHeight);

    /// <summary>
    ///     Sets the minimum size of the window using a <see cref="Size"/> value.
    /// </summary>
    /// <param name="size">The minimum size.</param>
    void SetMinSize(Size size);

    /// <summary>
    ///     Sets the minimum height of the window.
    /// </summary>
    /// <param name="minHeight">The minimum height in pixels.</param>
    void SetMinHeight(int minHeight);

    /// <summary>
    ///     Sets the minimum width of the window.
    /// </summary>
    /// <param name="minWidth">The minimum width in pixels.</param>
    void SetMinWidth(int minWidth);

    /// <summary>
    ///     Sets whether the window should start with the OS default size.
    /// </summary>
    /// <param name="enabled">Whether to use the OS default size.</param>
    void UseOsDefaultSize(bool enabled = true);

    /// <summary>
    ///     Sets whether the window is resizable.
    /// </summary>
    /// <param name="resizable">Whether the window should be resizable.</param>
    void SetResizable(bool resizable = true);
}
