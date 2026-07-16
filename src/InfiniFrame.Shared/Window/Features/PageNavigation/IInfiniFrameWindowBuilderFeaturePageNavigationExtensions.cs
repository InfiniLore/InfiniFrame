// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class IInfiniFrameWindowBuilderFeaturePageNavigationExtensions {
    /// <summary>
    ///     Sets the content to display as the start page and returns the builder for chaining.
    /// </summary>
    /// <param name="builder">The window builder instance.</param>
    /// <param name="content">The raw HTML content for the start page.</param>
    /// <returns>The <see cref="IInfiniFrameWindowBuilder"/> for method chaining.</returns>
    public static IInfiniFrameWindowBuilder SetStartPageContent(this IInfiniFrameWindowBuilder builder, string? content) {
        builder.Features.PageNavigation.SetStartPageContent(content);
        return builder;
    }
    
    /// <summary>
    ///     Sets the URL to navigate to as the start page and returns the builder for chaining.
    /// </summary>
    /// <param name="builder">The window builder instance.</param>
    /// <param name="url">The start page URL as a string.</param>
    /// <returns>The <see cref="IInfiniFrameWindowBuilder"/> for method chaining.</returns>
    public static IInfiniFrameWindowBuilder SetStartPageUrl(this IInfiniFrameWindowBuilder builder, string? url) {
        builder.Features.PageNavigation.SetStartPageUrl(url);
        return builder;
    }
    
    /// <summary>
    ///     Sets the URI to navigate to when the window is created and returns the builder for chaining.
    /// </summary>
    /// <param name="builder">The window builder instance.</param>
    /// <param name="startUrl">The start page URI.</param>
    /// <returns>The <see cref="IInfiniFrameWindowBuilder"/> for method chaining.</returns>
    public static IInfiniFrameWindowBuilder SetUrl(this IInfiniFrameWindowBuilder builder, Uri? startUrl) {
        builder.Features.PageNavigation.SetUrl(startUrl);
        return builder;
    }
}