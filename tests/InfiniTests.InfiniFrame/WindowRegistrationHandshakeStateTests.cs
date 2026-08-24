// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Interop;

namespace InfiniTests.InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowRegistrationHandshakeStateTests {

    [Test]
    public async Task ReadyPending_IsFirstValue(CancellationToken ct = default) {
        // Arrange & Act & Assert
        var value = WindowRegistrationHandshakeState.ReadyPending;
        await Assert.That(value).IsEqualTo(WindowRegistrationHandshakeState.ReadyPending);
    }

    [Test]
    public async Task RegistrationSending_IsSecondValue(CancellationToken ct = default) {
        // Arrange & Act & Assert
        var value = WindowRegistrationHandshakeState.RegistrationSending;
        await Assert.That(value).IsEqualTo(WindowRegistrationHandshakeState.RegistrationSending);
    }

    [Test]
    public async Task ReadyAcknowledged_IsThirdValue(CancellationToken ct = default) {
        // Arrange & Act & Assert
        var value = WindowRegistrationHandshakeState.ReadyAcknowledged;
        await Assert.That(value).IsEqualTo(WindowRegistrationHandshakeState.ReadyAcknowledged);
    }

    [Test]
    public async Task Failed_IsFourthValue(CancellationToken ct = default) {
        // Arrange & Act & Assert
        var value = WindowRegistrationHandshakeState.Failed;
        await Assert.That(value).IsEqualTo(WindowRegistrationHandshakeState.Failed);
    }

    [Test]
    public async Task AllValues_CanBeIterated(CancellationToken ct = default) {
        // Arrange
        WindowRegistrationHandshakeState[] values = Enum.GetValues<WindowRegistrationHandshakeState>();

        // Act
        int count = values.Length;

        // Assert
        await Assert.That(count).IsEqualTo(4);
    }
}
