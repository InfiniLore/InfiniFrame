// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowFeatureDecorations {
    bool IsChromeless { get; }
    bool IsTransparent { get; }
    string? Title { get; }
    string? IconFilePath { get; }
    
    IInfiniFrameWindow SetTransparent(bool enabled);
    IInfiniFrameWindow SetTitle(string? title);
    IInfiniFrameWindow SetIconFile(string iconFilePath);
}
