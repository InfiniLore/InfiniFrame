// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class IInfiniFrameWindowBuilderFeaturePositionExtensions {
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public static IInfiniFrameWindowBuilder SetLocation(this IInfiniFrameWindowBuilder builder, int left, int top) {
        builder.Features.Position.SetLocation(left, top);
        return builder;
    }
    
    public static IInfiniFrameWindowBuilder SetLocation(this IInfiniFrameWindowBuilder builder, Point location) {
        builder.Features.Position.SetLocation(location);
        return builder;
    }
    
    public static IInfiniFrameWindowBuilder SetLeft(this IInfiniFrameWindowBuilder builder, int left) {
        builder.Features.Position.SetLeft(left);
        return builder;
    }
    
    public static IInfiniFrameWindowBuilder SetTop(this IInfiniFrameWindowBuilder builder, int top) {
        builder.Features.Position.SetTop(top);
        return builder;
    }
    
    public static IInfiniFrameWindowBuilder UseOsDefaultLocation(this IInfiniFrameWindowBuilder builder, bool enabled = true) {
        builder.Features.Position.UseOsDefaultLocation(enabled);
        return builder;
    }
    
    public static IInfiniFrameWindowBuilder CenteredOnMainMonitor(this IInfiniFrameWindowBuilder builder, bool enabled = true) {
        builder.Features.Position.CenteredOnMainMonitor(enabled);
        return builder;
    }
}