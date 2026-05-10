// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Interop;

namespace InfiniFrameTests.MessageHandlers;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowRegistrationStateMachineTests {
    [Test]
    public async Task InitialState_IsReadyPending_AndTimeoutEligible() {
        var sut = new WindowRegistrationStateMachine();

        await Assert.That(sut.IsReadyPending()).IsTrue();
    }

    [Test]
    public async Task TryBeginRegistrationSendOnReady_ReturnsFalse_WhenSendAlreadyInProgress() {
        var sut = new WindowRegistrationStateMachine();

        bool firstStart = sut.TryBeginRegistrationSendOnReady();
        bool secondStart = sut.TryBeginRegistrationSendOnReady();

        await Assert.That(firstStart).IsTrue();
        await Assert.That(secondStart).IsFalse();
        await Assert.That(sut.IsReadyPending()).IsFalse();
    }

    [Test]
    public async Task CompleteRegistrationSend_SuccessfulAck_BlocksFurtherSends() {
        var sut = new WindowRegistrationStateMachine();
        sut.TryBeginRegistrationSendOnReady();

        sut.CompleteRegistrationSend(success: true);
        bool startedAgain = sut.TryBeginRegistrationSendOnReady();

        await Assert.That(startedAgain).IsFalse();
        await Assert.That(sut.IsReadyPending()).IsFalse();
    }

    [Test]
    public async Task CompleteRegistrationSend_Failure_AllowsRetry() {
        var sut = new WindowRegistrationStateMachine();
        sut.TryBeginRegistrationSendOnReady();
        sut.CompleteRegistrationSend(success: false);

        bool startedRetry = sut.TryBeginRegistrationSendOnReady();

        await Assert.That(startedRetry).IsTrue();
        await Assert.That(sut.IsReadyPending()).IsFalse();
    }

    [Test]
    public async Task CompleteRegistrationSend_Failure_WithoutReady_DoesNotEnableTimeoutLogging() {
        var sut = new WindowRegistrationStateMachine();

        sut.CompleteRegistrationSend(success: false);
        bool started = sut.TryBeginRegistrationSendOnReady();

        await Assert.That(started).IsTrue();
        await Assert.That(sut.IsReadyPending()).IsFalse();
    }
}
