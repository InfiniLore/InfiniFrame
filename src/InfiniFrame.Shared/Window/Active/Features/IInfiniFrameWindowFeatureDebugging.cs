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
    bool SupportsWebInspector { get; }
    bool IsWebInspectorEnabled { get; }
    bool SupportsRemoteDebugging { get; }
    int? RemoteDebuggingPort { get; }

    InfiniFrameDebugCapabilities Capabilities { get; }
    void EnableDevTools(bool enabled);

    [SupportedOSPlatform("macos13.3")]
    void EnableWebInspector(bool enabled = true);

    InfiniFrameDebugDiagnostics GetDiagnostics();

    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    bool TryGetRemoteDebuggingEndpoint(out Uri? endpoint);

    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    bool TryProbeEndpoint(out Uri? endpoint, out string? reason);
}
