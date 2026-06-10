// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowFeatureState {
    bool IsFullScreen { get; }
    bool IsMaximized { get; }
    bool IsMinimized { get; }
    bool IsTopMost { get; }
    bool IsFocused { get; }
    int ZoomFactor { get; }
    bool IsZoomEnabled { get; }
    Rectangle CachedPreFullScreenBounds { get; set; }
    Rectangle CachedPreMaximizedBounds { get; set; }
    
    IInfiniFrameWindow SetMaximized(bool maximized);
    IInfiniFrameWindow ToggleMaximized();
    IInfiniFrameWindow SetMinimized(bool minimized);
    IInfiniFrameWindow SetFullScreen(bool fullScreen);
    IInfiniFrameWindow SetFocused();
    IInfiniFrameWindow SetZoom(int zoom);
    IInfiniFrameWindow SetZoomEnabled(bool zoomEnabled);
    IInfiniFrameWindow SetTopMost(bool topMost);
}
