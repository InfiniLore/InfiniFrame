// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class IInfiniFrameWindowFeatureStateExtension {
    public static IInfiniFrameWindow SetMaximized(this IInfiniFrameWindow window, bool maximized = true) {
        window.Features.State.SetMaximized(maximized);
        return window;
    }
    
    public static IInfiniFrameWindow ToggleMaximized(this IInfiniFrameWindow window) {
        window.Features.State.ToggleMaximized();
        return window;
    }
    
    public static IInfiniFrameWindow SetMinimized(this IInfiniFrameWindow window, bool minimized = true) {
        window.Features.State.SetMinimized(minimized);
        return window;
    }
    
    public static IInfiniFrameWindow SetFullScreen(this IInfiniFrameWindow window, bool fullScreen = true) {
        window.Features.State.SetFullScreen(fullScreen);
        return window;
    }
    
    public static IInfiniFrameWindow SetFocused(this IInfiniFrameWindow window) {
        window.Features.State.SetFocused();
        return window;
    }
    
    public static IInfiniFrameWindow SetZoomFactor(this IInfiniFrameWindow window, int zoom) {
        window.Features.State.SetZoomFactor(zoom);
        return window;
    }
    
    public static IInfiniFrameWindow EnableZoom(this IInfiniFrameWindow window, bool zoomEnabled = true) {
        window.Features.State.EnableZoom(zoomEnabled);
        return window;
    }
    
    public static IInfiniFrameWindow SetTopMost(this IInfiniFrameWindow window, bool topMost = true) {
        window.Features.State.SetTopMost(topMost);
        return window;
    }
}
