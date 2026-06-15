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
    bool LimitLinuxWindowTitleLength { get; }

    void SetTransparent(bool enabled = true);
    void SetTitle(string? title);
    void SetIconFile(string iconFilePath);
    void SetLimitLinuxWindowTitleLength(bool enabled = true);
}
