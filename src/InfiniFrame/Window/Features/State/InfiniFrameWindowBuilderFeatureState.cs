// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowBuilderFeatureState : IInfiniFrameWindowBuilderFeatureState {
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureState.StartFullScreen"/>
    public bool StartFullScreen { get; private set; }
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureState.StartMaximized"/>
    public bool StartMaximized { get; private set; }
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureState.StartMinimized"/>
    public bool StartMinimized { get; private set; }
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureState.StartTopMost"/>
    public bool StartTopMost { get; private set; }
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureState.ZoomFactor"/>
    public int ZoomFactor { get; private set; } = 100;
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureState.IsZoomEnabled"/>
    public bool IsZoomEnabled { get; private set; } = true;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureState.SetFullScreen"/>
    public void SetFullScreen(bool fullScreen) {
        StartFullScreen = fullScreen;
        StartMaximized = false;
        StartMinimized = false;
    }
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureState.SetMaximized"/>
    public void SetMaximized(bool maximized) {
        StartFullScreen = false;
        StartMaximized = maximized;
        StartMinimized = false;
    }
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureState.SetMinimized"/>
    public void SetMinimized(bool minimized) {
        StartFullScreen = false;
        StartMaximized = false;
        StartMinimized = minimized;
    }
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureState.SetTopMost"/>
    public void SetTopMost(bool topMost) {
        StartTopMost = topMost;
    }
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureState.SetZoomFactor"/>
    public void SetZoomFactor(int zoom) {
        ZoomFactor = zoom;
    }
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureState.EnableZoom"/>
    public void EnableZoom(bool zoomEnabled) {
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