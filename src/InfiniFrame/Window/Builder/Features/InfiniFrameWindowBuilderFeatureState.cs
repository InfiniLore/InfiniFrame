// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowBuilderFeatureState : IInfiniFrameWindowBuilderFeatureState {
    public bool StartFullScreen { get; private set; }
    public bool StartMaximized { get; private set; }
    public bool StartMinimized { get; private set; }
    public bool StartTopMost { get; private set; }
    public int ZoomFactor { get; private set; } = 100;
    public bool IsZoomEnabled { get; private set; } = true;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public void SetFullScreen(bool fullScreen) {
        StartFullScreen = fullScreen;
        StartMaximized = false;
        StartMinimized = false;
    }
    public void SetMaximized(bool maximized) {
        StartFullScreen = false;
        StartMaximized = maximized;
        StartMinimized = false;
    }
    public void SetMinimized(bool minimized) {
        StartFullScreen = false;
        StartMaximized = false;
        StartMinimized = minimized;
    }
    public void SetTopMost(bool topMost) {
        StartTopMost = topMost;
    }
    public void SetZoomFactor(int zoom) {
        ZoomFactor = zoom;
    }
    public void SetZoomEnabled(bool zoomEnabled) {
        IsZoomEnabled = zoomEnabled;
    }
    
    public void ApplyToNativeParameters(ref InfiniFrameNativeParameters parameters) {
        parameters.FullScreen = StartFullScreen;
        parameters.Maximized = StartMaximized;
        parameters.Minimized = StartMinimized;
        parameters.Topmost = StartTopMost;
        parameters.Zoom = ZoomFactor;
        parameters.ZoomEnabled = IsZoomEnabled;
    }
}