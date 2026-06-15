// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowBuilderFeatureState : IInfiniFrameWindowBuilderFeature {
    bool StartFullScreen { get; }
    bool StartMaximized { get; }
    bool StartMinimized { get; }
    bool StartTopMost { get; }
    int ZoomFactor { get; }
    bool IsZoomEnabled { get; }
    
    void SetFullScreen(bool fullScreen);
    void SetMaximized(bool maximized);
    void SetMinimized(bool minimized);
    void SetTopMost(bool topMost);
    void SetZoomFactor(int zoom);
    void EnableZoom(bool zoomEnabled);
}
