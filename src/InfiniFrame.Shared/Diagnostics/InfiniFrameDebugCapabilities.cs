// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed record InfiniFrameDebugCapabilities {
    public required bool SupportsLocalDevTools { get; init; }
    public required bool SupportsRemoteDebuggingEndpoint { get; init; }
    public required bool SupportsWebInspectorAttach { get; init; }
    public required bool SupportsScriptErrorForwarding { get; init; }
    public required bool SupportsNavigationDiagnostics { get; init; }
}
