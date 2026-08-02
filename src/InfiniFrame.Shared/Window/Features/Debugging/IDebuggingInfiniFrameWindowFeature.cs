// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Debugging;
using System.Runtime.Versioning;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IDebuggingInfiniFrameWindowFeature {
    /// <summary>
    ///     Gets whether developer tools are enabled.
    /// </summary>
    bool IsDevToolsEnabled { get; }

    /// <summary>
    ///     Gets whether the platform supports Web Inspector attach.
    /// </summary>
    bool SupportsWebInspectorAttach { get; }

    /// <summary>
    ///     Gets whether the Web Inspector is enabled.
    /// </summary>
    bool IsWebInspectorEnabled { get; }

    /// <summary>
    ///     Gets whether the platform supports a remote debugging endpoint.
    /// </summary>
    bool SupportsRemoteDebuggingEndpoint { get; }

    /// <summary>
    ///     Gets the remote debugging port, if configured.
    /// </summary>
    int? RemoteDebuggingPort { get; }

    /// <summary>
    ///     Gets the debugging capabilities of the current platform.
    /// </summary>
    InfiniFrameDebugCapabilities Capabilities { get; }

    /// <summary>
    ///     Enables or disables developer tools.
    /// </summary>
    /// <param name="enabled">Whether developer tools should be enabled.</param>
    void EnableDevTools(bool enabled);

    /// <summary>
    ///     Gets diagnostics information about the current debugging state.
    /// </summary>
    /// <returns>A <see cref="InfiniFrameDebugDiagnostics"/> instance with diagnostic data.</returns>
    InfiniFrameDebugDiagnostics GetDiagnostics();

    /// <summary>
    ///     Attempts to get the remote debugging endpoint URI.
    /// </summary>
    /// <param name="endpoint">When successful, contains the remote debugging endpoint URI.</param>
    /// <returns><c>true</c> if the endpoint was retrieved; otherwise <c>false</c>.</returns>
    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    bool TryGetRemoteDebuggingEndpoint(out Uri? endpoint);

    /// <summary>
    ///     Attempts to probe the remote debugging endpoint for reachability.
    /// </summary>
    /// <param name="endpoint">When successful, contains the remote debugging endpoint URI.</param>
    /// <param name="reason">When unsuccessful, contains a description of why probing failed.</param>
    /// <returns><c>true</c> if the endpoint is reachable; otherwise <c>false</c>.</returns>
    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    bool TryProbeEndpoint(out Uri? endpoint, out string? reason);
}