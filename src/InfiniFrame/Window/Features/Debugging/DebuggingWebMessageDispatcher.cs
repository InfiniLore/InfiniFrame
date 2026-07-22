// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Text.Json;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal sealed class DebuggingWebMessageDispatcher : WindowFeatureWebMessageDispatcherBase<IDebuggingInfiniFrameWindowFeature> {
    public override string FeatureName => "debugging";

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    protected override IDebuggingInfiniFrameWindowFeature SelectFeature(IInfiniFrameWindowFeatures features)
        => features.Debugging;

    protected override object? Get(IDebuggingInfiniFrameWindowFeature feature, string command, JsonElement? args) => command switch {
        "isDevToolsEnabled" => feature.IsDevToolsEnabled,
        "supportsWebInspectorAttach" => feature.SupportsWebInspectorAttach,
        "isWebInspectorEnabled" => feature.IsWebInspectorEnabled,
        "supportsRemoteDebuggingEndpoint" => feature.SupportsRemoteDebuggingEndpoint,
        "remoteDebuggingPort" => feature.RemoteDebuggingPort,
        "capabilities" => feature.Capabilities,
        "diagnostics" => feature.GetDiagnostics(),
        "remoteDebuggingEndpoint" => GetRemoteDebuggingEndpoint(feature),
        "probeEndpoint" => ProbeEndpoint(feature),
        _ => throw Unsupported(command)
    };

    protected override void Post(IDebuggingInfiniFrameWindowFeature feature, string command, JsonElement? args) {
        if (command == "enableDevTools") feature.EnableDevTools(Required<bool>(args, "enabled"));
        else throw Unsupported(command);
    }

    private static DebugEndpointResult GetRemoteDebuggingEndpoint(IDebuggingInfiniFrameWindowFeature feature) {
        if (!OperatingSystem.IsWindows() && !OperatingSystem.IsLinux())
            return new DebugEndpointResult(false, null, "Remote debugging endpoints are not supported on this platform.");
        bool success = feature.TryGetRemoteDebuggingEndpoint(out Uri? endpoint);
        return new DebugEndpointResult(success, endpoint?.ToString(), null);
    }

    private static DebugEndpointResult ProbeEndpoint(IDebuggingInfiniFrameWindowFeature feature) {
        if (!OperatingSystem.IsWindows() && !OperatingSystem.IsLinux())
            return new DebugEndpointResult(false, null, "Remote debugging endpoints are not supported on this platform.");
        bool success = feature.TryProbeEndpoint(out Uri? endpoint, out string? reason);
        return new DebugEndpointResult(success, endpoint?.ToString(), reason);
    }
}
