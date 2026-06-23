// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.Window.Features.Decorations;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class TransparentTests {
    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task AtBuilderStage_DirectAssignment(bool value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.Features.Decorations.SetTransparent(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Decorations.IsTransparent).IsEqualTo(value);
        await Assert.That(initParameters.Transparent).IsEqualTo(value);
    }

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task AtBuilderStage_ExtensionAssignment(bool value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.SetTransparent(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Decorations.IsTransparent).IsEqualTo(value);
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
        await Assert.That(initParameters.Transparent).IsEqualTo(value);
    }

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task AtWindowStage_ThroughBuilderAssignment(bool value, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder => {
            builder.Features.Decorations.SetTransparent(value);
        }, ct);
        IInfiniFrameWindowBuilder builder = windowUtility.BuilderSnapshot;

        // Act

        // Assert
        await Assert.That(builder.Features.Decorations.IsTransparent).IsEqualTo(value);
    }
}
