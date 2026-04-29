// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Interop;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal readonly record struct InteropEnvelopeParseResult(
    bool Success,
    bool IsIgnored,
    string? MessageId,
    string? Payload,
    string? Command,
    string? RequestId,
    string? Error
)
{
    public static InteropEnvelopeParseResult Ignored => new(false, true, null, null, null, null, null);

    public static InteropEnvelopeParseResult CreateSuccess(
        string messageId,
        string? payload,
        string? command = null,
        string? requestId = null
    )
        => new(true, false, messageId, payload, command, requestId, null);

    public static InteropEnvelopeParseResult CreateFailure(string error)
        => new(false, false, null, null, null, null, error);
}