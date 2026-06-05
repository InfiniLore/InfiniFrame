// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge;
using InfiniFrame.Utilities;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

namespace InfiniFrame.Debugging;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class InfiniFrameWindowDebug : IInfiniFrameWindowDebug {
    private readonly InfiniFrameWindow _window;
    private readonly Dictionary<EventHandler<InfiniFrameDebugEventArgs>, Action<IInfiniFrameWindow, InfiniFrameDebugEventArgs>> _eventHandlers = [];

    #if NET9_0_OR_GREATER
    private readonly Lock _eventHandlersLock = new();
    #else
    private readonly object _eventHandlersLock = new();
    #endif
    
    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    internal InfiniFrameWindowDebug(InfiniFrameWindow window) {
        _window = window;
    }

    public event EventHandler<InfiniFrameDebugEventArgs>? Event {
        add {
            if (value is null) {
                return;
            }

            lock (_eventHandlersLock) {
                if (_eventHandlers.ContainsKey(value)) {
                    return;
                }

                void Bridge(IInfiniFrameWindow sender, InfiniFrameDebugEventArgs args) {
                    value(sender, args);
                }

                _eventHandlers[value] = Bridge;
                _window.EventsStore.DebugEvent.Add(Bridge);
            }
        }
        remove {
            if (value is null) {
                return;
            }

            lock (_eventHandlersLock) {
                if (!_eventHandlers.Remove(value, out Action<IInfiniFrameWindow, InfiniFrameDebugEventArgs>? bridge)) {
                    return;
                }

                _window.EventsStore.DebugEvent.Remove(bridge);
            }
        }
    }

    public bool DevToolsEnabled => InvokeUtility.InvokeAndReturn<bool, InfiniFrameNativeInteropStatus>(
        _window,
        InfiniFrameNative.GetDevToolsEnabled,
        validateResult: s => InfiniFrameNative.EnsureSucceeded(s, nameof(InfiniFrameNative.GetDevToolsEnabled)));
    public bool SupportsWebInspector => WebInspectorUtility.IsSupportedPlatform();
    public bool WebInspectorEnabled => _window.Configuration.StartupParameters.WebInspectorEnabled;
    public bool SupportsRemoteDebugging => RemoteDebuggingUtility.IsSupportedPlatform();
    public int? RemoteDebuggingPort => _window.Configuration.StartupParameters.RemoteDebuggingPort > 0
        ? _window.Configuration.StartupParameters.RemoteDebuggingPort
        : null;

    public InfiniFrameDebugCapabilities Capabilities => new() {
        SupportsLocalDevTools = true,
        SupportsRemoteDebuggingEndpoint = SupportsRemoteDebugging,
        SupportsWebInspectorAttach = SupportsWebInspector,
        SupportsScriptErrorForwarding = true,
        SupportsNavigationDiagnostics = true
    };

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public void SetDevToolsEnabled(bool enabled) {
        _window.Logger.LogDebug(".Debug.SetDevToolsEnabled({Enabled})", enabled);

        _window.Invoke(() => {
            InfiniFrameNative.GetDevToolsEnabled(_window.InstanceHandle, out bool isEnabled);
            if (isEnabled == enabled) return;

            InfiniFrameNative.SetDevToolsEnabled(_window.InstanceHandle, enabled);
        });
    }

    public void SetWebInspectorEnabled(bool enabled = true) {
        if (enabled && !SupportsWebInspector) {
            throw new PlatformNotSupportedException("Web inspector mode is only supported on macOS 13.3+ in InfiniFrame.");
        }

        throw new InvalidOperationException("WebInspectorEnabled is startup-only. Configure it with builder.SetWebInspectorEnabled(...) before Build().");
    }

    public bool TryGetRemoteDebuggingEndpoint(out Uri? endpoint) {
        endpoint = null;
        if (!SupportsRemoteDebugging) {
            throw new PlatformNotSupportedException("Remote debugging is only supported on Windows and Linux in InfiniFrame.");
        }

        int? port = RemoteDebuggingPort;
        if (!port.HasValue || _window.IsClosedOrClosing)
            return false;

        endpoint = RemoteDebuggingUtility.CreateEndpointUri(port.Value);
        return true;
    }

    public bool TryProbeEndpoint(out Uri? endpoint, out string? reason) {
        endpoint = null;
        reason = null;

        if (!Capabilities.SupportsRemoteDebuggingEndpoint) {
            reason = "Remote debugging endpoint probing is not supported on this platform.";
            _window.Logger.LogDebug("Debug endpoint probe skipped: {Reason}", reason);
            return false;
        }

        if (!TryGetRemoteDebuggingEndpoint(out endpoint) || endpoint is null) {
            reason = _window.IsClosedOrClosing
                ? "Window is closed."
                : "Remote debugging is disabled.";
            _window.Logger.LogDebug("Debug endpoint probe unavailable: {Reason}", reason);
            return false;
        }

        if (RemoteDebuggingUtility.TryProbeEndpoint(endpoint, out string? probeFailure)) {
            _window.Logger.LogDebug("Debug endpoint probe succeeded for {Endpoint}", endpoint);
            return true;
        }

        reason = probeFailure;
        _window.Logger.LogWarning("Debug endpoint probe failed for {Endpoint}: {Reason}", endpoint, reason);
        return false;
    }

    public InfiniFrameDebugDiagnostics GetDiagnostics() {
        Uri? endpoint = null;
        string? endpointReason = null;
        InfiniFrameDebugEndpointStatus endpointStatus;
        if (!Capabilities.SupportsRemoteDebuggingEndpoint) {
            endpointStatus = InfiniFrameDebugEndpointStatus.NotSupported;
        }
        else if (!RemoteDebuggingPort.HasValue) {
            endpointStatus = InfiniFrameDebugEndpointStatus.Disabled;
            endpointReason = "Remote debugging is disabled.";
        }
        else if (_window.IsClosedOrClosing || !TryGetRemoteDebuggingEndpoint(out endpoint) || endpoint is null) {
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
            LastDebugInitializationStatus = _window.LastDebugInitializationStatus,
            LastDebugInitializationError = _window.LastDebugInitializationError,
            IsWindowClosed = _window.IsClosedOrClosing,
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
