// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;
using System.Runtime.Versioning;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowBuilderFeatureDebugging {
    bool SupportsRemoteDebuggingEndpoint { get; }
    bool SupportsWebInspectorAttach { get; }

    bool IsDevToolsEnabled { get; }
    bool IsWebInspectorEnabled { get; }
    int RemoteDebuggingPort { get; }

    IInfiniFrameWindowBuilderFeatureDebugging EnableDevTools(bool enabled);

    [SupportedOSPlatform("macos13.3")]
    IInfiniFrameWindowBuilderFeatureDebugging EnableWebInspector(bool enabled = true);

    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    IInfiniFrameWindowBuilderFeatureDebugging SetRemoteDebuggingPort(int port);

    internal void ApplyToNativeParameters(ref InfiniFrameNativeParameters parameters);
}
