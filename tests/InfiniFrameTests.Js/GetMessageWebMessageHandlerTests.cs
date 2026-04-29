// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.HostMessaging;
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
        
        string inboundMessage = InteropEnvelopeProtocol.CreateEnvelopeMessage(
            "app:echo",
            "hello",
            InteropEnvelopeProtocol.GetCommand,
            "req-1"
        );

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

        string inboundMessage = InteropEnvelopeProtocol.CreateEnvelopeMessage(
            "app:missing",
            "hello",
            InteropEnvelopeProtocol.GetCommand,
            "req-2"
        );

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

        string inboundMessage = InteropEnvelopeProtocol.CreateEnvelopeMessage(
            "app:echo",
            "hello",
            InteropEnvelopeProtocol.GetCommand,
            "req-3"
        );

        // Act
        events.OnWebMessageReceived(inboundMessage);

        // Assert
        JsonElement responsePayload = GetLatestGetMessageResponsePayload(window);
        await Assert.That(responsePayload.GetProperty("requestId").GetString()).IsEqualTo("req-3");
        await Assert.That(responsePayload.GetProperty("success").GetBoolean()).IsFalse();
        await Assert.That(responsePayload.GetProperty("error").GetString()).Contains("No getMessage handler is registered");
    }

    private static (InfiniFrameWindowBuilder Builder, InfiniFrameWindowEvents Events, RecordingInfiniFrameWindowSubstitute Window, InfiniFrameWindowMessageHandler MessageHandler) CreateWindowHarness() {
        var builder = InfiniFrameWindowBuilder.Create();
        var events = (InfiniFrameWindowEvents)builder.Events;
        var messageHandler = (InfiniFrameWindowMessageHandler)builder.MessageHandlers;
        
        RecordingInfiniFrameWindowSubstitute window = new RecordingInfiniFrameWindowSubstitute()
            .BindToBuilder(builder);

        events.WebMessageReceived.Add(InfiniFrameWindowMessageHandler.HandleMessageRequest);
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
