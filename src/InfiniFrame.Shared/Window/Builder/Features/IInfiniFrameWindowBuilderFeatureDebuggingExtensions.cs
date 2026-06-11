// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.Versioning;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class IInfiniFrameWindowBuilderFeatureDebuggingExtensions {
    public static IInfiniFrameWindowBuilder SetDevToolsEnabled(this IInfiniFrameWindowBuilder builder, bool enabled = true) {
        builder.Features.Debugging.SetDevToolsEnabled(enabled);
        return builder;
    }

    [SupportedOSPlatform("macos13.3")]
    public static IInfiniFrameWindowBuilder SetWebInspectorEnabled(this IInfiniFrameWindowBuilder builder, bool enabled = true) {
        builder.Features.Debugging.SetWebInspectorEnabled(enabled);
        return builder;
    }

    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    public static IInfiniFrameWindowBuilder SetRemoteDebuggingPort(this IInfiniFrameWindowBuilder builder, int port) {
        builder.Features.Debugging.SetRemoteDebuggingPort(port);
        return builder;
    }
}
