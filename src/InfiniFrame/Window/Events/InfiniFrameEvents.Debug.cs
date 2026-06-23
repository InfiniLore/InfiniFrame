// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Debugging;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public partial class InfiniFrameEvents {
    /// <summary>
    ///     Raises the <see cref="IInfiniFrameEventsStore.DebuggingEvent" /> when the native window reports a debug event.
    /// </summary>
    /// <param name="kind">The kind of debug event (e.g., "Console", "Runtime").</param>
    /// <param name="message">The debug message content.</param>
    /// <param name="level">The severity level (e.g., "info", "warning", "error").</param>
    /// <param name="uri">The URI associated with the event, if any.</param>
    /// <param name="statusCode">An optional HTTP or status code.</param>
    /// <param name="timestampUnixMillisecondsUtc">The event timestamp in Unix milliseconds UTC.</param>
    /// <param name="platformPayload">Optional platform-specific payload data.</param>
    public void OnDebugEvent(
        string kind,
        string? message,
        string? level,
        string? uri,
        int statusCode,
        long timestampUnixMillisecondsUtc,
        string? platformPayload
    ) {
        ArgumentNullException.ThrowIfNull(Sender);

        if (!Enum.TryParse(kind, ignoreCase: true, out InfiniFrameDebugEventKind parsedKind)) {
            parsedKind = InfiniFrameDebugEventKind.Runtime;
        }

        DateTime timestampUtc = timestampUnixMillisecondsUtc > 0
            ? DateTimeOffset.FromUnixTimeMilliseconds(timestampUnixMillisecondsUtc).UtcDateTime
            : DateTime.UtcNow;

        EventsStore.DebuggingEvent.Invoke(Sender, new InfiniFrameDebugEventArgs {
            Kind = parsedKind,
            Message = string.IsNullOrWhiteSpace(message) ? null : message,
            Level = string.IsNullOrWhiteSpace(level) ? null : level,
            Uri = string.IsNullOrWhiteSpace(uri) ? null : uri,
            StatusCode = statusCode != 0 ? statusCode : null,
            TimestampUtc = timestampUtc,
            PlatformPayload = string.IsNullOrWhiteSpace(platformPayload) ? null : platformPayload
        });
    }
}
