// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Js;
using InfiniFrame.Js.Interop;
using InfiniFrame.Js.Interop.MessageHandlers;
using InfiniFrameTests.Shared.TestDoubles;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace InfiniFrameTests.Js;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class MessageHandlersTests {
    [Test]
    [DisplayName($"{nameof(MessageHandlersTests)}.{nameof(WindowManagement_CloseMessage_ClosesWindow)}")]
    public async Task WindowManagement_CloseMessage_ClosesWindow() {
        // Arrange
        (InfiniFrameWindowBuilder builder, InfiniFrameWindowEvents events, RecordingInfiniFrameWindowSubstitute window) = CreateWindowHarness();
        builder.RegisterWindowManagementWebMessageHandler();

        // Act
        events.OnWebMessageReceived(InteropEnvelopeProtocol.CreateEnvelopeMessage(HandlerNames.WindowClose));

        // Assert
        int closeCallCount = CountMethodCalls(window.Window, nameof(IInfiniFrameWindow.Close));
        await Assert.That(closeCallCount).IsEqualTo(1);
    }

    [Test]
    [DisplayName($"{nameof(MessageHandlersTests)}.{nameof(WindowManagement_RegistersWindowCloseSubscriptionAfterReadyHandshake)}")]
    public async Task WindowManagement_RegistersWindowCloseSubscriptionAfterReadyHandshake() {
        // Arrange
        (InfiniFrameWindowBuilder builder, InfiniFrameWindowEvents events, RecordingInfiniFrameWindowSubstitute window) = CreateWindowHarness();
        builder.RegisterWindowManagementWebMessageHandler();

        // Act
        events.OnWindowCreated();
        events.OnWebMessageReceived(InteropEnvelopeProtocol.CreateEnvelopeMessage(HandlerNames.WindowReady));
        await Task.Delay(150);

        // Assert
        int registrationCount = window.CountEnvelopeMessagesById(HandlerNames.RegisterWindowClose);
        await Assert.That(registrationCount).IsEqualTo(1);
    }

    [Test]
    [DisplayName($"{nameof(MessageHandlersTests)}.{nameof(FullscreenToggle_InvokesWindowMutation)}")]
    public async Task FullscreenToggle_InvokesWindowMutation() {
        // Arrange
        (InfiniFrameWindowBuilder builder, InfiniFrameWindowEvents events, RecordingInfiniFrameWindowSubstitute window) = CreateWindowHarness();
        builder.RegisterFullScreenWebMessageHandler();

        // Act
        events.OnWebMessageReceived(InteropEnvelopeProtocol.CreateEnvelopeMessage(HandlerNames.FullscreenToggle));

        // Assert
        int invokeCallCount = CountMethodCalls(window.Window, nameof(IInfiniFrameWindow.Invoke));
        await Assert.That(invokeCallCount).IsEqualTo(1);
    }

    [Test]
    [DisplayName($"{nameof(MessageHandlersTests)}.{nameof(TitleChanged_WithPayload_InvokesWindowMutation)}")]
    public async Task TitleChanged_WithPayload_InvokesWindowMutation() {
        // Arrange
        (InfiniFrameWindowBuilder builder, InfiniFrameWindowEvents events, RecordingInfiniFrameWindowSubstitute window) = CreateWindowHarness();
        builder.RegisterTitleChangedWebMessageHandler();

        // Act
        events.OnWebMessageReceived(InteropEnvelopeProtocol.CreateEnvelopeMessage(HandlerNames.TitleChanged, "new title"));

        // Assert
        int invokeCallCount = CountMethodCalls(window.Window, nameof(IInfiniFrameWindow.Invoke));
        await Assert.That(invokeCallCount).IsEqualTo(1);
    }

    [Test]
    [DisplayName($"{nameof(MessageHandlersTests)}.{nameof(TitleChanged_WithoutPayload_DoesNotInvokeWindowMutation)}")]
    public async Task TitleChanged_WithoutPayload_DoesNotInvokeWindowMutation() {
        // Arrange
        (InfiniFrameWindowBuilder builder, InfiniFrameWindowEvents events, RecordingInfiniFrameWindowSubstitute window) = CreateWindowHarness();
        builder.RegisterTitleChangedWebMessageHandler();

        // Act
        events.OnWebMessageReceived(InteropEnvelopeProtocol.CreateEnvelopeMessage(HandlerNames.TitleChanged));

        // Assert
        int invokeCallCount = CountMethodCalls(window.Window, nameof(IInfiniFrameWindow.Invoke));
        await Assert.That(invokeCallCount).IsEqualTo(0);
    }

    [Test]
    [DisplayName($"{nameof(MessageHandlersTests)}.{nameof(OpenExternal_WithInvalidUrl_LogsWarningWithoutThrowing)}")]
    public async Task OpenExternal_WithInvalidUrl_LogsWarningWithoutThrowing() {
        // Arrange
        (InfiniFrameWindowBuilder builder, InfiniFrameWindowEvents events, RecordingInfiniFrameWindowSubstitute window) = CreateWindowHarness();
        ILogger<IInfiniFrameWindow> logger = Substitute.For<ILogger<IInfiniFrameWindow>>();
        window.Window.Logger.Returns(logger);
        builder.RegisterOpenExternalTargetWebMessageHandler();

        // Act
        events.OnWebMessageReceived(InteropEnvelopeProtocol.CreateEnvelopeMessage(HandlerNames.OpenExternal, "not-a-valid-url"));

        // Assert
        bool warningLogged = logger.ReceivedCalls().Any(call =>
            call.GetMethodInfo().Name == nameof(ILogger.Log) &&
            call.GetArguments().Length > 0 &&
            call.GetArguments()[0] is LogLevel.Warning
        );

        await Assert.That(warningLogged).IsTrue();
    }

    [Test]
    [DisplayName($"{nameof(MessageHandlersTests)}.{nameof(OpenExternal_WithDisallowedScheme_LogsWarningWithoutThrowing)}")]
    public async Task OpenExternal_WithDisallowedScheme_LogsWarningWithoutThrowing() {
        // Arrange
        (InfiniFrameWindowBuilder builder, InfiniFrameWindowEvents events, RecordingInfiniFrameWindowSubstitute window) = CreateWindowHarness();
        ILogger<IInfiniFrameWindow> logger = Substitute.For<ILogger<IInfiniFrameWindow>>();
        window.Window.Logger.Returns(logger);
        builder.SetAllowedExternalSchemes(Uri.UriSchemeMailto);
        builder.RegisterOpenExternalTargetWebMessageHandler();

        // Act
        events.OnWebMessageReceived(InteropEnvelopeProtocol.CreateEnvelopeMessage(HandlerNames.OpenExternal, "https://example.com"));

        // Assert
        bool warningLogged = logger.ReceivedCalls().Any(call =>
            call.GetMethodInfo().Name == nameof(ILogger.Log) &&
            call.GetArguments().Length > 0 &&
            call.GetArguments()[0] is LogLevel.Warning
        );

        await Assert.That(warningLogged).IsTrue();
    }

    private static (InfiniFrameWindowBuilder Builder, InfiniFrameWindowEvents Events, RecordingInfiniFrameWindowSubstitute Window) CreateWindowHarness() {
        var builder = InfiniFrameWindowBuilder.Create();
        var events = (InfiniFrameWindowEvents)builder.Events;
        RecordingInfiniFrameWindowSubstitute window = new RecordingInfiniFrameWindowSubstitute()
            .BindToBuilder(builder);

        events.WebMessageReceived.Add(builder.MessageHandlers.Handle);
        events.CompleteSetup(window.Window);

        return (builder, events, window);
    }

    private static int CountMethodCalls(IInfiniFrameWindow window, string methodName) {
        return window.ReceivedCalls().Count(call => string.Equals(call.GetMethodInfo().Name, methodName, StringComparison.Ordinal));
    }
}
