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
    string? Error,
    bool IsLegacyProtocol
) {
    public static InteropEnvelopeParseResult CreateSuccess(string messageId, string? payload, bool isLegacyProtocol = false)
        => new(
            Success: true,
            MessageId: messageId,
            Payload: payload,
            Error: null,
            IsLegacyProtocol: isLegacyProtocol
        );

    public static InteropEnvelopeParseResult CreateFailure(string error)
        => new(
            Success: false,
            MessageId: null,
            Payload: null,
            Error: error,
            IsLegacyProtocol: false
        );
}
