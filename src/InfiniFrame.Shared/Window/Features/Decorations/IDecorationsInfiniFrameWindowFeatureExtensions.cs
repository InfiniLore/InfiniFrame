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
    /// <summary>
    ///     Enables or disables window transparency.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="enabled">Whether transparency should be enabled.</param>
    /// <returns>The window instance for chaining.</returns>
    public static IInfiniFrameWindow SetTransparent(this IInfiniFrameWindow window, bool enabled = true) {
        window.Features.Decorations.SetTransparent(enabled);
        return window;
    }

    /// <summary>
    ///     Sets the window background color. Pass <c>null</c> or <c>"transparent"</c> to reset to default.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="color">A hex color string (e.g. "#RRGGBB" or "#AARRGGBB"), or <c>null</c>/<c>"transparent"</c> to reset.</param>
    /// <returns>The window instance for chaining.</returns>
    public static IInfiniFrameWindow SetBackgroundColor(this IInfiniFrameWindow window, string? color) {
        window.Features.Decorations.SetBackgroundColor(color);
        return window;
    }

    /// <summary>
    ///     Sets the window title.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="title">The title to set.</param>
    /// <returns>The window instance for chaining.</returns>
    public static IInfiniFrameWindow SetTitle(this IInfiniFrameWindow window, string? title) {
        window.Features.Decorations.SetTitle(title);
        return window;
    }

    /// <summary>
    ///     Sets the window icon from a file path.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="iconFilePath">The path to the icon file.</param>
    /// <returns>The window instance for chaining.</returns>
    public static IInfiniFrameWindow SetIconFile(this IInfiniFrameWindow window, string iconFilePath) {
        window.Features.Decorations.SetIconFile(iconFilePath);
        return window;
    }

    /// <summary>
    ///     Sets whether the Linux window title length should be limited.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="enabled">Whether the title length should be limited.</param>
    /// <returns>The window instance for chaining.</returns>
    public static IInfiniFrameWindow SetLimitLinuxWindowTitleLength(this IInfiniFrameWindow window, bool enabled = true) {
        window.Features.Decorations.SetLimitLinuxWindowTitleLength(enabled);
        return window;
    }
}
