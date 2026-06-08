// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Interop;
using InfiniFrame.NativeBridge.Parameters;
using InfiniFrame.Security;
using InfiniTests.Substitutes;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace InfiniTests.InfiniFrame.MessageHandlers;
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
        int closeCallCount = CountMethodCalls(window.Window, nameof(IInfiniFrameWindow.Close));
        await Assert.That(closeCallCount).IsEqualTo(1);
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
        int invokeCallCount = CountMethodCalls(window.Window, nameof(IInfiniFrameWindow.Invoke));
        await Assert.That(invokeCallCount).IsEqualTo(1);
    }

    [Test]
    public async Task TitleChanged_WithPayload_InvokesWindowMutation(CancellationToken ct = default) {
        // Arrange
        (InfiniFrameWindowBuilder builder, InfiniFrameEvents events, RecordingInfiniFrameWindowSubstitute window) = CreateWindowHarness();
        builder.RegisterTitleChangedWebMessageHandler();

        // Act
        events.OnWebMessageReceived(InteropEnvelopeProtocol.CreateEnvelopeMessage(JsHandlerNames.TitleChanged, "new title"));

        // Assert
        int invokeCallCount = CountMethodCalls(window.Window, nameof(IInfiniFrameWindow.Invoke));
        await Assert.That(invokeCallCount).IsEqualTo(1);
    }

    [Test]
    public async Task TitleChanged_WithoutPayload_DoesNotInvokeWindowMutation(CancellationToken ct = default) {
        // Arrange
        (InfiniFrameWindowBuilder builder, InfiniFrameEvents events, RecordingInfiniFrameWindowSubstitute window) = CreateWindowHarness();
        builder.RegisterTitleChangedWebMessageHandler();

        // Act
        events.OnWebMessageReceived(InteropEnvelopeProtocol.CreateEnvelopeMessage(JsHandlerNames.TitleChanged));

        // Assert
        int invokeCallCount = CountMethodCalls(window.Window, nameof(IInfiniFrameWindow.Invoke));
        await Assert.That(invokeCallCount).IsEqualTo(0);
    }

    [Test]
    public async Task OpenExternal_WithInvalidUrl_LogsWarningWithoutThrowing(CancellationToken ct = default) {
        // Arrange
        (InfiniFrameWindowBuilder builder, InfiniFrameEvents events, RecordingInfiniFrameWindowSubstitute window) = CreateWindowHarness();
        var logger = Substitute.For<ILogger<IInfiniFrameWindow>>();
        window.Window.Logger.Returns(logger);
        builder.RegisterOpenExternalTargetWebMessageHandler();

        // Act
        events.OnWebMessageReceived(InteropEnvelopeProtocol.CreateEnvelopeMessage(JsHandlerNames.OpenExternal, "not-a-valid-url"));

        // Assert
        bool warningLogged = logger.ReceivedCalls().Any(call =>
            call.GetMethodInfo().Name == nameof(ILogger.Log) &&
            call.GetArguments().Length > 0 &&
            call.GetArguments()[0] is LogLevel.Warning
        );

        await Assert.That(warningLogged).IsTrue();
    }

    [Test]
    public async Task OpenExternal_WithDisallowedScheme_LogsWarningWithoutThrowing(CancellationToken ct = default) {
        // Arrange
        (InfiniFrameWindowBuilder builder, InfiniFrameEvents events, RecordingInfiniFrameWindowSubstitute window) = CreateWindowHarness();
        var logger = Substitute.For<ILogger<IInfiniFrameWindow>>();
        window.Window.Logger.Returns(logger);
        InfiniFrameUriSecurityPolicyRegistry.BindToWindow(
            window.Window,
            new InfiniFrameUriSecurityPolicy(
                [Uri.UriSchemeHttps, Uri.UriSchemeHttp, "app"],
                [Uri.UriSchemeMailto]));
        builder.RegisterOpenExternalTargetWebMessageHandler();

        // Act
        events.OnWebMessageReceived(InteropEnvelopeProtocol.CreateEnvelopeMessage(JsHandlerNames.OpenExternal, "https://example.com"));

        // Assert
        bool warningLogged = logger.ReceivedCalls().Any(call =>
            call.GetMethodInfo().Name == nameof(ILogger.Log) &&
            call.GetArguments().Length > 0 &&
            call.GetArguments()[0] is LogLevel.Warning
        );

        await Assert.That(warningLogged).IsTrue();
    }

    private static (InfiniFrameWindowBuilder Builder, InfiniFrameEvents Events, RecordingInfiniFrameWindowSubstitute Window) CreateWindowHarness() {
        var builder = InfiniFrameWindowBuilder.Create();
        var eventsStore = (InfiniFrameEventsStore)builder.EventsStore;

        RecordingInfiniFrameWindowSubstitute window = new RecordingInfiniFrameWindowSubstitute()
            .BindToBuilder(builder);

        var events = new InfiniFrameEvents(eventsStore);
        var nativeParameters = default(InfiniFrameNativeParameters);
        events.AssignEventCallbacks(ref nativeParameters);
        events.AssignToWindow(window.Window);

        return (builder, events, window);
    }

    private static int CountMethodCalls(IInfiniFrameWindow window, string methodName) {
        return window.ReceivedCalls().Count(call => string.Equals(call.GetMethodInfo().Name, methodName, StringComparison.Ordinal));
    }
}
