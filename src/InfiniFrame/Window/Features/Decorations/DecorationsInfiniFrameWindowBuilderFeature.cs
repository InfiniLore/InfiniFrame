// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;
using InfiniFrame.Utilities;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class DecorationsInfiniFrameWindowBuilderFeature : IDecorationsInfiniFrameWindowBuilderFeature {
    /// <inheritdoc cref="IDecorationsInfiniFrameWindowBuilderFeature.IsChromeless" />
    public bool IsChromeless { get; private set; }

    /// <inheritdoc cref="IDecorationsInfiniFrameWindowBuilderFeature.IsTransparent" />
    public bool IsTransparent { get; private set; }

    /// <inheritdoc cref="IDecorationsInfiniFrameWindowBuilderFeature.BackgroundColor" />
    public string? BackgroundColor { get; private set; }

    /// <inheritdoc cref="IDecorationsInfiniFrameWindowBuilderFeature.Title" />
    public string? Title { get; private set; } = TitleStringUtility.DefaultTitle;

    /// <inheritdoc cref="IDecorationsInfiniFrameWindowBuilderFeature.IconFilePath" />
    public string? IconFilePath { get; private set; }

    /// <inheritdoc cref="IDecorationsInfiniFrameWindowBuilderFeature.WindowsAppUserModelId" />
    public string? WindowsAppUserModelId { get; private set; }

    /// <inheritdoc cref="IDecorationsInfiniFrameWindowBuilderFeature.LimitLinuxWindowTitleLength" />
    public bool LimitLinuxWindowTitleLength { get; private set; }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="IDecorationsInfiniFrameWindowBuilderFeature.SetChromeless" />
    public void SetChromeless(bool enabled) {
        IsChromeless = enabled;
    }

    /// <inheritdoc cref="IDecorationsInfiniFrameWindowBuilderFeature.SetTransparent" />
    public void SetTransparent(bool enabled) {
        IsTransparent = enabled;
    }

    /// <inheritdoc cref="IDecorationsInfiniFrameWindowBuilderFeature.SetBackgroundColor" />
    public void SetBackgroundColor(string? color) {
        BackgroundColor = color;
    }

    /// <inheritdoc cref="IDecorationsInfiniFrameWindowBuilderFeature.SetTitle" />
    public void SetTitle(string? title) {
        Title = TitleStringUtility.Validate(title, LimitLinuxWindowTitleLength);
    }

    /// <inheritdoc cref="IDecorationsInfiniFrameWindowBuilderFeature.SetIconFile" />
    public void SetIconFile(string iconFilePath) {
        IconFilePath = iconFilePath;
    }

    /// <inheritdoc cref="IDecorationsInfiniFrameWindowBuilderFeature.SetWindowsAppUserModelId" />
    public void SetWindowsAppUserModelId(string? appUserModelId) {
        WindowsAppUserModelId = appUserModelId;
    }

    /// <inheritdoc cref="IDecorationsInfiniFrameWindowBuilderFeature.SetLimitLinuxWindowTitleLength" />
    public void SetLimitLinuxWindowTitleLength(bool enabled) {
        LimitLinuxWindowTitleLength = enabled;
    }

    public void ApplyToNativeParameters(ref InfiniFrameNativeParameters parameters) {
        parameters.Chromeless = IsChromeless;
        parameters.Transparent = IsTransparent;
        parameters.Title = Title;
        parameters.WindowIconFile = IconFileUtility.TryResolveIconFilePath(IconFilePath, out string? resolvedIconFilePath)
            ? resolvedIconFilePath
            : null;
        parameters.WindowsAppUserModelId = WindowsAppUserModelId;

        ColorUtility.ParseBackgroundColor(
            BackgroundColor, out byte r, out byte g, out byte b, out byte a);
        parameters.BackgroundColorR = r;
        parameters.BackgroundColorG = g;
        parameters.BackgroundColorB = b;
        parameters.BackgroundColorA = a;

        // parameters.LimitLinuxWindowTitleLength = LimitLinuxWindowTitleLength; // Not a C++ parameter.
    }
}
