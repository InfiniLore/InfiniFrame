// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Debugging;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Defines the kinds of debug events that can occur in the browser control.
/// </summary>
public enum InfiniFrameDebugEventKind {
    /// <summary>
    ///     A script error occurred in the browser control.
    /// </summary>
    ScriptError,
    /// <summary>
    ///     A navigation event occurred in the browser control.
    /// </summary>
    Navigation,
    /// <summary>
    ///     A process-level event occurred.
    /// </summary>
    Process,
    /// <summary>
    ///     A runtime event occurred.
    /// </summary>
    Runtime
}