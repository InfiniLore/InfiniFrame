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

    bool DevToolsEnabled { get; }
    bool WebInspectorEnabled { get; }
    int RemoteDebuggingPort { get; }

    IInfiniFrameWindowBuilderFeatureDebugging SetDevToolsEnabled(bool enabled);

    [SupportedOSPlatform("macos13.3")]
    IInfiniFrameWindowBuilderFeatureDebugging SetWebInspectorEnabled(bool enabled = true);

    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    IInfiniFrameWindowBuilderFeatureDebugging SetRemoteDebuggingPort(int port);

    internal void ApplyToNativeParameters(ref InfiniFrameNativeParameters parameters);
}
