// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class IInfiniFrameWindowBuilderFeaturePageNavigationExtensions {
    public static IInfiniFrameWindowBuilder SetStartPageContent(this IInfiniFrameWindowBuilder builder, string? content) {
        builder.Features.PageNavigation.SetStartPageContent(content);
        return builder;
    }
    
    public static IInfiniFrameWindowBuilder SetStartPageUrl(this IInfiniFrameWindowBuilder builder, string? url) {
        builder.Features.PageNavigation.SetStartPageUrl(url);
        return builder;
    }
    
    public static IInfiniFrameWindowBuilder SetUrl(this IInfiniFrameWindowBuilder builder, Uri? startUrl) {
        builder.Features.PageNavigation.SetUrl(startUrl);
        return builder;
    }
}