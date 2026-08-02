// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Interop;
using InfiniFrame.NativeBridge.Parameters;
using InfiniTests.Substitutes;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using System.Text.Json;

namespace InfiniTests.InfiniFrame.Window.Features.WebMessaging.Handlers;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class GetMessageWebMessageHandlerTests {
    [Test]
    public void PostMessage_FeatureRequest_InvokesMappedMutation() {
        (InfiniFrameWindowBuilder builder, InfiniFrameEvents events, RecordingInfiniFrameWindowSubstitute window)
            = CreateWindowHarness();
        builder.RegisterGetWebMessageHandler();

        string inboundMessage = InteropEnvelopeProtocol.CreateEnvelopeMessage(
            JsHandlerNames.WindowFeatureRequest,
            "{\"command\":\"__infiniframe:window:features:decorations:setTitle\",\"args\":{\"title\":\"Mapped title\"}}"
        );

        events.OnWebMessageReceived(inboundMessage);

        window.Window.Features.Decorations.Received(1).SetTitle("Mapped title");
    }

    [Test]
    public async Task GetMessage_StandardGetRequest_Title_ReturnsWindowTitle(CancellationToken ct = default) {
        // Arrange
        (InfiniFrameWindowBuilder builder, InfiniFrameEvents events, RecordingInfiniFrameWindowSubstitute window)
            = CreateWindowHarness();

        builder.RegisterGetWebMessageHandler();
        window.Window.Features.Decorations.Title.Returns("Native Test Title");

        string inboundMessage = InteropEnvelopeProtocol.CreateEnvelopeMessage(
            JsHandlerNames.GetRequest,
            "{\"command\":\"__infiniframe:window:features:decorations:title\"}",
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
        string? serializedResult = payload.GetProperty("data").GetString();
        await Assert.That(JsonSerializer.Deserialize<string>(serializedResult!)).IsEqualTo("Native Test Title");
    }

    [Test]
    public async Task GetMessage_ResolvesRegisteredHandlerAndReturnsData(CancellationToken ct = default) {
        // Arrange
        (InfiniFrameWindowBuilder _, InfiniFrameEvents events, RecordingInfiniFrameWindowSubstitute window)
            = CreateWindowHarness();

        events.RegisterWebMessageGetHandler("app:echo", handler: (_, payload) => $"echo:{payload}");

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
    public async Task GetMessage_WithoutRegisteredHandler_ReturnsErrorResponse(CancellationToken ct = default) {
        // Arrange
        (InfiniFrameWindowBuilder _, InfiniFrameEvents events, RecordingInfiniFrameWindowSubstitute window)
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
    public async Task GetMessage_WithoutRegisteredService_ReturnsErrorResponse(CancellationToken ct = default) {
        // Arrange
        (InfiniFrameWindowBuilder _, InfiniFrameEvents events, RecordingInfiniFrameWindowSubstitute window)
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

    private static (InfiniFrameWindowBuilder Builder, InfiniFrameEvents Events, RecordingInfiniFrameWindowSubstitute Window) CreateWindowHarness() {
        var builder = InfiniFrameWindowBuilder.Create();
        var eventsStore = (InfiniFrameEventsStore)builder.EventsStore;

        RecordingInfiniFrameWindowSubstitute window = new RecordingInfiniFrameWindowSubstitute()
            .BindToBuilder(builder);

        var events = new InfiniFrameEvents(eventsStore, NullLogger<InfiniFrameEvents>.Instance);
        var nativeParameters = default(InfiniFrameNativeParameters);
        events.AssignToNativeParameters(ref nativeParameters);
        events.AssignToWindow(window.Window);

        return (builder, events, window);
    }

    private static InteropEnvelopeParseResult GetLatestGetMessageResponse(
        RecordingInfiniFrameWindowSubstitute window
    ) {
        InteropEnvelopeParseResult responseEnvelope = window.GetSentMessagesSnapshot()
            .Select(InteropEnvelopeProtocol.ParseIncomingMessage)
            .LastOrDefault(r =>
                r is { IsSuccess: true, MessageId: JsHandlerNames.GetResponse }
            );

        Fail.When(!responseEnvelope.IsSuccess, "Expected a valid getMessage response envelope.");

        return responseEnvelope;
    }
}