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
    
    void SetStartPageContent(string? content);
    void SetStartPageUrl(string? startUrl);
    void SetUrl(Uri? startUrl);
}
