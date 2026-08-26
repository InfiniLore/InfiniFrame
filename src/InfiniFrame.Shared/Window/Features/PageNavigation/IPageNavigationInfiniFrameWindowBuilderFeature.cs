// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Configures the initial page content before window creation.
/// </summary>
public interface IPageNavigationInfiniFrameWindowBuilderFeature : IInfiniFrameWindowBuilderFeature {
    /// <summary>
    ///     Gets the start page content to display when the window is created.
    /// </summary>
    string? StartString { get; }

    /// <summary>
    ///     Gets the start page URL to navigate to when the window is created.
    /// </summary>
    string? StartUrl { get; }

    /// <summary>
    ///     Sets the content to display as the start page.
    /// </summary>
    /// <param name="content">The raw HTML content for the start page.</param>
    void SetStartPageContent(string? content);

    /// <summary>
    ///     Sets the URL to navigate to as the start page.
    /// </summary>
    /// <param name="startUrl">The start page URL as a string.</param>
    void SetStartPageUrl(string? startUrl);

    /// <summary>
    ///     Sets the URI to navigate to when the window is created.
    /// </summary>
    /// <param name="startUrl">The start page URI.</param>
    void SetUrl(Uri? startUrl);
}
