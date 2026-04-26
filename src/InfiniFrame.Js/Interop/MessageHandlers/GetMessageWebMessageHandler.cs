// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Interop;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace InfiniFrame.Js.Interop.MessageHandlers;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class GetMessageWebMessageHandler {
    public static T RegisterGetMessageWebMessageHandler<T>(this T builder) where T : class, IInfiniFrameWindowBuilder {
        RegisterWindowCreatedUtility.RegisterMessageHandler(builder, HandlerNames.GetMessageRequest, HandleGetMessageRequest);
        return builder;
    }

    private static void HandleGetMessageRequest(IInfiniFrameWindow window, string? payload) {
        if (!TryParseRequestPayload(payload, out string? requestId, out string? message, out string error)) {
            SendError(window, requestId, error);
            return;
        }

        if (window.ServiceProvider is null) {
            SendError(window, requestId, "Cannot resolve getMessage handler: no IServiceProvider was supplied.");
            return;
        }

        if (window.ServiceProvider.GetService(typeof(IInfiniFrameGetMessageService)) is not IInfiniFrameGetMessageService getMessageService) {
            SendError(
                window,
                requestId,
                "No IInfiniFrameGetMessageService is registered. Call services.AddInfiniFrameJs() before Build(provider)."
            );
            return;
        }

        InteropEnvelopeParseResult parseResult = InteropEnvelopeProtocol.ParseIncomingMessage(message!);
        if (!parseResult.Success) {
            SendError(window, requestId, $"Rejected getMessage request: {parseResult.Error ?? "Invalid envelope."}");
            return;
        }

        string messageId = parseResult.MessageId!;
        string? messagePayload = parseResult.Payload;

        try {
            if (!getMessageService.TryHandle(window, messageId, messagePayload, out string? responsePayload)) {
                SendError(window, requestId, $"No getMessage handler is registered for message ID '{messageId}'.");
                return;
            }

            SendSuccess(window, requestId!, responsePayload);
        }
        catch (Exception ex) when (IsNonFatalException(ex)) {
            window.Logger.LogError(ex, "Unhandled exception while processing getMessage request '{MessageId}'.", messageId);
            SendError(window, requestId, $"Unhandled exception while processing '{messageId}'.");
        }
    }

    private static bool TryParseRequestPayload(string? payload, out string? requestId, out string? message, out string error) {
        requestId = null;
        message = null;
        error = "Invalid getMessage request payload.";

        if (string.IsNullOrWhiteSpace(payload)) {
            error = "getMessage request payload is missing.";
            return false;
        }

        try {
            using JsonDocument document = JsonDocument.Parse(payload);
            JsonElement root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object) {
                error = "getMessage payload must be a JSON object.";
                return false;
            }

            if (!root.TryGetProperty("requestId", out JsonElement requestIdElement) || requestIdElement.ValueKind != JsonValueKind.String) {
                error = "getMessage payload requires a non-empty 'requestId' string.";
                return false;
            }

            requestId = requestIdElement.GetString();
            if (string.IsNullOrWhiteSpace(requestId)) {
                error = "getMessage payload requires a non-empty 'requestId' string.";
                return false;
            }

            if (!root.TryGetProperty("message", out JsonElement messageElement)) {
                error = "getMessage payload requires a 'message' field.";
                return false;
            }

            message = messageElement.ValueKind == JsonValueKind.String
                ? messageElement.GetString()
                : messageElement.GetRawText();

            if (string.IsNullOrWhiteSpace(message)) {
                error = "getMessage payload contains an empty 'message'.";
                return false;
            }

            return true;
        }
        catch (JsonException) {
            error = "getMessage payload JSON is malformed.";
            return false;
        }
    }

    private static void SendSuccess(IInfiniFrameWindow window, string requestId, string? data) {
        string responsePayloadJson = JsonSerializer.Serialize(new {
            requestId,
            success = true,
            data
        });
        string responseEnvelope = InteropEnvelopeProtocol.CreateEnvelopeMessage(HandlerNames.GetMessageResponse, responsePayloadJson);
        window.SendWebMessage(responseEnvelope);
    }

    private static void SendError(IInfiniFrameWindow window, string? requestId, string error) {
        string responsePayloadJson = JsonSerializer.Serialize(new {
            requestId,
            success = false,
            error
        });
        string responseEnvelope = InteropEnvelopeProtocol.CreateEnvelopeMessage(HandlerNames.GetMessageResponse, responsePayloadJson);
        window.SendWebMessage(responseEnvelope);
    }

    private static bool IsNonFatalException(Exception exception)
        => exception is not (OutOfMemoryException or AccessViolationException);
}
