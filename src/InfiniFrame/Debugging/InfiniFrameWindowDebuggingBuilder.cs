// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Utilities;
using System.Runtime.Versioning;

namespace InfiniFrame.Debugging;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class InfiniFrameWindowDebuggingBuilder : IInfiniFrameWindowDebuggingBuilder {
    public bool SupportsRemoteDebuggingEndpoint => RemoteDebuggingUtility.IsSupportedPlatform();
    public bool SupportsWebInspectorAttach => WebInspectorUtility.IsSupportedPlatform();

    public bool DevToolsEnabled { get; private set; } = true;
    public bool WebInspectorEnabled { get; private set; }
    public int RemoteDebuggingPort { get; private set; }
    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public IInfiniFrameWindowDebuggingBuilder SetDevToolsEnabled(bool enabled) {
        DevToolsEnabled = enabled;
        return this;
    }

    [SupportedOSPlatform("macos13.3")]
    public IInfiniFrameWindowDebuggingBuilder SetWebInspectorEnabled(bool enabled = true) {
        if (enabled) {
            WebInspectorUtility.ThrowIfUnsupported();
        }

        WebInspectorEnabled = enabled;
        return this;
    }

    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    public IInfiniFrameWindowDebuggingBuilder SetRemoteDebuggingPort(int port) {
        int normalized = RemoteDebuggingUtility.NormalizePort(port);
        RemoteDebuggingUtility.EnsureSupportedPlatform(normalized);
        RemoteDebuggingPort = normalized;
        return this;
    }
}
