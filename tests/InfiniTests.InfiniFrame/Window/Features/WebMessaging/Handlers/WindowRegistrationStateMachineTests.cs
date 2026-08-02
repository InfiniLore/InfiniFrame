// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Interop;

namespace InfiniTests.InfiniFrame.Window.Features.WebMessaging.Handlers;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowRegistrationStateMachineTests {
    [Test]
    public async Task InitialState_IsReadyPending_AndTimeoutEligible(CancellationToken ct = default) {
        // Arrange
        var sut = new WindowRegistrationStateMachine();

        // Assert
        await Assert.That(sut.IsReadyPending()).IsTrue();
    }

    [Test]
    public async Task TryBeginRegistrationSendOnReady_ReturnsFalse_WhenSendAlreadyInProgress(CancellationToken ct = default) {
        // Arrange
        var sut = new WindowRegistrationStateMachine();

        // Act
        bool firstStart = sut.TryBeginRegistrationSendOnReady();
        bool secondStart = sut.TryBeginRegistrationSendOnReady();

        // Assert
        await Assert.That(firstStart).IsTrue();
        await Assert.That(secondStart).IsFalse();
        await Assert.That(sut.IsReadyPending()).IsFalse();
    }

    [Test]
    public async Task CompleteRegistrationSend_SuccessfulAck_BlocksFurtherSends(CancellationToken ct = default) {
        // Arrange
        var sut = new WindowRegistrationStateMachine();
        sut.TryBeginRegistrationSendOnReady();

        // Act
        sut.CompleteRegistrationSend(success: true);
        bool startedAgain = sut.TryBeginRegistrationSendOnReady();

        // Assert
        await Assert.That(startedAgain).IsFalse();
        await Assert.That(sut.IsReadyPending()).IsFalse();
    }

    [Test]
    public async Task CompleteRegistrationSend_Failure_AllowsRetry(CancellationToken ct = default) {
        // Arrange
        var sut = new WindowRegistrationStateMachine();
        sut.TryBeginRegistrationSendOnReady();

        // Act
        sut.CompleteRegistrationSend(success: false);

        bool startedRetry = sut.TryBeginRegistrationSendOnReady();

        // Assert
        await Assert.That(startedRetry).IsTrue();
        await Assert.That(sut.IsReadyPending()).IsFalse();
    }

    [Test]
    public async Task CompleteRegistrationSend_Failure_WithoutReady_DoesNotEnableTimeoutLogging(CancellationToken ct = default) {
        // Arrange
        var sut = new WindowRegistrationStateMachine();

        // Act
        sut.CompleteRegistrationSend(success: false);
        bool started = sut.TryBeginRegistrationSendOnReady();

        // Assert
        await Assert.That(started).IsTrue();
        await Assert.That(sut.IsReadyPending()).IsFalse();
    }
}