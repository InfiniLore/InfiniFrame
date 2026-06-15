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
    public bool SupportsRemoteDebuggingEndpoint => RemoteDebuggingUtility.IsSupportedPlatform();
    public bool SupportsWebInspectorAttach => MacOsWebInspectorUtility.IsSupportedPlatform();

    public bool IsDevToolsEnabled { get; private set; } = true;
    public bool IsWebInspectorEnabled { get; private set; }
    public int RemoteDebuggingPort { get; private set; }
    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public IInfiniFrameWindowBuilderFeatureDebugging EnableDevTools(bool enabled) {
        IsDevToolsEnabled = enabled;
        return this;
    }

    [SupportedOSPlatform("macos13.3")]
    public IInfiniFrameWindowBuilderFeatureDebugging EnableWebInspector(bool enabled = true) {
        MacOsWebInspectorUtility.ThrowIfUnsupported();

        IsWebInspectorEnabled = enabled;
        return this;
    }

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
