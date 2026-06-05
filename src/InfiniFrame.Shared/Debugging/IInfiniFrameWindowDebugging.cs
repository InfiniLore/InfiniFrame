namespace InfiniFrame.Debugging;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowDebugging {
    bool DevToolsEnabled { get; }
    bool SupportsWebInspector { get; }
    bool WebInspectorEnabled { get; }
    bool SupportsRemoteDebugging { get; }
    int? RemoteDebuggingPort { get; }

    InfiniFrameDebugCapabilities Capabilities { get; }
    event EventHandler<InfiniFrameDebugEventArgs>? Event;
    void SetDevToolsEnabled(bool enabled);
    void SetWebInspectorEnabled(bool enabled = true);
    InfiniFrameDebugDiagnostics GetDiagnostics();
    bool TryGetRemoteDebuggingEndpoint(out Uri? endpoint);
    bool TryProbeEndpoint(out Uri? endpoint, out string? reason);
}
