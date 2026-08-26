// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Provides data for the navigation-starting event.
/// </summary>
/// <param name="Url">The target URL of the navigation.</param>
/// <param name="IsUserInitiated">True if the user initiated the navigation (e.g., link click, form submission).</param>
/// <param name="IsRedirect">True if the navigation is the result of a redirect.</param>
/// <param name="IsMainFrame">True if the navigation is in the main frame; false for sub-frame navigations.</param>
public sealed record NavigationStartingEventArgs(
    string Url,
    bool IsUserInitiated,
    bool IsRedirect,
    bool IsMainFrame
);
