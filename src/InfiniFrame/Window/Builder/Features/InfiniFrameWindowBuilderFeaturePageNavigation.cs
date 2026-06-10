// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
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
    public void SetStartString(string? startString) {
        StartString = startString;
    }
    
    public void SetStartUrl(string? startUrl) {
        StartUrl = startUrl;
    }
    
    public void SetStartUrl(Uri? startUrl) {
        StartUrl = startUrl?.ToString();
    }
    
}