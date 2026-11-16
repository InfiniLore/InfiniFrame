// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.Versioning;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Fluent extension methods for <see cref="IDebuggingInfiniFrameWindowBuilderFeature"/> on <see cref="IInfiniFrameWindowBuilder"/>.
/// </summary>
public static class IDebuggingInfiniFrameWindowBuilderFeatureExtensions {
    /// <param name="builder">The builder instance.</param>
    extension(IInfiniFrameWindowBuilder builder) {
        /// <summary>
        ///     Enables or disables developer tools for the builder.
        /// </summary>
        /// <param name="enabled">Whether developer tools should be enabled.</param>
        /// <returns>The builder instance for chaining.</returns>
        public IInfiniFrameWindowBuilder EnableDevTools(bool enabled = true) {
            builder.Features.Debugging.EnableDevTools(enabled);
            return builder;
        }
        /// <summary>
        ///     Enables or disables the Web Inspector on macOS 13.3+.
        /// </summary>
        /// <param name="enabled">Whether the Web Inspector should be enabled.</param>
        /// <returns>The builder instance for chaining.</returns>
        [SupportedOSPlatform("macos13.3")]
        public IInfiniFrameWindowBuilder EnableWebInspector(bool enabled = true) {
            builder.Features.Debugging.EnableWebInspector(enabled);
            return builder;
        }
        /// <summary>
        ///     Sets the remote debugging port on Windows and Linux.
        /// </summary>
        /// <param name="port">The port number to use for remote debugging.</param>
        /// <returns>The builder instance for chaining.</returns>
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("linux")]
        public IInfiniFrameWindowBuilder SetRemoteDebuggingPort(int port) {
            builder.Features.Debugging.SetRemoteDebuggingPort(port);
            return builder;
        }
        /// <summary>
        ///     Gets whether the platform supports Web Inspector attach.
        /// </summary>
        /// <returns><c>true</c> if Web Inspector attach is supported; otherwise <c>false</c>.</returns>
        public bool SupportsWebInspectorAttach() => builder.Features.Debugging.SupportsWebInspectorAttach;
        /// <summary>
        ///     Gets whether the platform supports a remote debugging endpoint.
        /// </summary>
        /// <returns><c>true</c> if remote debugging is supported; otherwise <c>false</c>.</returns>
        public bool SupportsRemoteDebuggingEndpoint() => builder.Features.Debugging.SupportsRemoteDebuggingEndpoint;
    }

}
