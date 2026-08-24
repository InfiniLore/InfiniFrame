// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Debugging;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Provides diagnostic information about the current debug configuration.
/// </summary>
public sealed record InfiniFrameDebugDiagnostics {
    /// <summary>
    ///     Gets the platform identifier (e.g., Windows, macOS, Linux).
    /// </summary>
    public required string Platform { get; init; }
    /// <summary>
    ///     Gets the runtime identifier.
    /// </summary>
    public required string Runtime { get; init; }
    /// <summary>
    ///     Gets the browser runtime version, if available.
    /// </summary>
    public string? BrowserRuntime { get; init; }

    /// <summary>
    ///     Gets the debugging capabilities supported by the platform.
    /// </summary>
    public required InfiniFrameDebugCapabilities Capabilities { get; init; }

    /// <summary>
    ///     Gets whether DevTools are enabled.
    /// </summary>
    public required bool DevToolsEnabled { get; init; }
    /// <summary>
    ///     Gets the remote debugging port, if configured.
    /// </summary>
    public required int? RemoteDebuggingPort { get; init; }
    /// <summary>
    ///     Gets whether the web inspector is enabled.
    /// </summary>
    public required bool WebInspectorEnabled { get; init; }

    /// <summary>
    ///     Gets the status of the debug endpoint.
    /// </summary>
    public required InfiniFrameDebugEndpointStatus EndpointStatus { get; init; }
    /// <summary>
    ///     Gets the debug endpoint URI, if available.
    /// </summary>
    public Uri? Endpoint { get; init; }
    /// <summary>
    ///     Gets the reason for the endpoint status, if applicable.
    /// </summary>
    public string? EndpointReason { get; init; }
    /// <summary>
    ///     Gets whether the associated window is closed.
    /// </summary>
    public required bool IsWindowClosed { get; init; }
    /// <summary>
    ///     Gets platform-specific notes about the debug configuration.
    /// </summary>
    public string? PlatformNotes { get; init; }

    /// <summary>Gets the most recently observed lifecycle state.</summary>
    public InfiniFrameWindowLifecycleState LifecycleState { get; init; }

    /// <summary>Gets the UTC time of the most recent lifecycle transition.</summary>
    public DateTimeOffset LastLifecycleTransitionUtc { get; init; }

    /// <summary>Gets operations that have not reached a terminal native callback.</summary>
    public IReadOnlyList<InfiniFrameOperationDiagnostics> OutstandingOperations { get; init; } = [];

    /// <summary>Gets the most recently completed operation, including its terminal reason.</summary>
    public InfiniFrameOperationDiagnostics? LastOperation { get; init; }
}
