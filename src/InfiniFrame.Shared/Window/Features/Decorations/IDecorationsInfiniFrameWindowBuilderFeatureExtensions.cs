// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class IDecorationsInfiniFrameWindowBuilderFeatureExtensions {
    /// <summary>
    ///     Sets whether the window should be chromeless.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="enabled">Whether the window should be chromeless.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IInfiniFrameWindowBuilder SetChromeless(this IInfiniFrameWindowBuilder builder, bool enabled = true) {
        builder.Features.Decorations.SetChromeless(enabled);
        return builder;
    }

    /// <summary>
    ///     Sets whether the window should be transparent.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="enabled">Whether the window should be transparent.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IInfiniFrameWindowBuilder SetTransparent(this IInfiniFrameWindowBuilder builder, bool enabled = true) {
        builder.Features.Decorations.SetTransparent(enabled);
        return builder;
    }

    /// <summary>
    ///     Sets the window background color. Pass <c>null</c> or <c>"transparent"</c> to reset to default.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="color">A hex color string (e.g. "#RRGGBB" or "#AARRGGBB"), or <c>null</c>/<c>"transparent"</c> to reset.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IInfiniFrameWindowBuilder SetBackgroundColor(this IInfiniFrameWindowBuilder builder, string? color) {
        builder.Features.Decorations.SetBackgroundColor(color);
        return builder;
    }

    /// <summary>
    ///     Sets the window title.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="title">The title to set.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IInfiniFrameWindowBuilder SetTitle(this IInfiniFrameWindowBuilder builder, string? title) {
        builder.Features.Decorations.SetTitle(title);
        return builder;
    }

    /// <summary>
    ///     Sets the window icon from a file path.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="iconFilePath">The path to the icon file.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IInfiniFrameWindowBuilder SetIconFile(this IInfiniFrameWindowBuilder builder, string iconFilePath) {
        builder.Features.Decorations.SetIconFile(iconFilePath);
        return builder;
    }

    /// <summary>
    ///     Sets the explicit Windows application user model ID used for taskbar grouping and application identity.
    ///     All windows in a process should use the same ID.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="appUserModelId">The application user model ID, or <c>null</c> to use Windows' default identity.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IInfiniFrameWindowBuilder SetWindowsAppUserModelId(
        this IInfiniFrameWindowBuilder builder,
        string? appUserModelId
    ) {
        builder.Features.Decorations.SetWindowsAppUserModelId(appUserModelId);
        return builder;
    }

    /// <summary>
    ///     Sets whether the Linux window title length should be limited.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="enabled">Whether the title length should be limited.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IInfiniFrameWindowBuilder SetLimitLinuxWindowTitleLength(this IInfiniFrameWindowBuilder builder, bool enabled = true) {
        builder.Features.Decorations.SetLimitLinuxWindowTitleLength(enabled);
        return builder;
    }
}
