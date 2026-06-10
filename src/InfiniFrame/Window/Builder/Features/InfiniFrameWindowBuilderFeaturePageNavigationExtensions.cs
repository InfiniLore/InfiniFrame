// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class InfiniFrameWindowBuilderFeaturePageNavigationExtensions {
    public static IInfiniFrameWindowBuilder SetStartString(this IInfiniFrameWindowBuilder builder, string? startString) {
        builder.Features.PageNavigation.SetStartString(startString);
        return builder;
    }
    
    public static IInfiniFrameWindowBuilder SetStartUrl(this IInfiniFrameWindowBuilder builder, string? startUrl) {
        builder.Features.PageNavigation.SetStartUrl(startUrl);
        return builder;
    }
}