// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;
using System.Runtime.Versioning;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IDebuggingInfiniFrameWindowBuilderFeature {
    /// <summary>
    ///     Gets whether the platform supports a remote debugging endpoint.
    /// </summary>
    bool SupportsRemoteDebuggingEndpoint { get; }

    /// <summary>
    ///     Gets whether the platform supports Web Inspector attach.
    /// </summary>
    bool SupportsWebInspectorAttach { get; }

    /// <summary>
    ///     Gets whether developer tools are enabled.
    /// </summary>
    bool IsDevToolsEnabled { get; }

    /// <summary>
    ///     Gets whether the Web Inspector is enabled.
    /// </summary>
    bool IsWebInspectorEnabled { get; }

    /// <summary>
    ///     Gets the remote debugging port.
    /// </summary>
    int RemoteDebuggingPort { get; }

    /// <summary>
    ///     Enables or disables developer tools.
    /// </summary>
    /// <param name="enabled">Whether developer tools should be enabled.</param>
    /// <returns>The builder feature instance for chaining.</returns>
    IDebuggingInfiniFrameWindowBuilderFeature EnableDevTools(bool enabled);

    /// <summary>
    ///     Enables or disables the Web Inspector on macOS 13.3+.
    /// </summary>
    /// <param name="enabled">Whether the Web Inspector should be enabled.</param>
    /// <returns>The builder feature instance for chaining.</returns>
    [SupportedOSPlatform("macos13.3")]
    IDebuggingInfiniFrameWindowBuilderFeature EnableWebInspector(bool enabled = true);

    /// <summary>
    ///     Sets the remote debugging port on Windows and Linux.
    /// </summary>
    /// <param name="port">The port number to use for remote debugging.</param>
    /// <returns>The builder feature instance for chaining.</returns>
    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    IDebuggingInfiniFrameWindowBuilderFeature SetRemoteDebuggingPort(int port);

    internal void ApplyToNativeParameters(ref InfiniFrameNativeParameters parameters);
}
