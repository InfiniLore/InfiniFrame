// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class IPageNavigationInfiniFrameWindowFeatureExtensions {
    /// <summary>
    ///     Loads the specified URI in the window and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="uri">The URI to load.</param>
    /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
    public static IInfiniFrameWindow Load(this IInfiniFrameWindow window, Uri uri) {
        window.Features.PageNavigation.Load(uri);
        return window;
    }

    /// <summary>
    ///     Loads the specified URI in the window and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="uri">The URI to load.</param>
    /// <param name="ct">The cancellation token to use.</param>
    public static Task<NavigationResult> LoadAsync(
        this IInfiniFrameWindow window,
        Uri uri,
        CancellationToken ct = default
    ) => window.Features.PageNavigation.LoadAsync(uri, ct);

    /// <summary>
    ///     Loads the content at the specified path in the window.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="path">The file path or URL string to load.</param>
    /// <param name="ct">The cancellation token to use.</param>
    public static Task<NavigationResult> LoadAsync(
        this IInfiniFrameWindow window,
        string path,
        CancellationToken ct = default
    ) => window.Features.PageNavigation.LoadAsync(path, ct);

    /// <summary>
    ///     Loads the content at the specified path in the window and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="path">The file path or URL string to load.</param>
    /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
    public static IInfiniFrameWindow Load(this IInfiniFrameWindow window, string path) {
        window.Features.PageNavigation.Load(path);
        return window;
    }

    /// <summary>
    ///     Loads raw HTML content as a string in the window and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="content">The raw HTML content to load.</param>
    /// <returns>The <see cref="IInfiniFrameWindow" /> for method chaining.</returns>
    public static IInfiniFrameWindow LoadRawString(this IInfiniFrameWindow window, string content) {
        window.Features.PageNavigation.LoadRawString(content);
        return window;
    }

    /// <summary>
    ///     Loads raw HTML content as a string in the window and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="content">The raw HTML content to load.</param>
    /// <param name="ct">The cancellation token.</param>
    public static Task<NavigationResult> LoadRawStringAsync(
        this IInfiniFrameWindow window,
        string content,
        CancellationToken ct = default
    ) => window.Features.PageNavigation.LoadRawStringAsync(content, ct);

    /// <summary>
    ///     Gets the current page URL as a string, or null if no URL is available.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <returns>The current page URL, or null.</returns>
    public static string? GetCurrentUrl(this IInfiniFrameWindow window)
        => window.Features.PageNavigation.GetCurrentUrl();

    /// <summary>
    ///     Gets the current page URL as a Uri, or null if no URL is available.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <returns>The current page URL, or null.</returns>
    public static Uri? GetCurrentUri(this IInfiniFrameWindow window)
        => window.Features.PageNavigation.GetCurrentUri();
}
