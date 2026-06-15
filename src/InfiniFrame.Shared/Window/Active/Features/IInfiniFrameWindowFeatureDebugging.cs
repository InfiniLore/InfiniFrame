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
    bool IsDevToolsEnabled { get; }
    bool SupportsWebInspectorAttach { get; }
    bool IsWebInspectorEnabled { get; }
    bool SupportsRemoteDebuggingEndpoint { get; }
    int? RemoteDebuggingPort { get; }

    InfiniFrameDebugCapabilities Capabilities { get; }
    void EnableDevTools(bool enabled);

    InfiniFrameDebugDiagnostics GetDiagnostics();

    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    bool TryGetRemoteDebuggingEndpoint(out Uri? endpoint);

    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    bool TryProbeEndpoint(out Uri? endpoint, out string? reason);
}
