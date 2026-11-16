// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Fluent extension methods for <see cref="IPageNavigationInfiniFrameWindowBuilderFeature"/> on <see cref="IInfiniFrameWindowBuilder"/>.
/// </summary>
public static class IPageNavigationInfiniFrameWindowBuilderFeatureExtensions {
    /// <param name="builder">The window builder instance.</param>
    extension(IInfiniFrameWindowBuilder builder) {
        /// <summary>
        ///     Sets the content to display as the start page and returns the builder for chaining.
        /// </summary>
        /// <param name="content">The raw HTML content for the start page.</param>
        /// <returns>The <see cref="IInfiniFrameWindowBuilder" /> for method chaining.</returns>
        public IInfiniFrameWindowBuilder SetStartPageContent(string? content) {
            builder.Features.PageNavigation.SetStartPageContent(content);
            return builder;
        }
        /// <summary>
        ///     Sets the URL to navigate to as the start page and returns the builder for chaining.
        /// </summary>
        /// <param name="url">The start page URL as a string.</param>
        /// <returns>The <see cref="IInfiniFrameWindowBuilder" /> for method chaining.</returns>
        public IInfiniFrameWindowBuilder SetStartPageUrl(string? url) {
            builder.Features.PageNavigation.SetStartPageUrl(url);
            return builder;
        }
        /// <summary>
        ///     Sets the URI to navigate to when the window is created and returns the builder for chaining.
        /// </summary>
        /// <param name="startUrl">The start page URI.</param>
        /// <returns>The <see cref="IInfiniFrameWindowBuilder" /> for method chaining.</returns>
        public IInfiniFrameWindowBuilder SetUrl(Uri? startUrl) {
            builder.Features.PageNavigation.SetUrl(startUrl);
            return builder;
        }
    }

}
