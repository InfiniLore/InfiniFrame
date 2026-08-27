// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>The terminal outcome of work dispatched to a native window thread.</summary>
public enum InfiniFrameDispatchResult {
    /// <summary>
    ///     The dispatched work completed successfully.
    /// </summary>
    Completed,

    /// <summary>
    ///     The dispatched work did not complete within the specified timeout.
    /// </summary>
    TimedOut,

    /// <summary>
    ///     The dispatched work was cancelled before execution.
    /// </summary>
    Cancelled,

    /// <summary>
    ///     The window was closed before the dispatched work could execute.
    /// </summary>
    WindowClosed,

    /// <summary>
    ///     The dispatched work failed with an exception.
    /// </summary>
    Failed
}
