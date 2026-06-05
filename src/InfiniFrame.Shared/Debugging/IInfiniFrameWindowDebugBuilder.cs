// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Debugging;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowDebugBuilder {
    bool SupportsRemoteDebuggingEndpoint { get; }
    bool SupportsWebInspectorAttach { get; }

    bool DevToolsEnabled { get; set; }
    bool WebInspectorEnabled { get; set; }
    int? RemoteDebuggingPort { get; set; }

    IInfiniFrameWindowBuilder SetDevToolsEnabled(bool enabled);
    IInfiniFrameWindowBuilder SetWebInspectorEnabled(bool enabled = true);
    IInfiniFrameWindowBuilder SetRemoteDebuggingPort(int? port);
}
