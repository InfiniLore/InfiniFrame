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

    public bool DevToolsEnabled { get; private set; } = true;
    public bool WebInspectorEnabled { get; private set; }
    public int RemoteDebuggingPort { get; private set; }
    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public IInfiniFrameWindowBuilderFeatureDebugging SetDevToolsEnabled(bool enabled) {
        DevToolsEnabled = enabled;
        return this;
    }

    [SupportedOSPlatform("macos13.3")]
    public IInfiniFrameWindowBuilderFeatureDebugging SetWebInspectorEnabled(bool enabled = true) {
        MacOsWebInspectorUtility.ThrowIfUnsupported();

        WebInspectorEnabled = enabled;
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
        parameters.DevToolsEnabled = DevToolsEnabled;
        parameters.WebInspectorEnabled = WebInspectorEnabled;
        parameters.RemoteDebuggingPort = RemoteDebuggingPort;
    }
}
