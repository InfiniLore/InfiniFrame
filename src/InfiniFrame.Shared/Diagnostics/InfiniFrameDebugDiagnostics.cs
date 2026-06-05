// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;

public sealed record InfiniFrameDebugDiagnostics {
    public required string Platform { get; init; }
    public required string Runtime { get; init; }
    public string? BrowserRuntime { get; init; }

    public required InfiniFrameDebugCapabilities Capabilities { get; init; }

    public required bool DevToolsEnabled { get; init; }
    public required int? RemoteDebuggingPort { get; init; }
    public required bool WebInspectorEnabled { get; init; }

    public required InfiniFrameDebugEndpointStatus EndpointStatus { get; init; }
    public Uri? Endpoint { get; init; }
    public string? EndpointReason { get; init; }

    public required string LastDebugInitializationStatus { get; init; }
    public string? LastDebugInitializationError { get; init; }
    public required bool IsWindowClosed { get; init; }
    public string? PlatformNotes { get; init; }
}
