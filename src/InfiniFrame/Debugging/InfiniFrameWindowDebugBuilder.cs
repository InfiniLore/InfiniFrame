// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Utilities;

namespace InfiniFrame.Debugging;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class InfiniFrameWindowDebugBuilder : IInfiniFrameWindowDebugBuilder {
    private readonly IInfiniFrameWindowBuilder _builder;

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    internal InfiniFrameWindowDebugBuilder(IInfiniFrameWindowBuilder builder) {
        _builder = builder;
    }

    public bool SupportsRemoteDebuggingEndpoint => RemoteDebuggingUtility.IsSupportedPlatform();
    public bool SupportsWebInspectorAttach => WebInspectorUtility.IsSupportedPlatform();

    public bool DevToolsEnabled {
        get => _builder.Configuration.DevToolsEnabled;
        set => _builder.Configuration.DevToolsEnabled = value;
    }

    public bool WebInspectorEnabled {
        get => _builder.Configuration.WebInspectorEnabled;
        set {
            if (value) {
                WebInspectorUtility.ThrowIfUnsupported();
            }

            _builder.Configuration.WebInspectorEnabled = value;
        }
    }

    public int? RemoteDebuggingPort {
        get => _builder.Configuration.RemoteDebuggingPort;
        set {
            int? normalized = RemoteDebuggingUtility.NormalizePort(value);
            RemoteDebuggingUtility.EnsureSupportedPlatform(normalized);
            _builder.Configuration.RemoteDebuggingPort = normalized;
        }
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public IInfiniFrameWindowBuilder SetDevToolsEnabled(bool enabled) {
        DevToolsEnabled = enabled;
        return _builder;
    }

    public IInfiniFrameWindowBuilder SetWebInspectorEnabled(bool enabled = true) {
        WebInspectorEnabled = enabled;
        return _builder;
    }

    public IInfiniFrameWindowBuilder SetRemoteDebuggingPort(int? port) {
        RemoteDebuggingPort = port;
        return _builder;
    }
}
