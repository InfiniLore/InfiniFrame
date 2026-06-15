// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowBuilderFeatureDecorations : IInfiniFrameWindowBuilderFeature {
    bool IsChromeless { get; }
    bool IsTransparent { get; }
    string? Title { get; }
    string? IconFilePath { get; }
    bool LimitLinuxWindowTitleLength { get; }

    void SetChromeless(bool enabled);
    void SetTransparent(bool enabled);
    void SetTitle(string? title);
    void SetIconFile(string iconFilePath);
    void SetLimitLinuxWindowTitleLength(bool enabled);
}
