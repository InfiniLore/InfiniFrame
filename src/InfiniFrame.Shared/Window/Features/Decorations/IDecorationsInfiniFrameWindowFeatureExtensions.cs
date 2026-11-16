// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Fluent extension methods for <see cref="IDecorationsInfiniFrameWindowFeature" /> on <see cref="IInfiniFrameWindow" />.
/// </summary>
public static class IDecorationsInfiniFrameWindowFeatureExtensions {
    /// <param name="window">The window instance.</param>
    extension(IInfiniFrameWindow window) {
        /// <summary>
        ///     Enables or disables window transparency.
        /// </summary>
        /// <param name="enabled">Whether transparency should be enabled.</param>
        /// <returns>The window instance for chaining.</returns>
        public IInfiniFrameWindow SetTransparent(bool enabled = true) {
            window.Features.Decorations.SetTransparent(enabled);
            return window;
        }
        /// <summary>
        ///     Sets the window background color. Pass <c>null</c> or <c>"transparent"</c> to reset to default.
        /// </summary>
        /// <param name="color">A hex color string (e.g. "#RRGGBB" or "#AARRGGBB"), or <c>null</c>/<c>"transparent"</c> to reset.</param>
        /// <returns>The window instance for chaining.</returns>
        public IInfiniFrameWindow SetBackgroundColor(string? color) {
            window.Features.Decorations.SetBackgroundColor(color);
            return window;
        }
        /// <summary>
        ///     Sets the window title.
        /// </summary>
        /// <param name="title">The title to set.</param>
        /// <returns>The window instance for chaining.</returns>
        public IInfiniFrameWindow SetTitle(string? title) {
            window.Features.Decorations.SetTitle(title);
            return window;
        }
        /// <summary>
        ///     Sets the window icon from a file path.
        /// </summary>
        /// <param name="iconFilePath">The path to the icon file.</param>
        /// <returns>The window instance for chaining.</returns>
        public IInfiniFrameWindow SetIconFile(string iconFilePath) {
            window.Features.Decorations.SetIconFile(iconFilePath);
            return window;
        }
        /// <summary>
        ///     Sets whether the Linux window title length should be limited.
        /// </summary>
        /// <param name="enabled">Whether the title length should be limited.</param>
        /// <returns>The window instance for chaining.</returns>
        public IInfiniFrameWindow SetLimitLinuxWindowTitleLength(bool enabled = true) {
            window.Features.Decorations.SetLimitLinuxWindowTitleLength(enabled);
            return window;
        }
    }

}
