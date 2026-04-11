// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Js.Interop;
using InfiniFrame.Js.MessageHandlers;
using Microsoft.Extensions.Logging;

namespace InfiniFrameTests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class RegisterWindowCreatedUtilityTests {
    private sealed class CapturingLogger : ILogger<IInfiniFrameWindow> {
        public List<string> Messages { get; } = [];

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) {
            Messages.Add(formatter(state, exception));
        }
    }

    [Test]
    [DisplayName($"{nameof(RegisterWindowCreatedUtilityTests)}.{nameof(Registration_IsGatedByWindowReadyHandshake)}")]
    public async Task Registration_IsGatedByWindowReadyHandshake() {
        // Arrange
        const string registrationMessageId = "__infiniframe:register:test";
        string readyEnvelope = InteropEnvelopeProtocol.CreateEnvelopeMessage("__infiniframe:ready");
        var logger = new CapturingLogger();
        var builder = InfiniFrameWindowBuilder.Create();
        var events = (InfiniFrameWindowEvents)builder.Events;

        var window = new InfiniFrameWindow {
            Logger = logger,
            ServiceProvider = null,
            CustomSchemes = [],
            Parent = null,
            Events = events,
            MessageHandlers = builder.MessageHandlers
        };

        events.WebMessageReceived.Add(builder.MessageHandlers.Handle);
        events.CompleteSetup(window);

        RegisterWindowCreatedUtility.RegisterWindowCreatedWebMessage(builder, registrationMessageId);

        // Act
        events.OnWindowCreated();
        await Task.Delay(150);

        // Assert pre-ready: nothing should be sent yet.
        int sendAttemptsBeforeReady = logger.Messages.Count(message => message.Contains("Skipping SendWebMessage during shutdown"));
        await Assert.That(sendAttemptsBeforeReady).IsEqualTo(0);

        // Act: explicit ready handshake.
        events.OnWebMessageReceived(readyEnvelope);
        await Task.Delay(150);

        // Assert: exactly one registration send attempt after ready.
        int sendAttemptsAfterReady = logger.Messages.Count(message => message.Contains("Skipping SendWebMessage during shutdown"));
        await Assert.That(sendAttemptsAfterReady).IsEqualTo(1);
    }

    [Test]
    [DisplayName($"{nameof(RegisterWindowCreatedUtilityTests)}.{nameof(Registration_IsIdempotentAcrossRepeatedReadyMessages)}")]
    public async Task Registration_IsIdempotentAcrossRepeatedReadyMessages() {
        // Arrange
        const string registrationMessageId = "__infiniframe:register:test";
        string readyEnvelope = InteropEnvelopeProtocol.CreateEnvelopeMessage("__infiniframe:ready");
        var logger = new CapturingLogger();
        var builder = InfiniFrameWindowBuilder.Create();
        var events = (InfiniFrameWindowEvents)builder.Events;

        var window = new InfiniFrameWindow {
            Logger = logger,
            ServiceProvider = null,
            CustomSchemes = [],
            Parent = null,
            Events = events,
            MessageHandlers = builder.MessageHandlers
        };

        events.WebMessageReceived.Add(builder.MessageHandlers.Handle);
        events.CompleteSetup(window);

        RegisterWindowCreatedUtility.RegisterWindowCreatedWebMessage(builder, registrationMessageId);

        // Act: ready received multiple times.
        events.OnWebMessageReceived(readyEnvelope);
        events.OnWebMessageReceived(readyEnvelope);
        await Task.Delay(150);

        // Assert: only one registration send attempt.
        int sendAttempts = logger.Messages.Count(message => message.Contains("Skipping SendWebMessage during shutdown"));
        await Assert.That(sendAttempts).IsEqualTo(1);
    }
}
