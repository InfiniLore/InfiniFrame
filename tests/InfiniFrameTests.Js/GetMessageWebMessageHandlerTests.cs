// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Interop;
using InfiniFrame.Js;
using InfiniFrame.Js.Interop;
using InfiniFrameTests.Shared.TestDoubles;
using System.Text.Json;

namespace InfiniFrameTests.Js;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class GetMessageWebMessageHandlerTests {
    [Test]
    public async Task GetMessage_ResolvesRegisteredHandlerAndReturnsData() {
        // Arrange
        (InfiniFrameWindowBuilder _, InfiniFrameWindowEvents events, RecordingInfiniFrameWindowSubstitute window, InfiniFrameWindowMessageHandler messageHandler) = CreateWindowHarness();
        messageHandler.RegisterHandler("app:echo", (_, payload) => $"echo:{payload}");
        
        string requestedEnvelope = InteropEnvelopeProtocol.CreateEnvelopeMessage("app:echo", "hello");
        string requestPayload = JsonSerializer.Serialize(new {
            requestId = "req-1",
            message = requestedEnvelope
        });
        string inboundMessage = InteropEnvelopeProtocol.CreateEnvelopeMessage(HandlerNames.GetMessageRequest, requestPayload);

        // Act
        events.OnWebMessageReceived(inboundMessage);

        // Assert
        JsonElement responsePayload = GetLatestGetMessageResponsePayload(window);
        await Assert.That(responsePayload.GetProperty("requestId").GetString()).IsEqualTo("req-1");
        await Assert.That(responsePayload.GetProperty("success").GetBoolean()).IsTrue();
        await Assert.That(responsePayload.GetProperty("data").GetString()).IsEqualTo("echo:hello");
    }

    [Test]
    public async Task GetMessage_WithoutRegisteredMessageHandler_ReturnsErrorResponse() {
        // Arrange
        (InfiniFrameWindowBuilder _, InfiniFrameWindowEvents events, RecordingInfiniFrameWindowSubstitute window, InfiniFrameWindowMessageHandler _) = CreateWindowHarness();

        string requestedEnvelope = InteropEnvelopeProtocol.CreateEnvelopeMessage("app:missing", "hello");
        string requestPayload = JsonSerializer.Serialize(new {
            requestId = "req-2",
            message = requestedEnvelope
        });
        string inboundMessage = InteropEnvelopeProtocol.CreateEnvelopeMessage(HandlerNames.GetMessageRequest, requestPayload);

        // Act
        events.OnWebMessageReceived(inboundMessage);

        // Assert
        JsonElement responsePayload = GetLatestGetMessageResponsePayload(window);
        await Assert.That(responsePayload.GetProperty("requestId").GetString()).IsEqualTo("req-2");
        await Assert.That(responsePayload.GetProperty("success").GetBoolean()).IsFalse();
        await Assert.That(responsePayload.GetProperty("error").GetString()).Contains("No getMessage handler is registered");
    }

    [Test]
    public async Task GetMessage_WithoutRegisteredService_ReturnsErrorResponse() {
        // Arrange
        (InfiniFrameWindowBuilder _, InfiniFrameWindowEvents events, RecordingInfiniFrameWindowSubstitute window, InfiniFrameWindowMessageHandler _) = CreateWindowHarness();

        string requestPayload = JsonSerializer.Serialize(new {
            requestId = "req-3",
            message = InteropEnvelopeProtocol.CreateEnvelopeMessage("app:echo", "hello")
        });
        string inboundMessage = InteropEnvelopeProtocol.CreateEnvelopeMessage(HandlerNames.GetMessageRequest, requestPayload);

        // Act
        events.OnWebMessageReceived(inboundMessage);

        // Assert
        JsonElement responsePayload = GetLatestGetMessageResponsePayload(window);
        await Assert.That(responsePayload.GetProperty("requestId").GetString()).IsEqualTo("req-3");
        await Assert.That(responsePayload.GetProperty("success").GetBoolean()).IsFalse();
        await Assert.That(responsePayload.GetProperty("error").GetString()).Contains("No IInfiniFrameGetMessageService is registered");
    }

    private static (InfiniFrameWindowBuilder Builder, InfiniFrameWindowEvents Events, RecordingInfiniFrameWindowSubstitute Window, InfiniFrameWindowMessageHandler MessageHandler) CreateWindowHarness() {
        var builder = InfiniFrameWindowBuilder.Create();
        var events = (InfiniFrameWindowEvents)builder.Events;
        var messageHandler = (InfiniFrameWindowMessageHandler)builder.MessageHandlers;
        
        RecordingInfiniFrameWindowSubstitute window = new RecordingInfiniFrameWindowSubstitute()
            .BindToBuilder(builder);

        events.WebMessageReceived.Add((sender, message) => builder.MessageHandlers.TryHandlePostDataRequest(sender, message));
        events.CompleteSetup(window.Window);

        return (builder, events, window, messageHandler);
    }

    private static JsonElement GetLatestGetMessageResponsePayload(RecordingInfiniFrameWindowSubstitute window) {
        string? responseEnvelope = window.GetSentMessagesSnapshot()
            .LastOrDefault(message => InteropEnvelopeProtocol.ParseIncomingMessage(message).MessageId == HandlerNames.GetMessageResponse);

        Fail.When(responseEnvelope is null, "Expected a getMessage response envelope.");
        
        InteropEnvelopeParseResult parsedEnvelope = InteropEnvelopeProtocol.ParseIncomingMessage(responseEnvelope);
        if (!parsedEnvelope.Success || string.IsNullOrWhiteSpace(parsedEnvelope.Payload))
            throw new InvalidOperationException("Expected a successful getMessage response envelope with payload.");

        using JsonDocument document = JsonDocument.Parse(parsedEnvelope.Payload);
        return document.RootElement.Clone();
    }
}
