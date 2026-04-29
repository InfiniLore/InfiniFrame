// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Interop;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal readonly record struct InteropEnvelopeParseResult(
    bool Success,
    string? MessageId,
    string? Payload,
    string? Command,
    string? RequestId,
    string? Error
) {
    public static InteropEnvelopeParseResult CreateSuccess(
        string messageId,
        string? payload,
        string? command = null,
        string? requestId = null
    )
        => new(
            true,
            messageId,
            payload,
            command,
            requestId,
            null
        );

    public static InteropEnvelopeParseResult CreateFailure(string error)
        => new(
            false,
            null,
            null,
            null,
            null,
            error
        );
}
