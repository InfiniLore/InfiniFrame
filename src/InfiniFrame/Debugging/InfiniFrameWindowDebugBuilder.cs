// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;
using InfiniFrame.Utilities;
using System.Runtime.Versioning;

namespace InfiniFrame.Debugging;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class InfiniFrameWindowDebugBuilder : IInfiniFrameWindowDebugBuilder {
    public bool SupportsRemoteDebuggingEndpoint => RemoteDebuggingUtility.IsSupportedPlatform();
    public bool SupportsWebInspectorAttach => WebInspectorUtility.IsSupportedPlatform();

    public bool DevToolsEnabled { get; private set; } = true;
    public bool WebInspectorEnabled { get; private set; }
    public int? RemoteDebuggingPort { get; private set; }
    
    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    internal InfiniFrameWindowDebugBuilder() { }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    internal void ApplyStartupDebugSettings(ref InfiniFrameNativeParameters parameters) {
        int? normalizedRemoteDebuggingPort = RemoteDebuggingUtility.NormalizePort(
            RemoteDebuggingPort,
            nameof(RemoteDebuggingPort));

        RemoteDebuggingUtility.EnsureSupportedPlatform(normalizedRemoteDebuggingPort);
        if (WebInspectorEnabled) {
            WebInspectorUtility.ThrowIfUnsupported();
        }

        parameters.DevToolsEnabled = DevToolsEnabled;
        parameters.WebInspectorEnabled = WebInspectorEnabled;
        parameters.RemoteDebuggingPort = normalizedRemoteDebuggingPort ?? 0;
        parameters.BrowserControlInitParameters = RemoteDebuggingUtility.ComposeBrowserControlInitParameters(
            parameters.BrowserControlInitParameters,
            normalizedRemoteDebuggingPort);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public void SetDevToolsEnabled(bool enabled) {
        DevToolsEnabled = enabled;
    }

    [SupportedOSPlatform("macos13.3")]
    public void SetWebInspectorEnabled(bool enabled = true) {
        if (enabled) {
            WebInspectorUtility.ThrowIfUnsupported();
        }

        WebInspectorEnabled = enabled;
    }

    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    public void SetRemoteDebuggingPort(int? port) {
        int? normalized = RemoteDebuggingUtility.NormalizePort(port);
        RemoteDebuggingUtility.EnsureSupportedPlatform(normalized);
        RemoteDebuggingPort = normalized;
    }
}
