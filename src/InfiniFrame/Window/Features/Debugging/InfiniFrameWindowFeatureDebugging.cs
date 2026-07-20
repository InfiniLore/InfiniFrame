// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Debugging;
using InfiniFrame.NativeBridge;
using InfiniFrame.Utilities;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class InfiniFrameWindowFeatureDebugging(
    IInfiniFrameWindow window,
    ILogger<InfiniFrameWindowFeatureDebugging> logger
) : IInfiniFrameWindowFeatureDebugging {

    /// <inheritdoc cref="IInfiniFrameWindowFeatureDebugging.IsDevToolsEnabled" />
    public bool IsDevToolsEnabled => NativeInvoke.InvokeSyncWithValidation<bool>(
        logger,
        window,
        window.ManagedThreadId,
        InfiniFrameNative.GetDevToolsEnabled
    );

    /// <inheritdoc cref="IInfiniFrameWindowFeatureDebugging.SupportsWebInspectorAttach" />
    public bool SupportsWebInspectorAttach => MacOsWebInspectorUtility.IsSupportedPlatform();

    /// <inheritdoc cref="IInfiniFrameWindowFeatureDebugging.IsWebInspectorEnabled" />
    public bool IsWebInspectorEnabled => window.Configuration.StartupParameters.WebInspectorEnabled;

    /// <inheritdoc cref="IInfiniFrameWindowFeatureDebugging.SupportsRemoteDebuggingEndpoint" />
    public bool SupportsRemoteDebuggingEndpoint => RemoteDebuggingUtility.IsSupportedPlatform();

    /// <inheritdoc cref="IInfiniFrameWindowFeatureDebugging.RemoteDebuggingPort" />
    public int? RemoteDebuggingPort => window.Configuration.StartupParameters.RemoteDebuggingPort > 0
        ? window.Configuration.StartupParameters.RemoteDebuggingPort
        : null;

    /// <inheritdoc cref="IInfiniFrameWindowFeatureDebugging.Capabilities" />
    public InfiniFrameDebugCapabilities Capabilities => new() {
        SupportsLocalDevTools = true,
        SupportsRemoteDebuggingEndpoint = SupportsRemoteDebuggingEndpoint,
        SupportsWebInspectorAttach = SupportsWebInspectorAttach,
        SupportsScriptErrorForwarding = true,
        SupportsNavigationDiagnostics = true
    };

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="IInfiniFrameWindowFeatureDebugging.EnableDevTools" />
    public void EnableDevTools(bool enabled) {
        logger.LogDebug(".Debug.SetDevToolsEnabled({Enabled})", enabled);

        bool originalValue = NativeInvoke.InvokeSyncWithValidation<bool>(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.GetDevToolsEnabled
        );

        if (originalValue == enabled) return;

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.SetDevToolsEnabled,
            enabled
        );
    }

    /// <inheritdoc cref="IInfiniFrameWindowFeatureDebugging.TryGetRemoteDebuggingEndpoint" />
    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    public bool TryGetRemoteDebuggingEndpoint(out Uri? endpoint) {
        endpoint = null;
        if (!SupportsRemoteDebuggingEndpoint) return false;

        int? port = RemoteDebuggingPort;
        if (!port.HasValue || window.IsClosedOrClosing())
            return false;

        endpoint = RemoteDebuggingUtility.CreateEndpointUri(port.Value);
        return true;
    }

    /// <inheritdoc cref="IInfiniFrameWindowFeatureDebugging.TryProbeEndpoint" />
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
            reason = window.IsClosedOrClosing()
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

    /// <inheritdoc cref="IInfiniFrameWindowFeatureDebugging.GetDiagnostics" />
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
        else if (window.IsClosedOrClosing() || !TryGetRemoteDebuggingEndpoint(out endpoint) || endpoint is null) {
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
            DevToolsEnabled = IsDevToolsEnabled,
            RemoteDebuggingPort = RemoteDebuggingPort,
            WebInspectorEnabled = IsWebInspectorEnabled,
            EndpointStatus = endpointStatus,
            Endpoint = endpoint,
            EndpointReason = endpointReason,
            IsWindowClosed = window.IsClosedOrClosing(),
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
