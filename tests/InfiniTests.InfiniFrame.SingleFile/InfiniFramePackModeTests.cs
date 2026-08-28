// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.SingleFile;

namespace InfiniTests.InfiniFrame.SingleFile;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[NotInParallelInfiniTests]
public class InfiniFramePackModeTests {

    [Test]
    public async Task IsActive_CanBeSetToTrue(CancellationToken ct = default) {
        // Arrange
        InfiniFramePackMode.IsActive = false;

        // Act
        InfiniFramePackMode.IsActive = true;

        // Assert
        await Assert.That(InfiniFramePackMode.IsActive).IsTrue();

        // Cleanup
        InfiniFramePackMode.IsActive = false;
    }

    [Test]
    public async Task IsActive_CanBeSetToFalse(CancellationToken ct = default) {
        // Arrange
        InfiniFramePackMode.IsActive = true;

        // Act
        InfiniFramePackMode.IsActive = false;

        // Assert
        await Assert.That(InfiniFramePackMode.IsActive).IsFalse();
    }

    [Test]
    public async Task IsActive_CanBeToggled(CancellationToken ct = default) {
        // Arrange
        InfiniFramePackMode.IsActive = false;

        // Act & Assert
        await Assert.That(InfiniFramePackMode.IsActive).IsFalse();

        InfiniFramePackMode.IsActive = true;
        await Assert.That(InfiniFramePackMode.IsActive).IsTrue();

        InfiniFramePackMode.IsActive = false;
        await Assert.That(InfiniFramePackMode.IsActive).IsFalse();

        // Cleanup
        InfiniFramePackMode.IsActive = false;
    }

    [Test]
    public async Task IsActive_IsPublicField(CancellationToken ct = default) {
        // Arrange
        var field = typeof(InfiniFramePackMode).GetField("IsActive");

        // Act (no-op — verifying reflection metadata)

        // Assert
        await Assert.That(field).IsNotNull();
        await Assert.That(field!.IsPublic).IsTrue();
        await Assert.That(field.IsStatic).IsTrue();
    }
}
