// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
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