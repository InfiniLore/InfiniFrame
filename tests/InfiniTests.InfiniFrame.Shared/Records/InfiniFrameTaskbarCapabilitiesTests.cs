// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Shared.Records;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameTaskbarCapabilitiesTests {

    [Test]
    public async Task Record_CanBeConstructed(CancellationToken ct = default) {
        // Arrange & Act
        var caps = new InfiniFrameTaskbarCapabilities {
            SupportsProgress = true,
            SupportsFlash = false
        };

        // Assert
        await Assert.That(caps.SupportsProgress).IsTrue();
        await Assert.That(caps.SupportsFlash).IsFalse();
    }

    [Test]
    public async Task Equality_SameValues_ReturnsTrue(CancellationToken ct = default) {
        // Arrange
        var caps1 = new InfiniFrameTaskbarCapabilities { SupportsProgress = true, SupportsFlash = true };
        var caps2 = new InfiniFrameTaskbarCapabilities { SupportsProgress = true, SupportsFlash = true };

        // Act & Assert
        await Assert.That(caps1).IsEqualTo(caps2);
    }

    [Test]
    public async Task Equality_DifferentValues_ReturnsFalse(CancellationToken ct = default) {
        // Arrange
        var caps1 = new InfiniFrameTaskbarCapabilities { SupportsProgress = true, SupportsFlash = false };
        var caps2 = new InfiniFrameTaskbarCapabilities { SupportsProgress = false, SupportsFlash = false };

        // Act & Assert
        await Assert.That(caps1).IsNotEqualTo(caps2);
    }
}
