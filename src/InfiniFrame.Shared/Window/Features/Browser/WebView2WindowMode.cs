// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Controls how Windows WebView2 browser environments are created for InfiniFrame windows.
/// </summary>
public enum WebView2WindowMode {
    /// <summary>
    ///     Creates an isolated WebView2 profile for each window. This is the compatibility default.
    /// </summary>
    IsolatedPerWindow = 0,

    /// <summary>
    ///     Uses the process-wide WebView2 window manager to share compatible WebView2 environment startup state.
    /// </summary>
    ManagedShared = 1
}
