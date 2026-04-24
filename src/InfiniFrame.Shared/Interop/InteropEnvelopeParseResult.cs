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
            true,
            messageId,
            payload,
            null,
            isLegacyProtocol
        );

    public static InteropEnvelopeParseResult CreateFailure(string error)
        => new(
            false,
            null,
            null,
            error,
            false
        );
}
