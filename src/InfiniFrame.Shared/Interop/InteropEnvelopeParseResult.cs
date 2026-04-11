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
    string? Error
) {
    public static InteropEnvelopeParseResult CreateSuccess(string messageId, string? payload)
        => new(
            Success: true,
            MessageId: messageId,
            Payload: payload,
            Error: null
        );

    public static InteropEnvelopeParseResult CreateFailure(string error)
        => new(
            Success: false,
            MessageId: null,
            Payload: null,
            Error: error
        );
}
