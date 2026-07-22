// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text.Json;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal sealed class StateWebMessageDispatcher : WindowFeatureWebMessageDispatcherBase<IInfiniFrameWindowFeatureState> {
    public override string FeatureName => "state";
    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    protected override IInfiniFrameWindowFeatureState SelectFeature(IInfiniFrameWindowFeatures features) 
        => features.State;

    protected override object Get(IInfiniFrameWindowFeatureState feature, string command, JsonElement? args) => command switch {
        "isFullScreen" => feature.IsFullScreen,
        "isMaximized" => feature.IsMaximized,
        "isMinimized" => feature.IsMinimized,
        "isTopMost" => feature.IsTopMost,
        "isFocused" => feature.IsFocused,
        "zoomFactor" => feature.ZoomFactor,
        "isZoomEnabled" => feature.IsZoomEnabled,
        "cachedPreFullScreenBounds" => feature.CachedPreFullScreenBounds,
        "cachedPreMaximizedBounds" => feature.CachedPreMaximizedBounds,
        _ => throw Unsupported(command)
    };

    protected override void Post(IInfiniFrameWindowFeatureState feature, string command, JsonElement? args) {
        switch (command) {
            case "setMaximized": feature.SetMaximized(Arg(args, "maximized", true)); return;
            case "toggleMaximized": feature.ToggleMaximized(); return;
            case "setMinimized": feature.SetMinimized(Arg(args, "minimized", true)); return;
            case "setFullScreen": feature.SetFullScreen(Arg(args, "fullScreen", true)); return;
            case "setFocused": feature.SetFocused(); return;
            case "setZoomFactor": feature.SetZoomFactor(Required<int>(args, "zoom")); return;
            case "enableZoom": feature.EnableZoom(Arg(args, "enabled", true)); return;
            case "setTopMost": feature.SetTopMost(Arg(args, "topMost", true)); return;
            default: throw Unsupported(command);
        }
    }
}
