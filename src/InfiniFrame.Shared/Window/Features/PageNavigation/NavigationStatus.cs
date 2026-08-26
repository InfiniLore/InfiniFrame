// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Represents the outcome of a page navigation operation.
/// </summary>
public enum NavigationStatus {
    /// <summary>
    ///     The navigation completed successfully.
    /// </summary>
    Succeeded,

    /// <summary>
    ///     The navigation failed due to an error.
    /// </summary>
    Failed,

    /// <summary>
    ///     The navigation was superseded by another navigation before completing.
    /// </summary>
    Superseded,

    /// <summary>
    ///     The window was closed before the navigation completed.
    /// </summary>
    WindowClosed
}
