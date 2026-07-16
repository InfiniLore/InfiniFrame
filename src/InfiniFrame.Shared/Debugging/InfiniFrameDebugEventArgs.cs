// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Debugging;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Provides data for debug events from the browser control.
/// </summary>
public sealed class InfiniFrameDebugEventArgs : EventArgs {
    /// <summary>
    ///     Gets the kind of debug event.
    /// </summary>
    public required InfiniFrameDebugEventKind Kind { get; init; }
    /// <summary>
    ///     Gets the debug message content.
    /// </summary>
    public string? Message { get; init; }
    /// <summary>
    ///     Gets the log level of the debug event.
    /// </summary>
    public string? Level { get; init; }
    /// <summary>
    ///     Gets the URI associated with the debug event.
    /// </summary>
    public string? Uri { get; init; }
    /// <summary>
    ///     Gets the HTTP status code associated with the debug event.
    /// </summary>
    public int? StatusCode { get; init; }
    /// <summary>
    ///     Gets the UTC timestamp of when the event occurred.
    /// </summary>
    public required DateTime TimestampUtc { get; init; }
    /// <summary>
    ///     Gets platform-specific payload data.
    /// </summary>
    public string? PlatformPayload { get; init; }
}
