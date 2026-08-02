// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IDecorationsInfiniFrameWindowFeature {
    /// <summary>
    ///     Gets whether the window is chromeless (no title bar or borders).
    /// </summary>
    bool IsChromeless { get; }

    /// <summary>
    ///     Gets whether the window supports transparency.
    /// </summary>
    bool IsTransparent { get; }

    /// <summary>
    ///     Gets the current window background color.
    /// </summary>
    string? BackgroundColor { get; }

    /// <summary>
    ///     Gets the current window title.
    /// </summary>
    string? Title { get; }

    /// <summary>
    ///     Gets the file path to the current window icon.
    /// </summary>
    string? IconFilePath { get; }

    /// <summary>
    ///     Gets whether Linux window title length is limited.
    /// </summary>
    bool LimitLinuxWindowTitleLength { get; }

    /// <summary>
    ///     Enables or disables window transparency.
    /// </summary>
    /// <param name="enabled">Whether transparency should be enabled.</param>
    void SetTransparent(bool enabled = true);

    /// <summary>
    ///     Sets the window background color. Pass <c>null</c> or <c>"transparent"</c> to reset to default.
    /// </summary>
    /// <param name="color">A hex color string (e.g. "#RRGGBB" or "#AARRGGBB"), or <c>null</c>/<c>"transparent"</c> to reset.</param>
    void SetBackgroundColor(string? color);

    /// <summary>
    ///     Sets the window title.
    /// </summary>
    /// <param name="title">The title to set.</param>
    void SetTitle(string? title);

    /// <summary>
    ///     Sets the window icon from a file path.
    /// </summary>
    /// <param name="iconFilePath">The path to the icon file.</param>
    void SetIconFile(string iconFilePath);

    /// <summary>
    ///     Sets whether the Linux window title length should be limited.
    /// </summary>
    /// <param name="enabled">Whether the title length should be limited.</param>
    void SetLimitLinuxWindowTitleLength(bool enabled = true);
}