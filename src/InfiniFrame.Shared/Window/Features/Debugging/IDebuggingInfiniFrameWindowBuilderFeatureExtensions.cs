// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.Versioning;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class IDebuggingInfiniFrameWindowBuilderFeatureExtensions {
    /// <summary>
    ///     Enables or disables developer tools for the builder.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="enabled">Whether developer tools should be enabled.</param>
    /// <returns>The builder instance for chaining.</returns>
    public static IInfiniFrameWindowBuilder EnableDevTools(this IInfiniFrameWindowBuilder builder, bool enabled = true) {
        builder.Features.Debugging.EnableDevTools(enabled);
        return builder;
    }

    /// <summary>
    ///     Enables or disables the Web Inspector on macOS 13.3+.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="enabled">Whether the Web Inspector should be enabled.</param>
    /// <returns>The builder instance for chaining.</returns>
    [SupportedOSPlatform("macos13.3")]
    public static IInfiniFrameWindowBuilder EnableWebInspector(this IInfiniFrameWindowBuilder builder, bool enabled = true) {
        builder.Features.Debugging.EnableWebInspector(enabled);
        return builder;
    }

    /// <summary>
    ///     Sets the remote debugging port on Windows and Linux.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <param name="port">The port number to use for remote debugging.</param>
    /// <returns>The builder instance for chaining.</returns>
    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    public static IInfiniFrameWindowBuilder SetRemoteDebuggingPort(this IInfiniFrameWindowBuilder builder, int port) {
        builder.Features.Debugging.SetRemoteDebuggingPort(port);
        return builder;
    }

    /// <summary>
    ///     Gets whether the platform supports Web Inspector attach.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <returns><c>true</c> if Web Inspector attach is supported; otherwise <c>false</c>.</returns>
    public static bool SupportsWebInspectorAttach(this IInfiniFrameWindowBuilder builder) {
        return builder.Features.Debugging.SupportsWebInspectorAttach;
    }

    /// <summary>
    ///     Gets whether the platform supports a remote debugging endpoint.
    /// </summary>
    /// <param name="builder">The builder instance.</param>
    /// <returns><c>true</c> if remote debugging is supported; otherwise <c>false</c>.</returns>
    public static bool SupportsRemoteDebuggingEndpoint(this IInfiniFrameWindowBuilder builder) {
        return builder.Features.Debugging.SupportsRemoteDebuggingEndpoint;
    }
}
