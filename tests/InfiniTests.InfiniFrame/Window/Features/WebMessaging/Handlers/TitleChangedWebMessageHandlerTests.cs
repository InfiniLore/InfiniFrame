// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Interop;
using InfiniFrame.NativeBridge.Parameters;
using InfiniTests.Substitutes;
using Microsoft.Extensions.Logging.Abstractions;

namespace InfiniTests.InfiniFrame.Window.Features.WebMessaging.Handlers;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class TitleChangedWebMessageHandlerTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Null / Empty Payload Tests
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    [Arguments("\t")]
    public async Task HandleWebMessage_NullOrEmptyPayload_DoesNotChangeTitle(string? payload) {
        // Arrange
        (InfiniFrameWindowBuilder builder, InfiniFrameEvents events, RecordingInfiniFrameWindowSubstitute window) = CreateWindowHarness();
        builder.RegisterTitleChangedWebMessageHandler();

        // Act
        events.OnWebMessageReceived(InteropEnvelopeProtocol.CreateEnvelopeMessage(JsHandlerNames.TitleChanged, payload));

        // Assert: SetTitle should never be called
        window.Decorations.SetTitle(Any<string?>()).WasNeverCalled();
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Valid Payload Tests
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task HandleWebMessage_ValidPayload_SetsWindowTitle(CancellationToken ct = default) {
        // Arrange
        (InfiniFrameWindowBuilder builder, InfiniFrameEvents events, RecordingInfiniFrameWindowSubstitute window) = CreateWindowHarness();
        builder.RegisterTitleChangedWebMessageHandler();

        // Act
        events.OnWebMessageReceived(InteropEnvelopeProtocol.CreateEnvelopeMessage(JsHandlerNames.TitleChanged, "My New Title"));

        // Assert: SetTitle should be called once with the new title
        window.Decorations.SetTitle("My New Title").WasCalled(Times.Once);
    }

    [Test]
    public async Task HandleWebMessage_DifferentPayload_UpdatesTitle(CancellationToken ct = default) {
        // Arrange
        (InfiniFrameWindowBuilder builder, InfiniFrameEvents events, RecordingInfiniFrameWindowSubstitute window) = CreateWindowHarness();
        builder.RegisterTitleChangedWebMessageHandler();

        // Act
        events.OnWebMessageReceived(InteropEnvelopeProtocol.CreateEnvelopeMessage(JsHandlerNames.TitleChanged, "First Title"));
        events.OnWebMessageReceived(InteropEnvelopeProtocol.CreateEnvelopeMessage(JsHandlerNames.TitleChanged, "Second Title"));

        // Assert: SetTitle should be called twice
        window.Decorations.SetTitle("First Title").WasCalled(Times.Once);
        window.Decorations.SetTitle("Second Title").WasCalled(Times.Once);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Helper Methods
    // -----------------------------------------------------------------------------------------------------------------
    private static (InfiniFrameWindowBuilder Builder, InfiniFrameEvents Events, RecordingInfiniFrameWindowSubstitute Window) CreateWindowHarness() {
        var builder = new InfiniFrameWindowBuilder();
        var eventsStore = (InfiniFrameEventsStore)builder.EventsStore;

        RecordingInfiniFrameWindowSubstitute window = new RecordingInfiniFrameWindowSubstitute()
            .BindToBuilder(builder);

        var events = new InfiniFrameEvents(eventsStore, NullLogger<InfiniFrameEvents>.Instance);
        var nativeParameters = default(InfiniFrameNativeParameters);
        events.AssignToNativeParameters(ref nativeParameters);
        events.AssignToWindow(window.Window);

        return (builder, events, window);
    }
}
