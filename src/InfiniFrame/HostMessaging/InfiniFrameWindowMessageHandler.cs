// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.BuilderSnapshots;
using InfiniFrame.Interop;
using InfiniFrame.Js;
using InfiniFrame.Js.Interop;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Text.Json;

namespace InfiniFrame.HostMessaging;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowMessageHandler : IInfiniFrameWindowMessageHandler {

    private ConcurrentDictionary<string, Action<IInfiniFrameWindow, string?>> PostDataHandlers { get; } = new();
    private readonly ConcurrentDictionary<string, Func<IInfiniFrameWindow, string?, string?>> GetDataHandlers = new();

    public bool IsEmpty => PostDataHandlers.IsEmpty && GetDataHandlers.IsEmpty;
    public int Count => PostDataHandlers.Count + GetDataHandlers.Count;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public void RegisterHandler(string messageId, Action<IInfiniFrameWindow, string?> handler) {
        PostDataHandlers.AddOrUpdate(messageId, handler, updateValueFactory: (_, _) => handler);
    }

    public void RegisterHandler(string messageId, Func<IInfiniFrameWindow, string?, string?> handler) {
        GetDataHandlers.AddOrUpdate(messageId, handler, static (_, updated) => updated);
    }

    public bool TryHandlePostDataRequest(IInfiniFrameWindow window, string message) {
        if (IsEmpty) return false;
        if (string.IsNullOrWhiteSpace(message)) return false;

        // ReSharper disable once UseDeconstruction
        InteropEnvelopeParseResult parseResult = InteropEnvelopeProtocol.ParseIncomingMessage(message);
        if (!parseResult.Success) {
            window.Logger.LogWarning("Rejected invalid web message: {Reason}", parseResult.Error ?? "Unknown error");
            return false;
        }

        string messageId = parseResult.MessageId!;
        string? payload = parseResult.Payload;

        if (!string.Equals(parseResult.Command, InteropEnvelopeProtocol.PostCommand, StringComparison.Ordinal))
            return false;

        if (!PostDataHandlers.TryGetValue(messageId, out Action<IInfiniFrameWindow, string?>? handler)) return false;

        try {
            handler(window, payload);
            return true;
        }
        catch (Exception ex) when (IsNonFatalException(ex)) {
            window.Logger.LogError(ex, "Unhandled exception while processing web message '{Message}'", message);
            return false;
        }
    }

    public bool TryHandleGetDataRequest(IInfiniFrameWindow window, string message, out string? response) {
        response = null;

        if (IsEmpty) return false;
        if (string.IsNullOrWhiteSpace(message)) return false;

        // ReSharper disable once UseDeconstruction
        InteropEnvelopeParseResult parseResult = InteropEnvelopeProtocol.ParseIncomingMessage(message);
        if (!parseResult.Success) {
            window.Logger.LogWarning("Rejected invalid web message: {Reason}", parseResult.Error ?? "Unknown error");
            return false;
        }

        string messageId = parseResult.MessageId!;
        string? payload = parseResult.Payload;

        if (!string.Equals(parseResult.Command, InteropEnvelopeProtocol.GetCommand, StringComparison.Ordinal))
            return false;

        if (!GetDataHandlers.TryGetValue(messageId, out Func<IInfiniFrameWindow, string?, string?>? handler)) return false;

        try {
            response = handler(window, payload);
            return true;
        }
        catch (Exception ex) when (IsNonFatalException(ex)) {
            window.Logger.LogError(ex, "Unhandled exception while processing web message '{MessageId}'", messageId);
            return false;
        }
    }

    internal InfiniFrameWindowMessageHandlersSnapshot ToSnapshot()
        => new(PostDataHandlers.ToArray(), GetDataHandlers.ToArray());

    internal static InfiniFrameWindowMessageHandler FromSnapshot(InfiniFrameWindowMessageHandlersSnapshot snapshot) {
        var copy = new InfiniFrameWindowMessageHandler();

        foreach ((string key, Action<IInfiniFrameWindow, string?> value) in snapshot.PostDataHandlers) {
            copy.RegisterHandler(key, value);
        }

        foreach ((string key, Func<IInfiniFrameWindow, string?, string?> value) in snapshot.GetDataHandlers) {
            copy.RegisterHandler(key, value);
        }

        return copy;
    }

    private static bool IsNonFatalException(Exception exception)
        => exception is not (OutOfMemoryException or AccessViolationException);

    public static void HandleMessageRequest(IInfiniFrameWindow window, string? message) {
        if (string.IsNullOrWhiteSpace(message)) {
            window.Logger.LogDebug("Rejected empty web message.");
            return;
        }

        InteropEnvelopeParseResult parseResult = InteropEnvelopeProtocol.ParseIncomingMessage(message);
        if (!parseResult.Success) {
            window.Logger.LogWarning("Rejected invalid web message: {Reason}", parseResult.Error ?? "Unknown error");
            return;
        }

        string messageId = parseResult.MessageId!;

        if (string.Equals(parseResult.Command, InteropEnvelopeProtocol.PostCommand, StringComparison.Ordinal)) {
            window.MessageHandlers.TryHandlePostDataRequest(window, message);
            return;
        }

        if (!string.Equals(parseResult.Command, InteropEnvelopeProtocol.GetCommand, StringComparison.Ordinal)) {
            return;
        }

        try {
            if (window.MessageHandlers.TryHandleGetDataRequest(window, message, out string? responsePayload)) {
                SendSuccess(window, parseResult.RequestId, responsePayload);
                return;
            }

            SendError(window, parseResult.RequestId, $"No getMessage handler is registered for message ID '{messageId}'.");

        }
        catch (Exception ex) when (IsNonFatalException(ex)) {
            window.Logger.LogError(ex, "Unhandled exception while processing getMessage request '{MessageId}'.", messageId);
            SendError(window, parseResult.RequestId, $"Unhandled exception while processing '{messageId}'.");
        }
    }

    private static void SendSuccess(IInfiniFrameWindow window, string? requestId, string? data) {
        string responsePayloadJson = JsonSerializer.Serialize(
            new GetMessageSuccessResponse {
                RequestId = requestId,
                Success = true,
                Data = data
            },
            InfiniFrameWindowMessagesJsonContext.Default.GetMessageSuccessResponse
        );
        string responseEnvelope = InteropEnvelopeProtocol.CreateEnvelopeMessage(
            HandlerNames.GetMessageResponse,
            responsePayloadJson,
            InteropEnvelopeProtocol.GetCommand,
            requestId
        );
        window.SendWebMessage(responseEnvelope);
    }

    private static void SendError(IInfiniFrameWindow window, string? requestId, string error) {
        string responsePayloadJson = JsonSerializer.Serialize(
            new GetMessageErrorResponse {
                RequestId = requestId,
                Success = false,
                Error = error
            },
            InfiniFrameWindowMessagesJsonContext.Default.GetMessageErrorResponse
        );
        string responseEnvelope = InteropEnvelopeProtocol.CreateEnvelopeMessage(
            HandlerNames.GetMessageResponse,
            responsePayloadJson,
            InteropEnvelopeProtocol.GetCommand,
            requestId
        );
        window.SendWebMessage(responseEnvelope);
    }
}
