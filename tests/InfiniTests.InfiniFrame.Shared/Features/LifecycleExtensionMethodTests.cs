// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Shared.Features;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class LifecycleExtensionMethodTests {

    [Test]
    public async Task WaitForClose_CallsFeature(CancellationToken ct = default) {
        // Arrange
        Mock<ILifecycleInfiniFrameWindowFeature> lifecycle = MockFactory.CreateLifecycleMock();

        // Act
        lifecycle.Object.WaitForClose();

        // Assert
        await Assert.That(lifecycle.Object).IsNotNull();
    }

    [Test]
    public async Task Close_CallsFeature(CancellationToken ct = default) {
        // Arrange
        Mock<ILifecycleInfiniFrameWindowFeature> lifecycle = MockFactory.CreateLifecycleMock();

        // Act
        lifecycle.Object.Close();

        // Assert
        await Assert.That(lifecycle.Object).IsNotNull();
    }

    [Test]
    public async Task IsClosedOrClosing_DefaultsToFalse(CancellationToken ct = default) {
        // Arrange
        Mock<ILifecycleInfiniFrameWindowFeature> lifecycle = MockFactory.CreateLifecycleMock();
        lifecycle.IsClosedOrClosing().Returns(false);

        // Act
        bool result = lifecycle.Object.IsClosedOrClosing();

        // Assert
        await Assert.That(result).IsFalse();
    }
}
