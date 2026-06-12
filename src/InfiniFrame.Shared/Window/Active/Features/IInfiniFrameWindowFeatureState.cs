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

    void SetMaximized(bool maximized = true);
    void ToggleMaximized();
    void SetMinimized(bool minimized = true);
    void SetFullScreen(bool fullScreen = true);
    void SetFocused();
    void SetZoomFactor(int zoom);
    void SetZoomEnabled(bool zoomEnabled = true);
    void SetTopMost(bool topMost = true);
}
