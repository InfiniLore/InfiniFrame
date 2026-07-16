// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowBuilderFeaturePageNavigation : IInfiniFrameWindowBuilderFeaturePageNavigation {
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeaturePageNavigation.StartString"/>
    public string? StartString { get; private set; }
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeaturePageNavigation.StartUrl"/>
    public string? StartUrl { get; private set; }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeaturePageNavigation.SetStartPageContent"/>
    public void SetStartPageContent(string? startString) {
        StartString = startString;
    }
    
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeaturePageNavigation.SetStartPageUrl"/>
    public void SetStartPageUrl(string? startUrl) {
        StartUrl = startUrl;
    }
    
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeaturePageNavigation.SetUrl"/>
    public void SetUrl(Uri? startUrl) {
        StartUrl = startUrl?.ToString();
    }

    public void ApplyToNativeParameters(ref InfiniFrameNativeParameters parameters) {
        parameters.StartUrl = StartUrl;
        parameters.StartString = StartString;
    }
}