// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Debugging;
using System.Runtime.Versioning;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class IDebuggingInfiniFrameWindowFeatureExtensions {
    /// <summary>
    ///     Enables or disables developer tools for the window.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="enabled">Whether developer tools should be enabled.</param>
    /// <returns>The window instance for chaining.</returns>
    public static IInfiniFrameWindow EnableDevTools(this IInfiniFrameWindow window, bool enabled = true) {
        window.Features.Debugging.EnableDevTools(enabled);
        return window;
    }

    /// <summary>
    ///     Attempts to get the remote debugging endpoint URI for the window.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="endpoint">When successful, contains the remote debugging endpoint URI.</param>
    /// <returns><c>true</c> if the endpoint was retrieved; otherwise <c>false</c>.</returns>
    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    public static bool TryGetRemoteDebuggingEndpoint(this IInfiniFrameWindow window, out Uri? endpoint)
        => window.Features.Debugging.TryGetRemoteDebuggingEndpoint(out endpoint);

    /// <summary>
    ///     Attempts to probe the remote debugging endpoint for reachability.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="endpoint">When successful, contains the remote debugging endpoint URI.</param>
    /// <param name="reason">When unsuccessful, contains a description of why probing failed.</param>
    /// <returns><c>true</c> if the endpoint is reachable; otherwise <c>false</c>.</returns>
    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    public static bool TryProbeRemoteDebuggingEndpoint(this IInfiniFrameWindow window, out Uri? endpoint, out string? reason)
        => window.Features.Debugging.TryProbeEndpoint(out endpoint, out reason);

    /// <summary>
    ///     Gets diagnostics information about the current debugging state.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <returns>A <see cref="InfiniFrameDebugDiagnostics"/> instance with diagnostic data.</returns>
    public static InfiniFrameDebugDiagnostics GetDebugDiagnostics(this IInfiniFrameWindow window)
        => window.Features.Debugging.GetDiagnostics();

    /// <summary>
    ///     Gets whether the platform supports Web Inspector attach.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <returns><c>true</c> if Web Inspector attach is supported; otherwise <c>false</c>.</returns>
    public static bool SupportsWebInspectorAttach(this IInfiniFrameWindow window) {
        return window.Features.Debugging.SupportsWebInspectorAttach;
    }

    /// <summary>
    ///     Gets whether the platform supports a remote debugging endpoint.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <returns><c>true</c> if remote debugging is supported; otherwise <c>false</c>.</returns>
    public static bool SupportsRemoteDebuggingEndpoint(this IInfiniFrameWindow window) {
        return window.Features.Debugging.SupportsRemoteDebuggingEndpoint;
    }
}
