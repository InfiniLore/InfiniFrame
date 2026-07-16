// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Interop;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
internal readonly record struct InteropEnvelopeParseResult(
    string? MessageId,
    string? Payload,
    string? Command,
    string? RequestId,
    string? Error
) {
    internal required ResultState Result { get; init; }
    public bool IsSuccess => Result == ResultState.Success;
    public bool IsFailure => Result == ResultState.Failure;
    public bool IsIgnored => Result == ResultState.Ignored;
    public bool IsBlazor => Result == ResultState.Blazor;

    public static InteropEnvelopeParseResult Ignored => new() { Result = ResultState.Ignored };
    public static InteropEnvelopeParseResult BlazorMessage => new() { Result = ResultState.Blazor };

    internal enum ResultState {
        Success,
        Failure,
        Ignored,
        Blazor
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    public static InteropEnvelopeParseResult CreateSuccess(
        string messageId,
        string? payload,
        string? command = null,
        string? requestId = null
    )
        => new(messageId, payload, command, requestId, null) { Result = ResultState.Success };

    public static InteropEnvelopeParseResult CreateFailure(string error)
        => new(null, null, null, null, error) { Result = ResultState.Failure };
}
