// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Debugging;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public partial class InfiniFrameEvents {
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
