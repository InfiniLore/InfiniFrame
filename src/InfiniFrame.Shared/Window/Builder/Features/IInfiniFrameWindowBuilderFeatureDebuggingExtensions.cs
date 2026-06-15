// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.Versioning;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class IInfiniFrameWindowBuilderFeatureDebuggingExtensions {
    public static IInfiniFrameWindowBuilder EnableDevTools(this IInfiniFrameWindowBuilder builder, bool enabled = true) {
        builder.Features.Debugging.EnableDevTools(enabled);
        return builder;
    }

    [SupportedOSPlatform("macos13.3")]
    public static IInfiniFrameWindowBuilder EnableWebInspector(this IInfiniFrameWindowBuilder builder, bool enabled = true) {
        builder.Features.Debugging.EnableWebInspector(enabled);
        return builder;
    }

    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    public static IInfiniFrameWindowBuilder SetRemoteDebuggingPort(this IInfiniFrameWindowBuilder builder, int port) {
        builder.Features.Debugging.SetRemoteDebuggingPort(port);
        return builder;
    }
}
