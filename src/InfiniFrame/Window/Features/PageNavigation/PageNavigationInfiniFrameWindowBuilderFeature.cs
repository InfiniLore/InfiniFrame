// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class PageNavigationInfiniFrameWindowBuilderFeature : IPageNavigationInfiniFrameWindowBuilderFeature {
    /// <inheritdoc cref="IPageNavigationInfiniFrameWindowBuilderFeature.StartString" />
    public string? StartString { get; private set; }
    /// <inheritdoc cref="IPageNavigationInfiniFrameWindowBuilderFeature.StartUrl" />
    public string? StartUrl { get; private set; }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="IPageNavigationInfiniFrameWindowBuilderFeature.SetStartPageContent" />
    public void SetStartPageContent(string? startString) {
        StartString = startString;
    }

    /// <inheritdoc cref="IPageNavigationInfiniFrameWindowBuilderFeature.SetStartPageUrl" />
    public void SetStartPageUrl(string? startUrl) {
        StartUrl = startUrl;
    }

    /// <inheritdoc cref="IPageNavigationInfiniFrameWindowBuilderFeature.SetUrl" />
    public void SetUrl(Uri? startUrl) {
        StartUrl = startUrl?.ToString();
    }

    public void ApplyToNativeParameters(ref InfiniFrameNativeParameters parameters) {
        parameters.StartUrl = StartUrl;
        parameters.StartString = StartString;
    }
}
