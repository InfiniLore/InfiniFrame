// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.HostMessaging;
using InfiniFrame.Interop;
using InfiniFrame.Js;
using InfiniFrame.Js.Interop;
using InfiniFrameTests.Shared.TestDoubles;
using NSubstitute;
using System.Text.Json;

namespace InfiniFrameTests.Js;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class GetMessageWebMessageHandlerTests {
    [Test]
    public async Task GetMessage_StandardGetRequest_Title_ReturnsWindowTitle() {
        // Arrange
        (InfiniFrameWindowBuilder builder, InfiniFrameWindowEvents events, RecordingInfiniFrameWindowSubstitute window, InfiniFrameWindowMessageHandler _)
            = CreateWindowHarness();

        builder.RegisterGetWebMessageHandler();
        window.Window.Title.Returns("Native Test Title");

        string inboundMessage = InteropEnvelopeProtocol.CreateEnvelopeMessage(
            HandlerNames.GetRequest,
            "{\"command\":\"title\"}",
            InteropEnvelopeProtocol.GetCommand,
            "req-standard-get-1"
        );

        // Act
        events.OnWebMessageReceived(inboundMessage);

        // Assert
        InteropEnvelopeParseResult response = GetLatestGetMessageResponse(window);

        using JsonDocument doc = JsonDocument.Parse(response.Payload!);
        JsonElement payload = doc.RootElement;

        await Assert.That(payload.GetProperty("requestId").GetString()).IsEqualTo("req-standard-get-1");
        await Assert.That(payload.GetProperty("success").GetBoolean()).IsTrue();
        await Assert.That(payload.GetProperty("data").GetString()).IsEqualTo("Native Test Title");
    }

    [Test]
    public async Task GetMessage_ResolvesRegisteredHandlerAndReturnsData() {
        // Arrange
        (InfiniFrameWindowBuilder _, InfiniFrameWindowEvents events, RecordingInfiniFrameWindowSubstitute window, InfiniFrameWindowMessageHandler messageHandler)
            = CreateWindowHarness();

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
        InteropEnvelopeParseResult response = GetLatestGetMessageResponse(window);

        using JsonDocument doc = JsonDocument.Parse(response.Payload!);
        JsonElement payload = doc.RootElement;

        await Assert.That(payload.GetProperty("requestId").GetString()).IsEqualTo("req-1");
        await Assert.That(payload.GetProperty("success").GetBoolean()).IsTrue();
        await Assert.That(payload.GetProperty("data").GetString()).IsEqualTo("echo:hello");
    }

    [Test]
    public async Task GetMessage_WithoutRegisteredHandler_ReturnsErrorResponse() {
        // Arrange
        (InfiniFrameWindowBuilder _, InfiniFrameWindowEvents events, RecordingInfiniFrameWindowSubstitute window, InfiniFrameWindowMessageHandler _)
            = CreateWindowHarness();

        string inboundMessage = InteropEnvelopeProtocol.CreateEnvelopeMessage(
            "app:missing",
            "hello",
            InteropEnvelopeProtocol.GetCommand,
            "req-2"
        );

        // Act
        events.OnWebMessageReceived(inboundMessage);

        // Assert
        InteropEnvelopeParseResult response = GetLatestGetMessageResponse(window);

        using JsonDocument doc = JsonDocument.Parse(response.Payload!);
        JsonElement payload = doc.RootElement;

        await Assert.That(payload.GetProperty("requestId").GetString()).IsEqualTo("req-2");
        await Assert.That(payload.GetProperty("success").GetBoolean()).IsFalse();
        await Assert.That(payload.GetProperty("error").GetString())
            .Contains("No getMessage handler is registered");
    }

    [Test]
    public async Task GetMessage_WithoutRegisteredService_ReturnsErrorResponse() {
        // Arrange
        (InfiniFrameWindowBuilder _, InfiniFrameWindowEvents events, RecordingInfiniFrameWindowSubstitute window, InfiniFrameWindowMessageHandler _)
            = CreateWindowHarness();

        string inboundMessage = InteropEnvelopeProtocol.CreateEnvelopeMessage(
            "app:echo",
            "hello",
            InteropEnvelopeProtocol.GetCommand,
            "req-3"
        );

        // Act
        events.OnWebMessageReceived(inboundMessage);

        // Assert
        InteropEnvelopeParseResult response = GetLatestGetMessageResponse(window);

        using JsonDocument doc = JsonDocument.Parse(response.Payload!);
        JsonElement payload = doc.RootElement;

        await Assert.That(payload.GetProperty("requestId").GetString())
            .IsEqualTo("req-3");

        await Assert.That(payload.GetProperty("success").GetBoolean())
            .IsFalse();

        await Assert.That(payload.GetProperty("error").GetString())
            .Contains("No getMessage handler is registered");
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

    private static InteropEnvelopeParseResult GetLatestGetMessageResponse(
        RecordingInfiniFrameWindowSubstitute window
    ) {
        InteropEnvelopeParseResult responseEnvelope = window.GetSentMessagesSnapshot()
            .Select(InteropEnvelopeProtocol.ParseIncomingMessage)
            .LastOrDefault(r =>
                r.Success &&
                r.MessageId == HandlerNames.GetResponse
            );

        Fail.When(!responseEnvelope.Success, "Expected a valid getMessage response envelope.");

        return responseEnvelope;
    }
}
