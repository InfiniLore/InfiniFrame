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
    ///     This overload carries the native-reported message origin in an ambient context.
    /// </summary>
    public void OnWebMessageReceived(string message, string? origin = null) {
        if (!SetupComplete) throw new InvalidOperationException("Setup not complete");
        ArgumentNullException.ThrowIfNull(message);
        
        if (string.IsNullOrWhiteSpace(message)) {
            Sender.Logger.LogDebug("Rejected empty web message.");
            return;
        }

        InteropEnvelopeParseResult parseResult = InteropEnvelopeProtocol.ParseIncomingMessage(message);
        if (parseResult.IsIgnored) return;
        
        if (!parseResult.Success) {
            Sender.Logger.LogWarning("Rejected invalid web message: {Reason}", parseResult.Error ?? "Unknown error");
            return;
        }

        string messageId = parseResult.MessageId!;

        if (string.Equals(parseResult.Command, InteropEnvelopeProtocol.PostCommand, StringComparison.Ordinal)) {
            TryHandlePostDataRequest(Sender, message);
            return;
        }

        if (!string.Equals(parseResult.Command, InteropEnvelopeProtocol.GetCommand, StringComparison.Ordinal)) {
            return;
        }

        try {
            if (TryHandleGetDataRequest(Sender, message, out string? responsePayload)) {
                SendSuccess(Sender, parseResult.RequestId, responsePayload);
                return;
            }

            SendError(Sender, parseResult.RequestId, $"No getMessage handler is registered for message ID '{messageId}'.");

        }
        catch (Exception ex) when (IsNonFatalException(ex)) {
            Sender.Logger.LogError(ex, "Unhandled exception while processing getMessage request '{MessageId}'.", messageId);
            SendError(Sender, parseResult.RequestId, $"Unhandled exception while processing '{messageId}'.");
        }
    }
    
    private bool TryHandlePostDataRequest(IInfiniFrameWindow window, string message) {
        if (string.IsNullOrWhiteSpace(message)) return false;

        // ReSharper disable once UseDeconstruction
        InteropEnvelopeParseResult parseResult = InteropEnvelopeProtocol.ParseIncomingMessage(message);
        if (parseResult == InteropEnvelopeParseResult.Ignored) return false;

        if (!parseResult.Success) {
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
        
        if (!parseResult.Success) {
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
