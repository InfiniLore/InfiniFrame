// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class IInfiniFrameWindowFeatureDecorationsExtensions {
    public static IInfiniFrameWindow SetTransparent(this IInfiniFrameWindow window, bool enabled = true) {
        window.Features.Decorations.SetTransparent(enabled);
        return window;
    }
    
    public static IInfiniFrameWindow SetTitle(this IInfiniFrameWindow window, string? title) {
        window.Features.Decorations.SetTitle(title);
        return window;
    }
    
    public static IInfiniFrameWindow SetIconFile(this IInfiniFrameWindow window, string iconFilePath) {
        window.Features.Decorations.SetIconFile(iconFilePath);
        return window;
    }
    
    public static IInfiniFrameWindow SetLimitLinuxWindowTitleLength(this IInfiniFrameWindow window, bool enabled = true) {
        window.Features.Decorations.SetLimitLinuxWindowTitleLength(enabled);
        return window;
    }
}
