// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Interop;

namespace InfiniTests.InfiniFrame.Interop;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowRegistrationStateMachineTests {

    // -----------------------------------------------------------------------------------------------------------------
    // Test Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task InitialState_IsReadyPending(CancellationToken ct = default) {
        // Arrange
        var stateMachine = new WindowRegistrationStateMachine();

        // Act & Assert
        await Assert.That(stateMachine.IsReadyPending()).IsTrue();
    }

    [Test]
    public async Task TryBeginRegistrationSendOnReady_WhenReadyPending_ShouldReturnTrue(CancellationToken ct = default) {
        // Arrange
        var stateMachine = new WindowRegistrationStateMachine();

        // Act
        bool result = stateMachine.TryBeginRegistrationSendOnReady();

        // Assert
        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task TryBeginRegistrationSendOnReady_WhenAlreadyInProgress_ShouldReturnFalse(CancellationToken ct = default) {
        // Arrange
        var stateMachine = new WindowRegistrationStateMachine();
        stateMachine.TryBeginRegistrationSendOnReady();

        // Act
        bool result = stateMachine.TryBeginRegistrationSendOnReady();

        // Assert
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task CompleteRegistrationSend_Success_ShouldMakeReadyPendingFalse(CancellationToken ct = default) {
        // Arrange
        var stateMachine = new WindowRegistrationStateMachine();
        stateMachine.TryBeginRegistrationSendOnReady();

        // Act
        stateMachine.CompleteRegistrationSend(true);

        // Assert
        await Assert.That(stateMachine.IsReadyPending()).IsFalse();
    }

    [Test]
    public async Task CompleteRegistrationSend_Failure_ShouldMakeReadyPendingFalse(CancellationToken ct = default) {
        // Arrange
        var stateMachine = new WindowRegistrationStateMachine();
        stateMachine.TryBeginRegistrationSendOnReady();

        // Act
        stateMachine.CompleteRegistrationSend(false);

        // Assert
        await Assert.That(stateMachine.IsReadyPending()).IsFalse();
    }

    [Test]
    public async Task CompleteRegistrationSend_Success_CanBeginNewRegistration(CancellationToken ct = default) {
        // Arrange
        var stateMachine = new WindowRegistrationStateMachine();
        stateMachine.TryBeginRegistrationSendOnReady();
        stateMachine.CompleteRegistrationSend(true);

        // Act - Should not be able to begin again since it's acknowledged
        bool result = stateMachine.TryBeginRegistrationSendOnReady();

        // Assert
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task CompleteRegistrationSend_Failure_CanBeginNewRegistration(CancellationToken ct = default) {
        // Arrange
        var stateMachine = new WindowRegistrationStateMachine();
        stateMachine.TryBeginRegistrationSendOnReady();
        stateMachine.CompleteRegistrationSend(false);

        // Act
        bool result = stateMachine.TryBeginRegistrationSendOnReady();

        // Assert
        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task WindowRegistrationState_ShouldExposeStateMachine(CancellationToken ct = default) {
        // Arrange

        // Act
        var state = new WindowRegistrationState();

        // Assert
        await Assert.That(state.StateMachine).IsNotNull();
    }
}
