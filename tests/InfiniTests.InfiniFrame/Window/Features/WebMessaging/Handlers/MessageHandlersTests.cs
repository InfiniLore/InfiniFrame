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
public class MessageHandlersTests {
    [Test]
    public async Task WindowManagement_CloseMessage_ClosesWindow(CancellationToken ct = default) {
        // Arrange
        (InfiniFrameWindowBuilder builder, InfiniFrameEvents events, RecordingInfiniFrameWindowSubstitute window) = CreateWindowHarness();
        builder.RegisterWindowManagementWebMessageHandler();

        // Act
        events.OnWebMessageReceived(InteropEnvelopeProtocol.CreateEnvelopeMessage(JsHandlerNames.WindowClose));

        // Assert
        window.Lifecycle.Close().WasCalled(Times.Once);
    }

    [Test]
    public async Task WindowManagement_RegistersWindowCloseSubscriptionAfterReadyHandshake(CancellationToken ct = default) {
        // Arrange
        (InfiniFrameWindowBuilder builder, InfiniFrameEvents events, RecordingInfiniFrameWindowSubstitute window) = CreateWindowHarness();
        builder.RegisterWindowManagementWebMessageHandler();

        // Act
        events.OnWindowCreated();
        events.OnWebMessageReceived(InteropEnvelopeProtocol.CreateEnvelopeMessage(JsHandlerNames.WindowReady));
        await Task.Delay(150);

        // Assert
        int registrationCount = window.CountEnvelopeMessagesById(JsHandlerNames.RegisterWindowClose);
        await Assert.That(registrationCount).IsEqualTo(1);
    }

    [Test]
    public async Task FullscreenToggle_InvokesWindowMutation(CancellationToken ct = default) {
        // Arrange
        (InfiniFrameWindowBuilder builder, InfiniFrameEvents events, RecordingInfiniFrameWindowSubstitute window) = CreateWindowHarness();
        builder.RegisterFullScreenWebMessageHandler();

        // Act
        events.OnWebMessageReceived(InteropEnvelopeProtocol.CreateEnvelopeMessage(JsHandlerNames.FullscreenToggle));

        // Assert
        await Assert.That(Mock.Invocations(window.State).Count(c => c.MemberName == "SetFullScreen")).IsEqualTo(1);
    }

    [Test]
    public async Task TitleChanged_WithPayload_InvokesWindowMutation(CancellationToken ct = default) {
        // Arrange
        (InfiniFrameWindowBuilder builder, InfiniFrameEvents events, RecordingInfiniFrameWindowSubstitute window) = CreateWindowHarness();
        builder.RegisterTitleChangedWebMessageHandler();

        // Act
        events.OnWebMessageReceived(InteropEnvelopeProtocol.CreateEnvelopeMessage(JsHandlerNames.TitleChanged, "new title"));

        // Assert
        window.Decorations.SetTitle(Any<string?>()).WasCalled(Times.Once);
    }

    [Test]
    public async Task TitleChanged_WithoutPayload_DoesNotInvokeWindowMutation(CancellationToken ct = default) {
        // Arrange
        (InfiniFrameWindowBuilder builder, InfiniFrameEvents events, RecordingInfiniFrameWindowSubstitute window) = CreateWindowHarness();
        builder.RegisterTitleChangedWebMessageHandler();

        // Act
        events.OnWebMessageReceived(InteropEnvelopeProtocol.CreateEnvelopeMessage(JsHandlerNames.TitleChanged));

        // Assert
        window.Decorations.SetTitle(Any<string?>()).WasNeverCalled();
    }

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
