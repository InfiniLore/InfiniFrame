// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Debugging;
using System.Runtime.Versioning;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowFeatureDebugging {
    bool DevToolsEnabled { get; }
    bool SupportsWebInspector { get; }
    bool WebInspectorEnabled { get; }
    bool SupportsRemoteDebugging { get; }
    int? RemoteDebuggingPort { get; }

    InfiniFrameDebugCapabilities Capabilities { get; }
    void SetDevToolsEnabled(bool enabled);

    [SupportedOSPlatform("macos13.3")]
    void SetWebInspectorEnabled(bool enabled = true);

    InfiniFrameDebugDiagnostics GetDiagnostics();

    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    bool TryGetRemoteDebuggingEndpoint(out Uri? endpoint);

    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    bool TryProbeEndpoint(out Uri? endpoint, out string? reason);
}
