// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class InfiniFrameWindowBuilderFeatureDecorationsExtensions {
    public static IInfiniFrameWindowBuilder SetChromeless(this IInfiniFrameWindowBuilder builder, bool enabled = true) {
        builder.Features.Decorations.SetChromeless(enabled);
        return builder;
    }

    public static IInfiniFrameWindowBuilder SetTransparent(this IInfiniFrameWindowBuilder builder, bool enabled = true) {
        builder.Features.Decorations.SetTransparent(enabled);
        return builder;
    }
    
    public static IInfiniFrameWindowBuilder SetTitle(this IInfiniFrameWindowBuilder builder, string? title) {
        builder.Features.Decorations.SetTitle(title);
        return builder;
    }
    
    public static IInfiniFrameWindowBuilder SetIconFile(this IInfiniFrameWindowBuilder builder, string iconFilePath) {
        builder.Features.Decorations.SetIconFile(iconFilePath);
        return builder;
    }
    
    public static IInfiniFrameWindowBuilder SetLimitLinuxWindowTitleLength(this IInfiniFrameWindowBuilder builder, bool enabled = true) {
        builder.Features.Decorations.SetLimitLinuxWindowTitleLength(enabled);
        return builder;
    }
}
