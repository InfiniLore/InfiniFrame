// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.Window.Features.InfiniFrameWindowFeaturePosition;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class CenteredOnMainMonitorTests {
    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task AtBuilderStage_DirectAssignment(bool value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.Features.Position.CenteredOnMainMonitor(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Position.StartCentered).IsEqualTo(value);
        await Assert.That(builder.Features.Position.StartAtOsDefaultLocation).IsEqualTo(!value);
        await Assert.That(initParameters.CenterOnInitialize).IsEqualTo(value);
        await Assert.That(initParameters.UseOsDefaultLocation).IsEqualTo(!value);
    }

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task AtBuilderStage_ExtensionAssignment(bool value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.CenteredOnMainMonitor(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Position.StartCentered).IsEqualTo(value);
        await Assert.That(builder.Features.Position.StartAtOsDefaultLocation).IsEqualTo(!value);
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
        await Assert.That(initParameters.CenterOnInitialize).IsEqualTo(value);
        await Assert.That(initParameters.UseOsDefaultLocation).IsEqualTo(!value);
    }
}
