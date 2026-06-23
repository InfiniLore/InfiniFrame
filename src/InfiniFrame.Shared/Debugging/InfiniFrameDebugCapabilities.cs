// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Debugging;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Describes the debugging capabilities supported by the current platform.
/// </summary>
public sealed record InfiniFrameDebugCapabilities {
    /// <summary>
    ///     Gets whether the platform supports local DevTools.
    /// </summary>
    public required bool SupportsLocalDevTools { get; init; }
    /// <summary>
    ///     Gets whether the platform supports a remote debugging endpoint.
    /// </summary>
    public required bool SupportsRemoteDebuggingEndpoint { get; init; }
    /// <summary>
    ///     Gets whether the platform supports web inspector attach.
    /// </summary>
    public required bool SupportsWebInspectorAttach { get; init; }
    /// <summary>
    ///     Gets whether the platform supports script error forwarding.
    /// </summary>
    public required bool SupportsScriptErrorForwarding { get; init; }
    /// <summary>
    ///     Gets whether the platform supports navigation diagnostics.
    /// </summary>
    public required bool SupportsNavigationDiagnostics { get; init; }
}
