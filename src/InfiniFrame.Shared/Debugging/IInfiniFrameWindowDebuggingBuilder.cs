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
    IInfiniFrameWindowDebuggingBuilder SetWebInspectorEnabled(bool enabled = true);
    IInfiniFrameWindowDebuggingBuilder SetRemoteDebuggingPort(int port);
}
