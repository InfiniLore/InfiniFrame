// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class IInfiniFrameWindowBuilderFeatureStateExtensions {
    public static IInfiniFrameWindowBuilder SetFullScreen(this IInfiniFrameWindowBuilder builder, bool fullScreen) {
        builder.Features.State.SetFullScreen(fullScreen);
        return builder;
    }
    
    public static IInfiniFrameWindowBuilder SetMaximized(this IInfiniFrameWindowBuilder builder, bool maximized) {
        builder.Features.State.SetMaximized(maximized);
        return builder;
    }
    
    public static IInfiniFrameWindowBuilder SetMinimized(this IInfiniFrameWindowBuilder builder, bool minimized) {
        builder.Features.State.SetMinimized(minimized);
        return builder;
    }
    
    public static IInfiniFrameWindowBuilder SetTopMost(this IInfiniFrameWindowBuilder builder, bool topMost) {
        builder.Features.State.SetTopMost(topMost);
        return builder;
    }
    
    public static IInfiniFrameWindowBuilder SetZoomFactor(this IInfiniFrameWindowBuilder builder, int zoom) {
        builder.Features.State.SetZoomFactor(zoom);
        return builder;
    }
    
    public static IInfiniFrameWindowBuilder SetZoomEnabled(this IInfiniFrameWindowBuilder builder, bool zoomEnabled) {
        builder.Features.State.SetZoomEnabled(zoomEnabled);
        return builder;
    }
}