// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class InfiniFrameWindowBuilderFeatureSizeExtensions {
    public static IInfiniFrameWindowBuilder SetStartSize(this IInfiniFrameWindowBuilder builder, int width, int height) {
        builder.Features.Size.SetStartSize(width, height);
        return builder;
    }
    
    public static IInfiniFrameWindowBuilder SetStartSize(this IInfiniFrameWindowBuilder builder, Size size) {
        builder.Features.Size.SetStartSize(size);
        return builder;
    }
    
    public static IInfiniFrameWindowBuilder SetStartHeight(this IInfiniFrameWindowBuilder builder, int height) {
        builder.Features.Size.SetStartHeight(height);
        return builder;
    }
    
    public static IInfiniFrameWindowBuilder SetStartWidth(this IInfiniFrameWindowBuilder builder, int width) {
        builder.Features.Size.SetStartWidth(width);
        return builder;
    }
    
    public static IInfiniFrameWindowBuilder SetMaxSize(this IInfiniFrameWindowBuilder builder, int maxWidth, int maxHeight) {
        builder.Features.Size.SetMaxSize(maxWidth, maxHeight);
        return builder;
    }
    
    public static IInfiniFrameWindowBuilder SetMaxSize(this IInfiniFrameWindowBuilder builder, Size size) {
        builder.Features.Size.SetMaxSize(size);
        return builder;
    }
    
    public static IInfiniFrameWindowBuilder SetMaxHeight(this IInfiniFrameWindowBuilder builder, int maxHeight) {
        builder.Features.Size.SetMaxHeight(maxHeight);
        return builder;
    }
    
    public static IInfiniFrameWindowBuilder SetMaxWidth(this IInfiniFrameWindowBuilder builder, int maxWidth) {
        builder.Features.Size.SetMaxWidth(maxWidth);
        return builder;
    }
    
    public static IInfiniFrameWindowBuilder SetMinSize(this IInfiniFrameWindowBuilder builder, int minWidth, int minHeight) {
        builder.Features.Size.SetMinSize(minWidth, minHeight);
        return builder;
    }
    
    public static IInfiniFrameWindowBuilder SetMinSize(this IInfiniFrameWindowBuilder builder, Size size) {
        builder.Features.Size.SetMinSize(size);
        return builder;
    }
    
    public static IInfiniFrameWindowBuilder SetMinHeight(this IInfiniFrameWindowBuilder builder, int minHeight) {
        builder.Features.Size.SetMinHeight(minHeight);
        return builder;
    }
    
    public static IInfiniFrameWindowBuilder SetMinWidth(this IInfiniFrameWindowBuilder builder, int minWidth) {
        builder.Features.Size.SetMinWidth(minWidth);
        return builder;
    }
    
    public static IInfiniFrameWindowBuilder UseOsDefaultSize(this IInfiniFrameWindowBuilder builder, bool enabled) {
        builder.Features.Size.UseOsDefaultSize(enabled);
        return builder;
    }
    
    public static IInfiniFrameWindowBuilder SetResizable(this IInfiniFrameWindowBuilder builder, bool enabled) {
        builder.Features.Size.SetResizable(enabled);
        return builder;
    }
}
