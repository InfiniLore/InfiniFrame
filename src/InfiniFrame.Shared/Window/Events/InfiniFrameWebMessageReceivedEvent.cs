// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Represents a web message received from the browser control.
/// </summary>
/// <param name="Message">The message content.</param>
/// <param name="Origin">The origin of the message, if available.</param>
public readonly record struct InfiniFrameWebMessageReceivedEvent(
    string Message,
    string? Origin
);
