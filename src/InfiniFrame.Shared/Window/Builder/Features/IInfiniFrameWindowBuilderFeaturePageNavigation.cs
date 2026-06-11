// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowBuilderFeaturePageNavigation : IInfiniFrameWindowBuilderFeature{
    string? StartString { get; }
    string? StartUrl { get; }
    
    void SetString(string? startString);
    void SetUrl(string? startUrl);
    void SetUrl(Uri? startUrl);
}
