// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Blazor;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Specifies actions that can be performed on a window from Blazor interop.
/// </summary>
public enum WindowAction {
    /// <summary>
    ///     Minimize the window.
    /// </summary>
    Minimize,
    /// <summary>
    ///     Maximize the window.
    /// </summary>
    Maximize,
    /// <summary>
    ///     Close the window.
    /// </summary>
    Close
}