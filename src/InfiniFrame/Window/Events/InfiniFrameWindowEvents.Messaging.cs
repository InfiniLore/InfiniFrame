// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Interop;
using InfiniFrame.Js;
using InfiniFrame.Js.Interop;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public partial class InfiniFrameWindowEvents {

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window sends a message.
    /// </summary>
    public void OnWebMessageReceived(string message, string? origin = null) {
        if (!SetupComplete) throw new InvalidOperationException("Setup not complete");

        ArgumentNullException.ThrowIfNull(message);

        if (Sender.InstanceHandle == IntPtr.Zero) {
            Sender.Logger.LogDebug("Skipping web message handling because window is closed.");
            return;
        }

        if (string.IsNullOrWhiteSpace(message)) {
            Sender.Logger.LogDebug("Rejected empty web message.");
            return;
        }

        InteropEnvelopeParseResult parseResult = InteropEnvelopeProtocol.ParseIncomingMessage(message);
        switch (parseResult) {
            case { IsBlazor: true }: {
                // Blazor messages are handled by the Blazor WebView
                EventsStore.WebMessageReceived.Invoke(Sender, new InfiniFrameWebMessageReceivedEvent(message, origin));
                return;
            }

            case { IsIgnored: true }: {
                Sender.Logger.LogDebug("Ignored web message with ID '{messageId}' due to parsing rules. Defaulting to WebMessageReceived", parseResult.MessageId);
                EventsStore.WebMessageReceived.Invoke(Sender, new InfiniFrameWebMessageReceivedEvent(message, origin));
                return;
            }

            case { IsSuccess: false }: {
                Sender.Logger.LogWarning("Rejected invalid web message: {Reason}", parseResult.Error ?? "Unknown error");
                return;
            }
        }

        string messageId = parseResult.MessageId!;

        switch (parseResult.Command) {
            case InteropEnvelopeProtocol.PostCommand: {
                try {
                    if (!TryHandlePostDataRequest(Sender, message)) Sender.Logger.LogWarning("Failed to handle post data request for message ID '{messageId}'", messageId);
                    return;
                }
                catch (Exception ex) when (IsNonFatalException(ex)) {
                    Sender.Logger.LogError(ex, "Unhandled exception while processing getMessage request '{MessageId}'.", messageId);
                    return;
                }
            }

            case InteropEnvelopeProtocol.GetCommand: {
                try {
                    if (!TryHandleGetDataRequest(Sender, message, out string? responsePayload)) {
                        SendError(Sender, parseResult.RequestId, $"No getMessage handler is registered for message ID '{messageId}'.");
                        return;
                    }

                    SendSuccess(Sender, parseResult.RequestId, responsePayload);
                    return;
                }
                catch (Exception ex) when (IsNonFatalException(ex)) {
                    Sender.Logger.LogError(ex, "Unhandled exception while processing getMessage request '{MessageId}'.", messageId);
                    SendError(Sender, parseResult.RequestId, $"Unhandled exception while processing '{messageId}'.");
                    return;
                }
            }

            default:
                Sender.Logger.LogWarning("Unhandled command '{command}' for message ID '{messageId}'", parseResult.Command, messageId);
                return;
        }
    }

    private bool TryHandlePostDataRequest(IInfiniFrameWindow window, string message) {
        if (string.IsNullOrWhiteSpace(message)) return false;

        // ReSharper disable once UseDeconstruction
        InteropEnvelopeParseResult parseResult = InteropEnvelopeProtocol.ParseIncomingMessage(message);
        if (parseResult == InteropEnvelopeParseResult.Ignored) return false;

        if (!parseResult.IsSuccess) {
            window.Logger.LogWarning("Rejected invalid web message: {Reason}", parseResult.Error ?? "Unknown error");
            return false;
        }

        string messageId = parseResult.MessageId!;
        string? payload = parseResult.Payload;

        if (!string.Equals(parseResult.Command, InteropEnvelopeProtocol.PostCommand, StringComparison.Ordinal))
            return false;

        try {
            return EventsStore.WebMessagePostData.TryInvoke(messageId, Sender!, payload);
        }
        catch (Exception ex) when (IsNonFatalException(ex)) {
            window.Logger.LogError(ex, "Unhandled exception while processing web message '{Message}'", message);
            return false;
        }
    }

    private bool TryHandleGetDataRequest(IInfiniFrameWindow window, string message, out string? response) {
        response = null;

        if (string.IsNullOrWhiteSpace(message)) return false;

        // ReSharper disable once UseDeconstruction
        InteropEnvelopeParseResult parseResult = InteropEnvelopeProtocol.ParseIncomingMessage(message);
        if (parseResult.IsIgnored) return false;

        if (!parseResult.IsSuccess) {
            window.Logger.LogWarning("Rejected invalid web message: {Reason}", parseResult.Error ?? "Unknown error");
            return false;
        }

        string messageId = parseResult.MessageId!;
        string? payload = parseResult.Payload;

        if (!string.Equals(parseResult.Command, InteropEnvelopeProtocol.GetCommand, StringComparison.Ordinal))
            return false;

        try {
            return EventsStore.WebMessageGetData.TryInvoke(messageId, Sender!, payload, out response);
        }
        catch (Exception ex) when (IsNonFatalException(ex)) {
            window.Logger.LogError(ex, "Unhandled exception while processing web message '{MessageId}'", messageId);
            return false;
        }
    }

    private static bool IsNonFatalException(Exception exception)
        => exception is not (OutOfMemoryException or AccessViolationException);



    private static void SendSuccess(IInfiniFrameWindow window, string? requestId, string? data) {
        string responsePayloadJson = JsonSerializer.Serialize(
            new InteropGetMessageSuccessResponse {
                RequestId = requestId,
                Success = true,
                Data = data
            },
            InteropGetMessageJsonContext.Default.InteropGetMessageSuccessResponse
        );
        string responseEnvelope = InteropEnvelopeProtocol.CreateEnvelopeMessage(
            HandlerNames.GetResponse,
            responsePayloadJson,
            InteropEnvelopeProtocol.GetCommand,
            requestId
        );
        window.SendWebMessage(responseEnvelope);
    }

    private static void SendError(IInfiniFrameWindow window, string? requestId, string error) {
        string responsePayloadJson = JsonSerializer.Serialize(
            new InteropGetMessageErrorResponse {
                RequestId = requestId,
                Success = false,
                Error = error
            },
            InteropGetMessageJsonContext.Default.InteropGetMessageErrorResponse
        );
        string responseEnvelope = InteropEnvelopeProtocol.CreateEnvelopeMessage(
            HandlerNames.GetResponse,
            responsePayloadJson,
            InteropEnvelopeProtocol.GetCommand,
            requestId
        );
        window.SendWebMessage(responseEnvelope);
    }
}
