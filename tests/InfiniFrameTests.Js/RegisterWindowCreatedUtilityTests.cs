// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Js.Interop;
using InfiniFrameTests.Shared.TestDoubles;

namespace InfiniFrameTests.Js;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class RegisterWindowCreatedUtilityTests {
    [Test]
    public async Task Registration_IsGatedByWindowReadyHandshake() {
        // Arrange
        const string registrationMessageId = "__infiniframe:register:test";
        string readyEnvelope = InteropEnvelopeProtocol.CreateEnvelopeMessage("__infiniframe:ready");
        var builder = InfiniFrameWindowBuilder.Create();
        var events = (InfiniFrameWindowEvents)builder.Events;
        RecordingInfiniFrameWindowSubstitute window = new RecordingInfiniFrameWindowSubstitute()
            .BindToBuilder(builder);

        events.WebMessageReceived.Add((sender, message) => builder.MessageHandlers.TryHandlePostDataRequest(sender, message));
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

        int ackAttemptsAfterReady = window.CountEnvelopeMessagesById("__infiniframe:ready:ack");
        await Assert.That(ackAttemptsAfterReady).IsEqualTo(1);
    }

    [Test]
    public async Task Registration_IsIdempotentAcrossRepeatedReadyMessages() {
        // Arrange
        const string registrationMessageId = "__infiniframe:register:test";
        string readyEnvelope = InteropEnvelopeProtocol.CreateEnvelopeMessage("__infiniframe:ready");
        var builder = InfiniFrameWindowBuilder.Create();
        var events = (InfiniFrameWindowEvents)builder.Events;
        RecordingInfiniFrameWindowSubstitute window = new RecordingInfiniFrameWindowSubstitute()
            .BindToBuilder(builder);

        events.WebMessageReceived.Add((sender, message) => builder.MessageHandlers.TryHandlePostDataRequest(sender, message));
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

    [Test]
    public async Task Registration_AcknowledgementIsSentAfterRegistrations() {
        // Arrange
        const string registrationMessageId = "__infiniframe:register:test";
        string readyEnvelope = InteropEnvelopeProtocol.CreateEnvelopeMessage("__infiniframe:ready");
        var builder = InfiniFrameWindowBuilder.Create();
        var events = (InfiniFrameWindowEvents)builder.Events;
        RecordingInfiniFrameWindowSubstitute window = new RecordingInfiniFrameWindowSubstitute()
            .BindToBuilder(builder);

        events.WebMessageReceived.Add((sender, message) => builder.MessageHandlers.TryHandlePostDataRequest(sender, message));
        events.CompleteSetup(window.Window);

        RegisterWindowCreatedUtility.RegisterWindowCreatedWebMessage(builder, registrationMessageId);

        // Act
        events.OnWindowCreated();
        events.OnWebMessageReceived(readyEnvelope);
        await Task.Delay(150);

        // Assert
        IReadOnlyList<string> sentMessages = window.GetSentMessagesSnapshot();
        int registrationIndex = FindMessageIndex(sentMessages, registrationMessageId);
        int ackIndex = FindMessageIndex(sentMessages, "__infiniframe:ready:ack");

        await Assert.That(registrationIndex).IsGreaterThanOrEqualTo(0);
        await Assert.That(ackIndex).IsGreaterThan(registrationIndex);
    }

    private static int FindMessageIndex(IReadOnlyList<string> sentMessages, string messageId)
        => sentMessages
            .Select((message, index) => new {
                ParseResult = InteropEnvelopeProtocol.ParseIncomingMessage(message),
                Index = index
            })
            .Where(item => item.ParseResult.Success && string.Equals(item.ParseResult.MessageId, messageId, StringComparison.Ordinal))
            .Select(item => item.Index)
            .FirstOrDefault(-1);
}
