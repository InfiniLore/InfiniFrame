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
    
    public static IInfiniFrameWindow SetZoom(this IInfiniFrameWindow window, int zoom) {
        window.Features.State.SetZoom(zoom);
        return window;
    }
    
    public static IInfiniFrameWindow SetZoomEnabled(this IInfiniFrameWindow window, bool zoomEnabled = true) {
        window.Features.State.SetZoomEnabled(zoomEnabled);
        return window;
    }
    
    public static IInfiniFrameWindow SetTopMost(this IInfiniFrameWindow window, bool topMost = true) {
        window.Features.State.SetTopMost(topMost);
        return window;
    }
}
