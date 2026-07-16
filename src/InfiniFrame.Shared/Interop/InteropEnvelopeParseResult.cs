// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.Interop;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Represents the result of parsing an interop envelope from the JavaScript bridge.
/// </summary>
/// <param name="MessageId">The message identifier.</param>
/// <param name="Payload">The message payload.</param>
/// <param name="Command">The command associated with the message.</param>
/// <param name="RequestId">The request identifier for request/response patterns.</param>
/// <param name="Error">The error message, if the parse failed.</param>
internal readonly record struct InteropEnvelopeParseResult(
    string? MessageId,
    string? Payload,
    string? Command,
    string? RequestId,
    string? Error
) {
    internal required ResultState Result { get; init; }
    /// <summary>
    ///     Gets whether the parse result indicates success.
    /// </summary>
    public bool IsSuccess => Result == ResultState.Success;
    /// <summary>
    ///     Gets whether the parse result indicates failure.
    /// </summary>
    public bool IsFailure => Result == ResultState.Failure;
    /// <summary>
    ///     Gets whether the parse result was ignored.
    /// </summary>
    public bool IsIgnored => Result == ResultState.Ignored;
    /// <summary>
    ///     Gets whether the parse result is a Blazor message.
    /// </summary>
    public bool IsBlazor => Result == ResultState.Blazor;

    /// <summary>
    ///     Gets a pre-built instance representing an ignored message.
    /// </summary>
    public static InteropEnvelopeParseResult Ignored => new() { Result = ResultState.Ignored };
    /// <summary>
    ///     Gets a pre-built instance representing a Blazor message.
    /// </summary>
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
    /// <summary>
    ///     Creates a successful parse result with the specified values.
    /// </summary>
    /// <param name="messageId">The message identifier.</param>
    /// <param name="payload">The message payload.</param>
    /// <param name="command">The command associated with the message.</param>
    /// <param name="requestId">The request identifier for request/response patterns.</param>
    /// <returns>A new <see cref="InteropEnvelopeParseResult" /> indicating success.</returns>
    public static InteropEnvelopeParseResult CreateSuccess(
        string messageId,
        string? payload,
        string? command = null,
        string? requestId = null
    )
        => new(messageId, payload, command, requestId, null) { Result = ResultState.Success };

    /// <summary>
    ///     Creates a failure parse result with the specified error message.
    /// </summary>
    /// <param name="error">The error message describing the parse failure.</param>
    /// <returns>A new <see cref="InteropEnvelopeParseResult" /> indicating failure.</returns>
    public static InteropEnvelopeParseResult CreateFailure(string error)
        => new(null, null, null, null, error) { Result = ResultState.Failure };
}
