// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Interop;
using InfiniFrame.Utilities;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public partial class InfiniFrameEvents {

    /// <summary>
    ///     Invokes registered user-defined handler methods when the native window sends a message.
    /// </summary>
    public void OnWebMessageReceived(string message, string? origin = null) {
        ArgumentNullException.ThrowIfNull(Sender);
        ArgumentNullException.ThrowIfNull(message);

        if (Sender.LifecycleState != InfiniFrameWindowLifecycleState.Running) {
            Logger.LogDebug("Skipping web message handling because window is closed.");
            return;
        }

        if (string.IsNullOrWhiteSpace(message)) {
            Logger.LogDebug("Rejected empty web message.");
            return;
        }

        InteropEnvelopeParseResult parseResult = InteropEnvelopeProtocol.ParseIncomingMessage(message);

        switch (parseResult) {
            case { IsBlazor: true }:
                EventsStore.WebMessageReceived.Invoke(
                    Sender,
                    new InfiniFrameWebMessageReceivedEvent(message, origin)
                );
                return;

            case { IsIgnored: true }:
                Logger.LogDebug(
                    "Ignored web message with ID '{messageId}' due to parsing rules. Defaulting to WebMessageReceived",
                    parseResult.MessageId
                );
                EventsStore.WebMessageReceived.Invoke(
                    Sender,
                    new InfiniFrameWebMessageReceivedEvent(message, origin)
                );
                return;

            case { IsSuccess: false }:
                Logger.LogWarning(
                    "Rejected invalid web message: {Reason}",
                    parseResult.Error ?? "Unknown error"
                );
                return;
        }

        string messageId = parseResult.MessageId!;
        string? payload = parseResult.Payload;

        switch (parseResult.Command) {
            case InteropEnvelopeProtocol.PostCommand:
                try {
                    if (!EventsStore.WebMessagePostData.TryInvoke(messageId, Sender!, payload)) {
                        Logger.LogWarning(
                            "Failed to handle post data request for message ID '{messageId}'",
                            messageId
                        );
                    }
                }
                catch (Exception ex) when (ExceptionsUtility.IsNonFatalException(ex)) {
                    Logger.LogError(
                        ex,
                        "Unhandled exception while processing postMessage '{MessageId}'",
                        messageId
                    );
                }

                return;

            case InteropEnvelopeProtocol.GetCommand:
                try {
                    if (!EventsStore.WebMessageGetData.TryInvoke(messageId, Sender!, payload, out string? response)) {
                        SendError(Sender, parseResult.RequestId,
                            $"No getMessage handler is registered for message ID '{messageId}'.");
                        return;
                    }

                    SendSuccess(Sender, parseResult.RequestId, response);
                }
                catch (Exception ex) when (ExceptionsUtility.IsNonFatalException(ex)) {
                    Logger.LogError(
                        ex,
                        "Unhandled exception while processing getMessage '{MessageId}'",
                        messageId
                    );

                    SendError(Sender, parseResult.RequestId,
                        $"Unhandled exception while processing '{messageId}'.");
                }

                return;

            default:
                Logger.LogWarning(
                    "Unhandled command '{command}' for message ID '{messageId}'",
                    parseResult.Command,
                    messageId
                );
                return;
        }
    }

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
            JsHandlerNames.GetResponse,
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
            JsHandlerNames.GetResponse,
            responsePayloadJson,
            InteropEnvelopeProtocol.GetCommand,
            requestId
        );

        window.SendWebMessage(responseEnvelope);
    }
}
