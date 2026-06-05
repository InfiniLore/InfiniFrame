namespace InfiniFrame.Debugging;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowDebugBuilder {
    bool SupportsRemoteDebuggingEndpoint { get; }
    bool SupportsWebInspectorAttach { get; }

    bool DevToolsEnabled { get; }
    bool WebInspectorEnabled { get; }
    int? RemoteDebuggingPort { get; }

    void SetDevToolsEnabled(bool enabled);
    void SetWebInspectorEnabled(bool enabled = true);
    void SetRemoteDebuggingPort(int? port);
}
