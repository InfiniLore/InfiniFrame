// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Fluent extension methods for <see cref="IPageNavigationInfiniFrameWindowFeature" /> on <see cref="IInfiniFrameWindow" />.
/// </summary>
public static class IPageNavigationInfiniFrameWindowFeatureExtensions {
    /// <param name="window">The window instance.</param>
    extension(IInfiniFrameWindow window) {
        /// <summary>
        ///     Loads the specified URI in the window and returns the window for chaining.
        /// </summary>
        /// <param name="uri">The URI to load.</param>
        /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
        public IInfiniFrameWindow Load(Uri uri) {
            window.Features.PageNavigation.Load(uri);
            return window;
        }
        /// <summary>
        ///     Loads the specified URI in the window and returns the window for chaining.
        /// </summary>
        /// <param name="uri">The URI to load.</param>
        /// <param name="ct">The cancellation token to use.</param>
        public Task<NavigationResult> LoadAsync(
            Uri uri,
            CancellationToken ct = default
        ) => window.Features.PageNavigation.LoadAsync(uri, ct);
        /// <summary>
        ///     Loads the content at the specified path in the window.
        /// </summary>
        /// <param name="path">The file path or URL string to load.</param>
        /// <param name="ct">The cancellation token to use.</param>
        public Task<NavigationResult> LoadAsync(
            string path,
            CancellationToken ct = default
        ) => window.Features.PageNavigation.LoadAsync(path, ct);
        /// <summary>
        ///     Loads the content at the specified path in the window and returns the window for chaining.
        /// </summary>
        /// <param name="path">The file path or URL string to load.</param>
        /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
        public IInfiniFrameWindow Load(string path) {
            window.Features.PageNavigation.Load(path);
            return window;
        }
        /// <summary>
        ///     Loads raw HTML content as a string in the window and returns the window for chaining.
        /// </summary>
        /// <param name="content">The raw HTML content to load.</param>
        /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
        public IInfiniFrameWindow LoadRawString(string content) {
            window.Features.PageNavigation.LoadRawString(content);
            return window;
        }
        /// <summary>
        ///     Loads raw HTML content as a string in the window and returns the window for chaining.
        /// </summary>
        /// <param name="content">The raw HTML content to load.</param>
        /// <param name="ct">The cancellation token.</param>
        public Task<NavigationResult> LoadRawStringAsync(
            string content,
            CancellationToken ct = default
        ) => window.Features.PageNavigation.LoadRawStringAsync(content, ct);
        /// <summary>
        ///     Gets the current page URL as a string, or null if no URL is available.
        /// </summary>
        /// <returns>The current page URL, or null.</returns>
        public string? GetCurrentUrl()
            => window.Features.PageNavigation.GetCurrentUrl();
        /// <summary>
        ///     Gets the current page URL as a Uri, or null if no URL is available.
        /// </summary>
        /// <returns>The current page URL, or null.</returns>
        public Uri? GetCurrentUri()
            => window.Features.PageNavigation.GetCurrentUri();
    }

}
