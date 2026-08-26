// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class StateInfiniFrameWindowBuilderFeature : IStateInfiniFrameWindowBuilderFeature {
    /// <inheritdoc cref="IStateInfiniFrameWindowBuilderFeature.StartFullScreen" />
    public bool StartFullScreen { get; private set; }
    /// <inheritdoc cref="IStateInfiniFrameWindowBuilderFeature.StartMaximized" />
    public bool StartMaximized { get; private set; }
    /// <inheritdoc cref="IStateInfiniFrameWindowBuilderFeature.StartMinimized" />
    public bool StartMinimized { get; private set; }
    /// <inheritdoc cref="IStateInfiniFrameWindowBuilderFeature.StartTopMost" />
    public bool StartTopMost { get; private set; }
    /// <inheritdoc cref="IStateInfiniFrameWindowBuilderFeature.ZoomFactor" />
    public int ZoomFactor { get; private set; } = 100;
    /// <inheritdoc cref="IStateInfiniFrameWindowBuilderFeature.IsZoomEnabled" />
    public bool IsZoomEnabled { get; private set; } = true;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="IStateInfiniFrameWindowBuilderFeature.SetFullScreen" />
    public void SetFullScreen(bool fullScreen) {
        StartFullScreen = fullScreen;
        StartMaximized = false;
        StartMinimized = false;
    }
    /// <inheritdoc cref="IStateInfiniFrameWindowBuilderFeature.SetMaximized" />
    public void SetMaximized(bool maximized) {
        StartFullScreen = false;
        StartMaximized = maximized;
        StartMinimized = false;
    }
    /// <inheritdoc cref="IStateInfiniFrameWindowBuilderFeature.SetMinimized" />
    public void SetMinimized(bool minimized) {
        StartFullScreen = false;
        StartMaximized = false;
        StartMinimized = minimized;
    }
    /// <inheritdoc cref="IStateInfiniFrameWindowBuilderFeature.SetTopMost" />
    public void SetTopMost(bool topMost) {
        StartTopMost = topMost;
    }
    /// <inheritdoc cref="IStateInfiniFrameWindowBuilderFeature.SetZoomFactor" />
    public void SetZoomFactor(int zoom) {
        ZoomFactor = zoom;
    }
    /// <inheritdoc cref="IStateInfiniFrameWindowBuilderFeature.EnableZoom" />
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
