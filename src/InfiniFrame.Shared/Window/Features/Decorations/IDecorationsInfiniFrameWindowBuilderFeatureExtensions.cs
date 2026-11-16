// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Fluent extension methods for <see cref="IDecorationsInfiniFrameWindowBuilderFeature"/> on <see cref="IInfiniFrameWindowBuilder"/>.
/// </summary>
public static class IDecorationsInfiniFrameWindowBuilderFeatureExtensions {
    /// <param name="builder">The builder instance.</param>
    extension(IInfiniFrameWindowBuilder builder) {
        /// <summary>
        ///     Sets whether the window should be chromeless.
        /// </summary>
        /// <param name="enabled">Whether the window should be chromeless.</param>
        /// <returns>The builder instance for chaining.</returns>
        public IInfiniFrameWindowBuilder SetChromeless(bool enabled = true) {
            builder.Features.Decorations.SetChromeless(enabled);
            return builder;
        }
        /// <summary>
        ///     Sets whether the window should be transparent.
        /// </summary>
        /// <param name="enabled">Whether the window should be transparent.</param>
        /// <returns>The builder instance for chaining.</returns>
        public IInfiniFrameWindowBuilder SetTransparent(bool enabled = true) {
            builder.Features.Decorations.SetTransparent(enabled);
            return builder;
        }
        /// <summary>
        ///     Sets the window background color. Pass <c>null</c> or <c>"transparent"</c> to reset to default.
        /// </summary>
        /// <param name="color">A hex color string (e.g. "#RRGGBB" or "#AARRGGBB"), or <c>null</c>/<c>"transparent"</c> to reset.</param>
        /// <returns>The builder instance for chaining.</returns>
        public IInfiniFrameWindowBuilder SetBackgroundColor(string? color) {
            builder.Features.Decorations.SetBackgroundColor(color);
            return builder;
        }
        /// <summary>
        ///     Sets the window title.
        /// </summary>
        /// <param name="title">The title to set.</param>
        /// <returns>The builder instance for chaining.</returns>
        public IInfiniFrameWindowBuilder SetTitle(string? title) {
            builder.Features.Decorations.SetTitle(title);
            return builder;
        }
        /// <summary>
        ///     Sets the window icon from a file path.
        /// </summary>
        /// <param name="iconFilePath">The path to the icon file.</param>
        /// <returns>The builder instance for chaining.</returns>
        public IInfiniFrameWindowBuilder SetIconFile(string iconFilePath) {
            builder.Features.Decorations.SetIconFile(iconFilePath);
            return builder;
        }
        /// <summary>
        ///     Sets whether the Linux window title length should be limited.
        /// </summary>
        /// <param name="enabled">Whether the title length should be limited.</param>
        /// <returns>The builder instance for chaining.</returns>
        public IInfiniFrameWindowBuilder SetLimitLinuxWindowTitleLength(bool enabled = true) {
            builder.Features.Decorations.SetLimitLinuxWindowTitleLength(enabled);
            return builder;
        }
    }

}
