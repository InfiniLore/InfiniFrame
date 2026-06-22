// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;
using InfiniFrame.Utilities;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowBuilderFeatureDecorations : IInfiniFrameWindowBuilderFeatureDecorations {
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureDecorations.IsChromeless"/>
    public bool IsChromeless { get; private set; }

    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureDecorations.IsTransparent"/>
    public bool IsTransparent { get; private set; }

    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureDecorations.Title"/>
    public string? Title { get; private set; } = TitleStringUtility.DefaultTitle;

    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureDecorations.IconFilePath"/>
    public string? IconFilePath { get; private set; }

    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureDecorations.LimitLinuxWindowTitleLength"/>
    public bool LimitLinuxWindowTitleLength { get; private set; }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureDecorations.SetChromeless"/>
    public void SetChromeless(bool enabled) {
        IsChromeless = enabled;
    }

    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureDecorations.SetTransparent"/>
    public void SetTransparent(bool enabled) {
        IsTransparent = enabled;
    }

    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureDecorations.SetTitle"/>
    public void SetTitle(string? title) {
        Title = TitleStringUtility.Validate(title, LimitLinuxWindowTitleLength);
    }

    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureDecorations.SetIconFile"/>
    public void SetIconFile(string iconFilePath) {
        IconFilePath = iconFilePath;
    }

    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureDecorations.SetLimitLinuxWindowTitleLength"/>
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
        // parameters.LimitLinuxWindowTitleLength = LimitLinuxWindowTitleLength; // Not a C++ parameter.
    }
}
