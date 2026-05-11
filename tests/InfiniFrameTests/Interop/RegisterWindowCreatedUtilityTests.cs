// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Interop;
using InfiniFrame.Native;
using InfiniFrameTests.Shared.TestDoubles;

namespace InfiniFrameTests.Interop;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class RegisterWindowCreatedUtilityTests {
    [Test]
    public async Task Registration_IsGatedByWindowReadyHandshake(CancellationToken ct = default) {
        // Arrange
        const string registrationMessageId = "__infiniframe:register:test";
        string readyEnvelope = InteropEnvelopeProtocol.CreateEnvelopeMessage("__infiniframe:ready");
        var builder = InfiniFrameWindowBuilder.Create();
        var eventsStore = (InfiniFrameEventsStore)builder.EventsStore;
        var events = new InfiniFrameEvents(eventsStore);
        RecordingInfiniFrameWindowSubstitute window = new RecordingInfiniFrameWindowSubstitute()
            .BindToBuilder(builder);

        var nativeParameters = default(InfiniFrameNativeParameters);
        events.AssignEventCallbacks(ref nativeParameters);
        events.AssignSender(window.Window);

        RegisterWindowCreatedUtility.RegisterWindowCreatedWebMessage(builder, registrationMessageId);

        // Act
        events.OnWindowCreated();
        await Task.Delay(150, ct);

        // Assert pre-ready: nothing should be sent yet.
        int sendAttemptsBeforeReady = window.CountEnvelopeMessagesById(registrationMessageId);
        await Assert.That(sendAttemptsBeforeReady).IsEqualTo(0);

        // Act: explicit ready handshake.
        events.OnWebMessageReceived(readyEnvelope);
        await Task.Delay(150, ct);

        // Assert: exactly one registration send attempt after ready.
        int sendAttemptsAfterReady = window.CountEnvelopeMessagesById(registrationMessageId);
        await Assert.That(sendAttemptsAfterReady).IsEqualTo(1);

        int ackAttemptsAfterReady = window.CountEnvelopeMessagesById("__infiniframe:ready:ack");
        await Assert.That(ackAttemptsAfterReady).IsEqualTo(1);
    }

    [Test]
    public async Task Registration_IsIdempotentAcrossRepeatedReadyMessages(CancellationToken ct = default) {
        // Arrange
        const string registrationMessageId = "__infiniframe:register:test";
        string readyEnvelope = InteropEnvelopeProtocol.CreateEnvelopeMessage("__infiniframe:ready");
        var builder = InfiniFrameWindowBuilder.Create();
        var eventsStore = (InfiniFrameEventsStore)builder.EventsStore;
        var events = new InfiniFrameEvents(eventsStore);
        RecordingInfiniFrameWindowSubstitute window = new RecordingInfiniFrameWindowSubstitute()
            .BindToBuilder(builder);

        var nativeParameters = default(InfiniFrameNativeParameters);
        events.AssignEventCallbacks(ref nativeParameters);
        events.AssignSender(window.Window);

        RegisterWindowCreatedUtility.RegisterWindowCreatedWebMessage(builder, registrationMessageId);

        // Act: ready envelope received multiple times.
        events.OnWebMessageReceived(readyEnvelope);
        events.OnWebMessageReceived(readyEnvelope);
        await Task.Delay(150, ct);

        // Assert: only one registration send attempt.
        int sendAttempts = window.CountEnvelopeMessagesById(registrationMessageId);
        await Assert.That(sendAttempts).IsEqualTo(1);
    }

    [Test]
    public async Task Registration_AcknowledgementIsSentAfterRegistrations(CancellationToken ct = default) {
        // Arrange
        const string registrationMessageId = "__infiniframe:register:test";
        string readyEnvelope = InteropEnvelopeProtocol.CreateEnvelopeMessage("__infiniframe:ready");
        var builder = InfiniFrameWindowBuilder.Create();
        var eventsStore = (InfiniFrameEventsStore)builder.EventsStore;
        var events = new InfiniFrameEvents(eventsStore);
        RecordingInfiniFrameWindowSubstitute window = new RecordingInfiniFrameWindowSubstitute()
            .BindToBuilder(builder);

        var nativeParameters = default(InfiniFrameNativeParameters);
        events.AssignEventCallbacks(ref nativeParameters);
        events.AssignSender(window.Window);

        RegisterWindowCreatedUtility.RegisterWindowCreatedWebMessage(builder, registrationMessageId);

        // Act
        events.OnWindowCreated();
        events.OnWebMessageReceived(readyEnvelope);
        await Task.Delay(150, ct);

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
            .Where(item => item.ParseResult.IsSuccess && string.Equals(item.ParseResult.MessageId, messageId, StringComparison.Ordinal))
            .Select(item => item.Index)
            .FirstOrDefault(-1);
}
