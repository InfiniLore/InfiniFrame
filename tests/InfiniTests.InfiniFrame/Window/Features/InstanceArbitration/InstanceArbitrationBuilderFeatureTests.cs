// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.InstanceArbitration;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InstanceArbitrationBuilderFeatureTests {

    [Test]
    public async Task AtBuilderStage_DirectAssignment(CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.Features.InstanceArbitration.SetMode(InstanceArbitrationMode.PrimaryOnly);
        builder.Features.InstanceArbitration.SetMutexName("Test.Mutex");
        builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.InstanceArbitration.Mode).IsEqualTo(InstanceArbitrationMode.PrimaryOnly);
        await Assert.That(builder.Features.InstanceArbitration.MutexName).IsEqualTo("Test.Mutex");
    }

    [Test]
    public async Task AtBuilderStage_ExtensionAssignment(CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder
            .SetInstanceArbitrationMode(InstanceArbitrationMode.PrimaryOnly)
            .SetInstanceArbitrationMutexName("Test.Mutex");
        builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.InstanceArbitration.Mode).IsEqualTo(InstanceArbitrationMode.PrimaryOnly);
        await Assert.That(builder.Features.InstanceArbitration.MutexName).IsEqualTo("Test.Mutex");
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
    }

    [Test]
    public async Task AtBuilderStage_DefaultValues(CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.InstanceArbitration.Mode).IsEqualTo(InstanceArbitrationMode.Disabled);
        await Assert.That(builder.Features.InstanceArbitration.MutexName).IsNull();
    }

    [Test]
    public async Task AtBuilderStage_MutexName(CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.Features.InstanceArbitration.SetMutexName("Custom.App.Mutex");

        // Assert
        await Assert.That(builder.Features.InstanceArbitration.MutexName).IsEqualTo("Custom.App.Mutex");
    }
}
