// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge;
using InfiniFrame.Utilities;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace InfiniFrame.Debugging;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class InfiniFrameWindowDebugging(ILogger<InfiniFrameWindowDebugging> logger) : IInfiniFrameWindowDebugging {
    public bool DevToolsEnabled => InvokeUtility.InvokeAndReturn<bool, InfiniFrameNativeInteropStatus>(
        Window,
        InfiniFrameNative.GetDevToolsEnabled,
        validateResult: s => InfiniFrameNative.EnsureSucceeded(s, nameof(InfiniFrameNative.GetDevToolsEnabled)));
    public bool SupportsWebInspector => MacOsWebInspectorUtility.IsSupportedPlatform();
    public bool WebInspectorEnabled => Window.Configuration.StartupParameters.WebInspectorEnabled;
    public bool SupportsRemoteDebugging => RemoteDebuggingUtility.IsSupportedPlatform();
    public int? RemoteDebuggingPort => Window.Configuration.StartupParameters.RemoteDebuggingPort > 0
        ? Window.Configuration.StartupParameters.RemoteDebuggingPort
        : null;

    public InfiniFrameDebugCapabilities Capabilities => new() {
        SupportsLocalDevTools = true,
        SupportsRemoteDebuggingEndpoint = SupportsRemoteDebugging,
        SupportsWebInspectorAttach = SupportsWebInspector,
        SupportsScriptErrorForwarding = true,
        SupportsNavigationDiagnostics = true
    };

    private IInfiniFrameWindow Window { get; set; } = null!;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    internal void AssignToWindow(IInfiniFrameWindow window) {
        ArgumentNullException.ThrowIfNull(window);
        Window = window;
    }
    
    public void SetDevToolsEnabled(bool enabled) {
        logger.LogDebug(".Debug.SetDevToolsEnabled({Enabled})", enabled);

        Window.Invoke(() => {
            InfiniFrameNative.GetDevToolsEnabled(Window.InstanceHandle, out bool isEnabled);
            if (isEnabled == enabled) return;

            InfiniFrameNative.SetDevToolsEnabled(Window.InstanceHandle, enabled);
        });
    }

    [SupportedOSPlatform("macos13.3")]
    public void SetWebInspectorEnabled(bool enabled = true) {
        throw new InvalidOperationException("WebInspectorEnabled is startup-only. Configure it with builder.SetWebInspectorEnabled(...) before Build().");
    }

    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    public bool TryGetRemoteDebuggingEndpoint(out Uri? endpoint) {
        endpoint = null;
        if (!SupportsRemoteDebugging) return false;

        int? port = RemoteDebuggingPort;
        if (!port.HasValue || Window.IsClosedOrClosing)
            return false;

        endpoint = RemoteDebuggingUtility.CreateEndpointUri(port.Value);
        return true;
    }

    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    public bool TryProbeEndpoint(out Uri? endpoint, out string? reason) {
        endpoint = null;
        reason = null;

        if (!Capabilities.SupportsRemoteDebuggingEndpoint) {
            reason = "Remote debugging endpoint probing is not supported on this platform.";
            logger.LogDebug("Debug endpoint probe skipped: {Reason}", reason);
            return false;
        }

        if (!TryGetRemoteDebuggingEndpoint(out endpoint) || endpoint is null) {
            reason = Window.IsClosedOrClosing
                ? "Window is closed."
                : "Remote debugging is disabled.";
            logger.LogDebug("Debug endpoint probe unavailable: {Reason}", reason);
            return false;
        }

        if (RemoteDebuggingUtility.TryProbeEndpoint(endpoint, out string? probeFailure)) {
            logger.LogDebug("Debug endpoint probe succeeded for {Endpoint}", endpoint);
            return true;
        }

        reason = probeFailure;
        logger.LogWarning("Debug endpoint probe failed for {Endpoint}: {Reason}", endpoint, reason);
        return false;
    }

    public InfiniFrameDebugDiagnostics GetDiagnostics() {
        Uri? endpoint = null;
        string? endpointReason = null;
        InfiniFrameDebugEndpointStatus endpointStatus;
        if (!IsRemoteDebuggingPlatform()) {
            endpointStatus = InfiniFrameDebugEndpointStatus.NotSupported;
        }
        else if (!RemoteDebuggingPort.HasValue) {
            endpointStatus = InfiniFrameDebugEndpointStatus.Disabled;
            endpointReason = "Remote debugging is disabled.";
        }
        else if (Window.IsClosedOrClosing || !TryGetRemoteDebuggingEndpoint(out endpoint) || endpoint is null) {
            endpointStatus = InfiniFrameDebugEndpointStatus.Unavailable;
            endpointReason = "Window is closed.";
        }
        else if (TryProbeEndpoint(out _, out endpointReason)) {
            endpointStatus = InfiniFrameDebugEndpointStatus.Reachable;
        }
        else if (string.IsNullOrWhiteSpace(endpointReason)) {
            endpointStatus = InfiniFrameDebugEndpointStatus.Configured;
        }
        else {
            endpointStatus = InfiniFrameDebugEndpointStatus.Unreachable;
        }

        return new InfiniFrameDebugDiagnostics {
            Platform = RuntimeInformation.OSDescription,
            Runtime = RuntimeInformation.FrameworkDescription,
            BrowserRuntime = GetBrowserRuntimeIdentity(),
            Capabilities = Capabilities,
            DevToolsEnabled = DevToolsEnabled,
            RemoteDebuggingPort = RemoteDebuggingPort,
            WebInspectorEnabled = WebInspectorEnabled,
            EndpointStatus = endpointStatus,
            Endpoint = endpoint,
            EndpointReason = endpointReason,
            IsWindowClosed = Window.IsClosedOrClosing,
            PlatformNotes = GetPlatformDiagnosticsNotes()
        };
    }

    private static string? GetPlatformDiagnosticsNotes() {
        if (OperatingSystem.IsLinux()) {
            return "Linux remote inspector server is process-scoped in WebKitGTK.";
        }

        if (OperatingSystem.IsMacOS()) {
            return "Web inspector is Safari attach mode and differs from remote TCP endpoint debugging.";
        }

        return null;
    }

    [SupportedOSPlatformGuard("windows")]
    [SupportedOSPlatformGuard("linux")]
    private static bool IsRemoteDebuggingPlatform() =>
        OperatingSystem.IsWindows() || OperatingSystem.IsLinux();

    private static string? GetBrowserRuntimeIdentity() {
        if (OperatingSystem.IsWindows()) {
            return InfiniFrameNative.GetWebView2RuntimeVersion();
        }

        if (OperatingSystem.IsLinux()) {
            return "WebKitGTK";
        }

        if (OperatingSystem.IsMacOS()) {
            return "WKWebView";
        }

        return null;
    }
}
