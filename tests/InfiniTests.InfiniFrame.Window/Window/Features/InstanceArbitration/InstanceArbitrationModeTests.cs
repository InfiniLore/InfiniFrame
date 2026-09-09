// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.InstanceArbitration;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InstanceArbitrationModeTests {

    [Test]
    public async Task Disabled_IsDefault(CancellationToken ct) {
        // Arrange & Act
        InstanceArbitrationMode defaultValue = default;

        // Assert
        await Assert.That(defaultValue).IsEqualTo(InstanceArbitrationMode.Disabled);
    }

    [Test]
    public async Task Disabled_HasExpectedValue(CancellationToken ct) {
        // Arrange & Act
        int value = (int)InstanceArbitrationMode.Disabled;

        // Assert
        await Assert.That(value).IsEqualTo(0);
    }

    [Test]
    public async Task PrimaryOnly_HasExpectedValue(CancellationToken ct) {
        // Arrange & Act
        int value = (int)InstanceArbitrationMode.PrimaryOnly;

        // Assert
        await Assert.That(value).IsEqualTo(1);
    }

    [Test]
    public async Task PrimaryWithArgForwarding_HasExpectedValue(CancellationToken ct) {
        // Arrange & Act
        int value = (int)InstanceArbitrationMode.PrimaryWithArgForwarding;

        // Assert
        await Assert.That(value).IsEqualTo(2);
    }
}
