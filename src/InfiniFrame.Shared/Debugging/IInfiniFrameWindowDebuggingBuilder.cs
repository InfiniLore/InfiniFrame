using System.Runtime.Versioning;

namespace InfiniFrame.Debugging;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowDebuggingBuilder {
    bool SupportsRemoteDebuggingEndpoint { get; }
    bool SupportsWebInspectorAttach { get; }

    bool DevToolsEnabled { get; }
    bool WebInspectorEnabled { get; }
    int RemoteDebuggingPort { get; }

    IInfiniFrameWindowDebuggingBuilder SetDevToolsEnabled(bool enabled);
    
    [SupportedOSPlatform("macos13.3")]
    IInfiniFrameWindowDebuggingBuilder SetWebInspectorEnabled(bool enabled = true);
    
    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    IInfiniFrameWindowDebuggingBuilder SetRemoteDebuggingPort(int port);
}
