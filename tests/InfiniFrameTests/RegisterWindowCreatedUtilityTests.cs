// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Js.Interop;
using InfiniFrameTests.Shared.TestDoubles;

namespace InfiniFrameTests;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class RegisterWindowCreatedUtilityTests {
    [Test]
    [DisplayName($"{nameof(RegisterWindowCreatedUtilityTests)}.{nameof(Registration_IsGatedByWindowReadyHandshake)}")]
    public async Task Registration_IsGatedByWindowReadyHandshake() {
        // Arrange
        const string registrationMessageId = "__infiniframe:register:test";
        string readyEnvelope = InteropEnvelopeProtocol.CreateEnvelopeMessage("__infiniframe:ready");
        var builder = InfiniFrameWindowBuilder.Create();
        var events = (InfiniFrameWindowEvents)builder.Events;
        var window = new RecordingInfiniFrameWindowSubstitute()
            .BindToBuilder(builder);

        events.WebMessageReceived.Add(builder.MessageHandlers.Handle);
        events.CompleteSetup(window.Window);

        RegisterWindowCreatedUtility.RegisterWindowCreatedWebMessage(builder, registrationMessageId);

        // Act
        events.OnWindowCreated();
        await Task.Delay(150);

        // Assert pre-ready: nothing should be sent yet.
        int sendAttemptsBeforeReady = window.CountEnvelopeMessagesById(registrationMessageId);
        await Assert.That(sendAttemptsBeforeReady).IsEqualTo(0);

        // Act: explicit ready handshake.
        events.OnWebMessageReceived(readyEnvelope);
        await Task.Delay(150);

        // Assert: exactly one registration send attempt after ready.
        int sendAttemptsAfterReady = window.CountEnvelopeMessagesById(registrationMessageId);
        await Assert.That(sendAttemptsAfterReady).IsEqualTo(1);
    }

    [Test]
    [DisplayName(
        $"{nameof(RegisterWindowCreatedUtilityTests)}.{nameof(Registration_IsIdempotentAcrossRepeatedReadyMessages)}")]
    public async Task Registration_IsIdempotentAcrossRepeatedReadyMessages() {
        // Arrange
        const string registrationMessageId = "__infiniframe:register:test";
        string readyEnvelope = InteropEnvelopeProtocol.CreateEnvelopeMessage("__infiniframe:ready");
        var builder = InfiniFrameWindowBuilder.Create();
        var events = (InfiniFrameWindowEvents)builder.Events;
        RecordingInfiniFrameWindowSubstitute window = new RecordingInfiniFrameWindowSubstitute()
            .BindToBuilder(builder);

        events.WebMessageReceived.Add(builder.MessageHandlers.Handle);
        events.CompleteSetup(window.Window);

        RegisterWindowCreatedUtility.RegisterWindowCreatedWebMessage(builder, registrationMessageId);

        // Act: ready envelope received multiple times.
        events.OnWebMessageReceived(readyEnvelope);
        events.OnWebMessageReceived(readyEnvelope);
        await Task.Delay(150);

        // Assert: only one registration send attempt.
        int sendAttempts = window.CountEnvelopeMessagesById(registrationMessageId);
        await Assert.That(sendAttempts).IsEqualTo(1);
    }
}