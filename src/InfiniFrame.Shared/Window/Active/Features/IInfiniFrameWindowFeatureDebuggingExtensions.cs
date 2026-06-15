// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Debugging;
using System.Runtime.Versioning;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class IInfiniFrameWindowFeatureDebuggingExtensions {
    public static IInfiniFrameWindow EnableDevTools(this IInfiniFrameWindow window, bool enabled = true) {
        window.Features.Debugging.EnableDevTools(enabled);
        return window;
    }

    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    public static bool TryGetRemoteDebuggingEndpoint(this IInfiniFrameWindow window, out Uri? endpoint)
        => window.Features.Debugging.TryGetRemoteDebuggingEndpoint(out endpoint);

    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    public static bool TryProbeRemoteDebuggingEndpoint(this IInfiniFrameWindow window, out Uri? endpoint, out string? reason)
        => window.Features.Debugging.TryProbeEndpoint(out endpoint, out reason);

    public static InfiniFrameDebugDiagnostics GetDebugDiagnostics(this IInfiniFrameWindow window)
        => window.Features.Debugging.GetDiagnostics();
    
    public static bool SupportsWebInspectorAttach(this IInfiniFrameWindow window) {
        return window.Features.Debugging.SupportsWebInspectorAttach;
    }
    
    public static bool SupportsRemoteDebuggingEndpoint(this IInfiniFrameWindow window) {
        return window.Features.Debugging.SupportsRemoteDebuggingEndpoint;
    }
}
