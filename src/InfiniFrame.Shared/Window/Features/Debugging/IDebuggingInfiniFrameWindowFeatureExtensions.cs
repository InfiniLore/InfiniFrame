// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.Versioning;
using InfiniFrame.Debugging;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Fluent extension methods for <see cref="IDebuggingInfiniFrameWindowFeature" /> on <see cref="IInfiniFrameWindow" />.
/// </summary>
public static class IDebuggingInfiniFrameWindowFeatureExtensions {
    /// <param name="window">The window instance.</param>
    extension(IInfiniFrameWindow window) {
        /// <summary>
        ///     Enables or disables developer tools for the window.
        /// </summary>
        /// <param name="enabled">Whether developer tools should be enabled.</param>
        /// <returns>The window instance for chaining.</returns>
        public IInfiniFrameWindow EnableDevTools(bool enabled = true) {
            window.Features.Debugging.EnableDevTools(enabled);
            return window;
        }
        /// <summary>
        ///     Attempts to get the remote debugging endpoint URI for the window.
        /// </summary>
        /// <param name="endpoint">When successful, contains the remote debugging endpoint URI.</param>
        /// <returns><c>true</c> if the endpoint was retrieved; otherwise <c>false</c>.</returns>
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("linux")]
        public bool TryGetRemoteDebuggingEndpoint(out Uri? endpoint)
            => window.Features.Debugging.TryGetRemoteDebuggingEndpoint(out endpoint);
        /// <summary>
        ///     Attempts to probe the remote debugging endpoint for reachability.
        /// </summary>
        /// <param name="endpoint">When successful, contains the remote debugging endpoint URI.</param>
        /// <param name="reason">When unsuccessful, contains a description of why probing failed.</param>
        /// <returns><c>true</c> if the endpoint is reachable; otherwise <c>false</c>.</returns>
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("linux")]
        public bool TryProbeRemoteDebuggingEndpoint(out Uri? endpoint, out string? reason)
            => window.Features.Debugging.TryProbeEndpoint(out endpoint, out reason);
        /// <summary>
        ///     Gets diagnostics information about the current debugging state.
        /// </summary>
        /// <returns>A <see cref="InfiniFrameDebugDiagnostics" /> instance with diagnostic data.</returns>
        public InfiniFrameDebugDiagnostics GetDebugDiagnostics()
            => window.Features.Debugging.GetDiagnostics();
        /// <summary>
        ///     Gets whether the platform supports Web Inspector attach.
        /// </summary>
        /// <returns><c>true</c> if Web Inspector attach is supported; otherwise <c>false</c>.</returns>
        public bool SupportsWebInspectorAttach() => window.Features.Debugging.SupportsWebInspectorAttach;
        /// <summary>
        ///     Gets whether the platform supports a remote debugging endpoint.
        /// </summary>
        /// <returns><c>true</c> if remote debugging is supported; otherwise <c>false</c>.</returns>
        public bool SupportsRemoteDebuggingEndpoint() => window.Features.Debugging.SupportsRemoteDebuggingEndpoint;
    }

}
