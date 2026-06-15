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
public sealed class InfiniFrameWindowBuilderFeatureDebugging : IInfiniFrameWindowBuilderFeatureDebugging {
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureDebugging.SupportsRemoteDebuggingEndpoint"/>
    public bool SupportsRemoteDebuggingEndpoint => RemoteDebuggingUtility.IsSupportedPlatform();

    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureDebugging.SupportsWebInspectorAttach"/>
    public bool SupportsWebInspectorAttach => MacOsWebInspectorUtility.IsSupportedPlatform();

    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureDebugging.IsDevToolsEnabled"/>
    public bool IsDevToolsEnabled { get; private set; } = true;

    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureDebugging.IsWebInspectorEnabled"/>
    public bool IsWebInspectorEnabled { get; private set; }

    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureDebugging.RemoteDebuggingPort"/>
    public int RemoteDebuggingPort { get; private set; }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureDebugging.EnableDevTools"/>
    public IInfiniFrameWindowBuilderFeatureDebugging EnableDevTools(bool enabled) {
        IsDevToolsEnabled = enabled;
        return this;
    }

    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureDebugging.EnableWebInspector"/>
    [SupportedOSPlatform("macos13.3")]
    public IInfiniFrameWindowBuilderFeatureDebugging EnableWebInspector(bool enabled = true) {
        MacOsWebInspectorUtility.ThrowIfUnsupported();

        IsWebInspectorEnabled = enabled;
        return this;
    }

    /// <inheritdoc cref="IInfiniFrameWindowBuilderFeatureDebugging.SetRemoteDebuggingPort"/>
    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    public IInfiniFrameWindowBuilderFeatureDebugging SetRemoteDebuggingPort(int port) {
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
