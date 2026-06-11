// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowBuilderFeaturePageNavigation : IInfiniFrameWindowBuilderFeaturePageNavigation {
    public string? StartString { get; private set; }
    public string? StartUrl { get; private set; }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public void SetString(string? startString) {
        StartString = startString;
    }
    
    public void SetUrl(string? startUrl) {
        StartUrl = startUrl;
    }
    
    public void SetUrl(Uri? startUrl) {
        StartUrl = startUrl?.ToString();
    }

    public void ApplyToNativeParameters(ref InfiniFrameNativeParameters parameters) {
        parameters.StartUrl = StartUrl;
        parameters.StartString = StartString;
    }
}