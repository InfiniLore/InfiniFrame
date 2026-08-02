// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IPageNavigationInfiniFrameWindowFeature {
    /// <summary>
    ///     Loads the specified URI in the window.
    /// </summary>
    /// <param name="uri">The URI to load.</param>
    void Load(Uri uri);

    Task<NavigationResult> LoadAsync(Uri uri, CancellationToken ct = default);

    /// <summary>
    ///     Loads the content at the specified path in the window.
    /// </summary>
    /// <param name="path">The file path or URL string to load.</param>
    void Load(string path);

    /// <summary>
    ///     Attempts to load the specified URI in the window.
    /// </summary>
    /// <param name="uri">The URI to load.</param>
    /// <returns>true if the URI was loaded successfully; otherwise, false.</returns>
    bool TryLoadUri(Uri uri);

    /// <summary>
    ///     Attempts to load the content at the specified path in the window.
    /// </summary>
    /// <param name="path">The file path or URL string to load.</param>
    /// <returns>true if the path was loaded successfully; otherwise, false.</returns>
    bool TryLoadPath(string path);

    /// <summary>
    ///     Loads raw HTML content as a string in the window.
    /// </summary>
    /// <param name="content">The raw HTML content to load.</param>
    void LoadRawString(string content);

    Task<NavigationResult> LoadRawStringAsync(string content, CancellationToken ct = default);

    /// <summary>
    ///     Gets the current page URL as a string, or null if no URL is available (e.g. after LoadRawString).
    /// </summary>
    string? CurrentUrl { get; }

    /// <summary>
    ///     Convenience property that parses <see cref="CurrentUrl"/> into a <see cref="Uri"/>.
    /// </summary>
    Uri? CurrentUri { get; }
}