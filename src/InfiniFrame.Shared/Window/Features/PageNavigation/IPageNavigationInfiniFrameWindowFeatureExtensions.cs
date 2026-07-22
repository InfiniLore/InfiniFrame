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
    /// <returns>The <see cref="IInfiniFrameWindow"/> for method chaining.</returns>
    public static IInfiniFrameWindow Load(this IInfiniFrameWindow window, Uri uri) {
        window.Features.PageNavigation.Load(uri);
        return window;
    }

    /// <summary>
    ///     Loads the content at the specified path in the window and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="path">The file path or URL string to load.</param>
    /// <returns>The <see cref="IInfiniFrameWindow"/> for method chaining.</returns>
    public static IInfiniFrameWindow Load(this IInfiniFrameWindow window, string path) {
        window.Features.PageNavigation.Load(path);
        return window;
    }

    /// <summary>
    ///     Loads raw HTML content as a string in the window and returns the window for chaining.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="content">The raw HTML content to load.</param>
    /// <returns>The <see cref="IInfiniFrameWindow"/> for method chaining.</returns>
    public static IInfiniFrameWindow LoadRawString(this IInfiniFrameWindow window, string content) {
        window.Features.PageNavigation.LoadRawString(content);
        return window;
    }
}
