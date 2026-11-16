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
    /// <param name="window">The window instance.</param>
    extension(IInfiniFrameWindow window) {
        /// <summary>
        ///     Sets the size of the window using width and height values and returns the window for chaining.
        /// </summary>
        /// <param name="width">The width in pixels.</param>
        /// <param name="height">The height in pixels.</param>
        /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
        public IInfiniFrameWindow SetSize(int width, int height) {
            window.Features.Size.SetSize(width, height);
            return window;
        }
        /// <summary>
        ///     Sets the size of the window using a <see cref="Size" /> value and returns the window for chaining.
        /// </summary>
        /// <param name="size">The size to set.</param>
        /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
        public IInfiniFrameWindow SetSize(Size size) {
            window.Features.Size.SetSize(size);
            return window;
        }
        /// <summary>
        ///     Sets the height of the window and returns the window for chaining.
        /// </summary>
        /// <param name="height">The height in pixels.</param>
        /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
        public IInfiniFrameWindow SetHeight(int height) {
            window.Features.Size.SetHeight(height);
            return window;
        }
        /// <summary>
        ///     Sets the maximum size of the window and returns the window for chaining.
        /// </summary>
        /// <param name="maxWidth">The maximum width in pixels.</param>
        /// <param name="maxHeight">The maximum height in pixels.</param>
        /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
        public IInfiniFrameWindow SetMaxSize(int maxWidth, int maxHeight) {
            window.Features.Size.SetMaxSize(maxWidth, maxHeight);
            return window;
        }
        /// <summary>
        ///     Sets the maximum size of the window using a <see cref="Size" /> value and returns the window for chaining.
        /// </summary>
        /// <param name="size">The maximum size.</param>
        /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
        public IInfiniFrameWindow SetMaxSize(Size size) {
            window.Features.Size.SetMaxSize(size);
            return window;
        }
        /// <summary>
        ///     Sets the maximum height of the window and returns the window for chaining.
        /// </summary>
        /// <param name="maxHeight">The maximum height in pixels.</param>
        /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
        public IInfiniFrameWindow SetMaxHeight(int maxHeight) {
            window.Features.Size.SetMaxHeight(maxHeight);
            return window;
        }
        /// <summary>
        ///     Sets the maximum width of the window and returns the window for chaining.
        /// </summary>
        /// <param name="maxWidth">The maximum width in pixels.</param>
        /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
        public IInfiniFrameWindow SetMaxWidth(int maxWidth) {
            window.Features.Size.SetMaxWidth(maxWidth);
            return window;
        }
        /// <summary>
        ///     Sets the minimum size of the window and returns the window for chaining.
        /// </summary>
        /// <param name="minWidth">The minimum width in pixels.</param>
        /// <param name="minHeight">The minimum height in pixels.</param>
        /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
        public IInfiniFrameWindow SetMinSize(int minWidth, int minHeight) {
            window.Features.Size.SetMinSize(minWidth, minHeight);
            return window;
        }
        /// <summary>
        ///     Sets the minimum size of the window using a <see cref="Size" /> value and returns the window for chaining.
        /// </summary>
        /// <param name="size">The minimum size.</param>
        /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
        public IInfiniFrameWindow SetMinSize(Size size) {
            window.Features.Size.SetMinSize(size);
            return window;
        }
        /// <summary>
        ///     Sets the minimum height of the window and returns the window for chaining.
        /// </summary>
        /// <param name="minHeight">The minimum height in pixels.</param>
        /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
        public IInfiniFrameWindow SetMinHeight(int minHeight) {
            window.Features.Size.SetMinHeight(minHeight);
            return window;
        }
        /// <summary>
        ///     Sets the minimum width of the window and returns the window for chaining.
        /// </summary>
        /// <param name="minWidth">The minimum width in pixels.</param>
        /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
        public IInfiniFrameWindow SetMinWidth(int minWidth) {
            window.Features.Size.SetMinWidth(minWidth);
            return window;
        }
        /// <summary>
        ///     Sets the width of the window and returns the window for chaining.
        /// </summary>
        /// <param name="width">The width in pixels.</param>
        /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
        public IInfiniFrameWindow SetWidth(int width) {
            window.Features.Size.SetWidth(width);
            return window;
        }
        /// <summary>
        ///     Resizes the window by the specified offsets from the given origin and returns the window for chaining.
        /// </summary>
        /// <param name="widthOffset">The width offset in pixels.</param>
        /// <param name="heightOffset">The height offset in pixels.</param>
        /// <param name="origin">The origin point for the resize operation.</param>
        /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
        public IInfiniFrameWindow Resize(int widthOffset, int heightOffset, ResizeOrigin origin) {
            window.Features.Size.Resize(widthOffset, heightOffset, origin);
            return window;
        }
        /// <summary>
        ///     Sets whether the window is resizable and returns the window for chaining.
        /// </summary>
        /// <param name="resizable">Whether the window should be resizable.</param>
        /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
        public IInfiniFrameWindow SetResizable(bool resizable = true) {
            window.Features.Size.SetResizable(resizable);
            return window;
        }
    }

}
