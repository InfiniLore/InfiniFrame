// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Interop;

namespace InfiniTests.InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowReadyRegistrationStateTests {

    [Test]
    public async Task Properties_DefaultToFalse(CancellationToken ct = default) {
        // Arrange & Act
        var state = new WindowReadyRegistrationState();

        // Assert
        await Assert.That(state.ReadyHandlerRegistered).IsFalse();
        await Assert.That(state.WindowCreatedHandlerRegistered).IsFalse();
    }

    [Test]
    public async Task RegistrationMessageIds_IsInitialized(CancellationToken ct = default) {
        // Arrange & Act
        var state = new WindowReadyRegistrationState();

        // Assert
        await Assert.That(state.RegistrationMessageIds).IsNotNull();
    }

    [Test]
    public async Task Windows_IsInitialized(CancellationToken ct = default) {
        // Arrange & Act
        var state = new WindowReadyRegistrationState();

        // Assert
        await Assert.That(state.Windows).IsNotNull();
    }

    [Test]
    public async Task Lock_IsInitialized(CancellationToken ct = default) {
        // Arrange & Act
        var state = new WindowReadyRegistrationState();

        // Assert
        await Assert.That(state.Lock).IsNotNull();
    }

    [Test]
    public async Task ReadyHandlerRegistered_CanBeSetToTrue(CancellationToken ct = default) {
        // Arrange
        var state = new WindowReadyRegistrationState();

        // Act
        state.ReadyHandlerRegistered = true;

        // Assert
        await Assert.That(state.ReadyHandlerRegistered).IsTrue();
    }

    [Test]
    public async Task WindowCreatedHandlerRegistered_CanBeSetToTrue(CancellationToken ct = default) {
        // Arrange
        var state = new WindowReadyRegistrationState();

        // Act
        state.WindowCreatedHandlerRegistered = true;

        // Assert
        await Assert.That(state.WindowCreatedHandlerRegistered).IsTrue();
    }
}
