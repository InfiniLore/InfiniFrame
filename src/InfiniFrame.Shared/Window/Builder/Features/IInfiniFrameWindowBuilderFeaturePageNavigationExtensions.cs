// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class IInfiniFrameWindowBuilderFeaturePageNavigationExtensions {
    public static IInfiniFrameWindowBuilder SetString(this IInfiniFrameWindowBuilder builder, string? startString) {
        builder.Features.PageNavigation.SetString(startString);
        return builder;
    }
    
    public static IInfiniFrameWindowBuilder SetUrl(this IInfiniFrameWindowBuilder builder, string? startUrl) {
        builder.Features.PageNavigation.SetUrl(startUrl);
        return builder;
    }
    
    public static IInfiniFrameWindowBuilder SetUrl(this IInfiniFrameWindowBuilder builder, Uri? startUrl) {
        builder.Features.PageNavigation.SetUrl(startUrl);
        return builder;
    }
}