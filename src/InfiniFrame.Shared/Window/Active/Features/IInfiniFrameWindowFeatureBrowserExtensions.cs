// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class IInfiniFrameWindowFeatureBrowserExtensions {
    public static IInfiniFrameWindow EnableContextMenu(this IInfiniFrameWindow window, bool enabled = true) {
        window.Features.Browser.EnableContextMenu(enabled);
        return window;
    }
    
    public static IInfiniFrameWindow EnableMediaAutoplay(this IInfiniFrameWindow window, bool enabled = true) {
        window.Features.Browser.EnableMediaAutoplay(enabled);
        return window;
    }

    public static IInfiniFrameWindow SetUserAgent(this IInfiniFrameWindow window, string? userAgent) {
        window.Features.Browser.SetUserAgent(userAgent);
        return window;
    }
    
    public static IInfiniFrameWindow Win32SetWebView2Path(this IInfiniFrameWindow window, string data) {
        window.Features.Browser.Win32SetWebView2Path(data);
        return window;
    }
    
    public static IInfiniFrameWindow ClearBrowserAutoFill(this IInfiniFrameWindow window) {
        window.Features.Browser.ClearBrowserAutoFill();
        return window;
    }
}
