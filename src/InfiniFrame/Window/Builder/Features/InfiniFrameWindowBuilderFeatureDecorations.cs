// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Utilities;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowBuilderFeatureDecorations : IInfiniFrameWindowBuilderFeatureDecorations {
    public bool IsChromeless { get; private set; }
    public bool IsTransparent { get; private set; }
    public string? Title { get; private set; } = TitleStringUtility.DefaultTitle;
    public string? IconFilePath { get; private set; }

    public bool LimitLinuxWindowTitleLength { get; private set; } = false;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public void SetChromeless(bool enabled) {
        IsChromeless = enabled;
    }

    public void SetTransparent(bool enabled) {
        IsTransparent = enabled;
    }
    
    public void SetTitle(string? title) {
        Title = TitleStringUtility.Validate(title, LimitLinuxWindowTitleLength);
    }
    
    public void SetIconFile(string iconFilePath) {
        IconFilePath = iconFilePath;
    }
    
    public void SetLimitLinuxWindowTitleLength(bool enabled) {
        LimitLinuxWindowTitleLength = enabled;
    }
}
