// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Js.Interop;

namespace InfiniFrameTests.Js;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowRegistrationStateMachineTests {
    [Test]
    [DisplayName($"{nameof(WindowRegistrationStateMachineTests)}.{nameof(InitialState_IsReadyPending_AndTimeoutEligible)}")]
    public async Task InitialState_IsReadyPending_AndTimeoutEligible() {
        var sut = new WindowRegistrationStateMachine();

        await Assert.That(sut.ShouldLogReadyHandshakeTimeout()).IsTrue();
    }

    [Test]
    [DisplayName($"{nameof(WindowRegistrationStateMachineTests)}.{nameof(TryBeginRegistrationSendOnReady_ReturnsFalse_WhenSendAlreadyInProgress)}")]
    public async Task TryBeginRegistrationSendOnReady_ReturnsFalse_WhenSendAlreadyInProgress() {
        var sut = new WindowRegistrationStateMachine();

        bool firstStart = sut.TryBeginRegistrationSendOnReady();
        bool secondStart = sut.TryBeginRegistrationSendOnReady();

        await Assert.That(firstStart).IsTrue();
        await Assert.That(secondStart).IsFalse();
        await Assert.That(sut.ShouldLogReadyHandshakeTimeout()).IsFalse();
    }

    [Test]
    [DisplayName($"{nameof(WindowRegistrationStateMachineTests)}.{nameof(CompleteRegistrationSend_Success_BlocksFurtherSends)}")]
    public async Task CompleteRegistrationSend_Success_BlocksFurtherSends() {
        var sut = new WindowRegistrationStateMachine();
        sut.TryBeginRegistrationSendOnReady();

        sut.CompleteRegistrationSend(success: true);
        bool startedAgain = sut.TryBeginRegistrationSendOnReady();

        await Assert.That(startedAgain).IsFalse();
        await Assert.That(sut.ShouldLogReadyHandshakeTimeout()).IsFalse();
    }

    [Test]
    [DisplayName($"{nameof(WindowRegistrationStateMachineTests)}.{nameof(CompleteRegistrationSend_Failure_AllowsRetry)}")]
    public async Task CompleteRegistrationSend_Failure_AllowsRetry() {
        var sut = new WindowRegistrationStateMachine();
        sut.TryBeginRegistrationSendOnReady();
        sut.CompleteRegistrationSend(success: false);

        bool startedRetry = sut.TryBeginRegistrationSendOnReady();

        await Assert.That(startedRetry).IsTrue();
        await Assert.That(sut.ShouldLogReadyHandshakeTimeout()).IsFalse();
    }

    [Test]
    [DisplayName($"{nameof(WindowRegistrationStateMachineTests)}.{nameof(CompleteRegistrationSend_Failure_WithoutReady_DoesNotEnableTimeoutLogging)}")]
    public async Task CompleteRegistrationSend_Failure_WithoutReady_DoesNotEnableTimeoutLogging() {
        var sut = new WindowRegistrationStateMachine();

        sut.CompleteRegistrationSend(success: false);
        bool started = sut.TryBeginRegistrationSendOnReady();

        await Assert.That(started).IsTrue();
        await Assert.That(sut.ShouldLogReadyHandshakeTimeout()).IsFalse();
    }
}
