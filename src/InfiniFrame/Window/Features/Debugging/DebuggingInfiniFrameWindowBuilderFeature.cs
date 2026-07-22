// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;
using InfiniFrame.Utilities;
using System.Runtime.Versioning;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class DebuggingInfiniFrameWindowBuilderFeature : IDebuggingInfiniFrameWindowBuilderFeature {
    /// <inheritdoc cref="IDebuggingInfiniFrameWindowBuilderFeature.SupportsRemoteDebuggingEndpoint"/>
    public bool SupportsRemoteDebuggingEndpoint => RemoteDebuggingUtility.IsSupportedPlatform();

    /// <inheritdoc cref="IDebuggingInfiniFrameWindowBuilderFeature.SupportsWebInspectorAttach"/>
    public bool SupportsWebInspectorAttach => MacOsWebInspectorUtility.IsSupportedPlatform();

    /// <inheritdoc cref="IDebuggingInfiniFrameWindowBuilderFeature.IsDevToolsEnabled"/>
    public bool IsDevToolsEnabled { get; private set; } = true;

    /// <inheritdoc cref="IDebuggingInfiniFrameWindowBuilderFeature.IsWebInspectorEnabled"/>
    public bool IsWebInspectorEnabled { get; private set; }

    /// <inheritdoc cref="IDebuggingInfiniFrameWindowBuilderFeature.RemoteDebuggingPort"/>
    public int RemoteDebuggingPort { get; private set; }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="IDebuggingInfiniFrameWindowBuilderFeature.EnableDevTools"/>
    public IDebuggingInfiniFrameWindowBuilderFeature EnableDevTools(bool enabled) {
        IsDevToolsEnabled = enabled;
        return this;
    }

    /// <inheritdoc cref="IDebuggingInfiniFrameWindowBuilderFeature.EnableWebInspector"/>
    [SupportedOSPlatform("macos13.3")]
    public IDebuggingInfiniFrameWindowBuilderFeature EnableWebInspector(bool enabled = true) {
        MacOsWebInspectorUtility.ThrowIfUnsupported();

        IsWebInspectorEnabled = enabled;
        return this;
    }

    /// <inheritdoc cref="IDebuggingInfiniFrameWindowBuilderFeature.SetRemoteDebuggingPort"/>
    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    public IDebuggingInfiniFrameWindowBuilderFeature SetRemoteDebuggingPort(int port) {
        int normalized = RemoteDebuggingUtility.NormalizePort(port);
        RemoteDebuggingUtility.EnsureSupportedPlatform(normalized);
        RemoteDebuggingPort = normalized;
        return this;
    }

    public void ApplyToNativeParameters(ref InfiniFrameNativeParameters parameters) {
        parameters.DevToolsEnabled = IsDevToolsEnabled;
        parameters.WebInspectorEnabled = IsWebInspectorEnabled;
        parameters.RemoteDebuggingPort = RemoteDebuggingPort;
    }
}
