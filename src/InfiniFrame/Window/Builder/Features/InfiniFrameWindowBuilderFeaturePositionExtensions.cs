// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class InfiniFrameWindowBuilderFeaturePositionExtensions {
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public static IInfiniFrameWindowBuilder SetStartLocation(this IInfiniFrameWindowBuilder builder, int left, int top) {
        builder.Features.Position.SetStartLocation(left, top);
        return builder;
    }
    
    public static IInfiniFrameWindowBuilder SetStartLocation(this IInfiniFrameWindowBuilder builder, Point location) {
        builder.Features.Position.SetStartLocation(location);
        return builder;
    }
    
    public static IInfiniFrameWindowBuilder SetStartLeft(this IInfiniFrameWindowBuilder builder, int left) {
        builder.Features.Position.SetStartLeft(left);
        return builder;
    }
    
    public static IInfiniFrameWindowBuilder SetStartTop(this IInfiniFrameWindowBuilder builder, int top) {
        builder.Features.Position.SetStartTop(top);
        return builder;
    }
    
    public static IInfiniFrameWindowBuilder UseOsDefaultLocation(this IInfiniFrameWindowBuilder builder, bool enabled) {
        builder.Features.Position.UseOsDefaultLocation(enabled);
        return builder;
    }
    
    public static IInfiniFrameWindowBuilder StartCenteredOnMainMonitor(this IInfiniFrameWindowBuilder builder, bool enabled) {
        builder.Features.Position.StartCenteredOnMainMonitor(enabled);
        return builder;
    }
}