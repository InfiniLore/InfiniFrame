// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.Window.Features.InfiniFrameWindowFeatureDecorations;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class IconFileTests {
    [Test]
    [Arguments("C:/temp/infiniframe-icon-a.ico")]
    [Arguments("C:/temp/infiniframe-icon-b.ico")]
    public async Task AtBuilderStage_DirectAssignment(string value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.Features.Decorations.SetIconFile(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Decorations.IconFilePath).IsEqualTo(value);
        await Assert.That(initParameters.WindowIconFile).IsEqualTo(value);
    }

    [Test]
    [Arguments("C:/temp/infiniframe-icon-c.ico")]
    [Arguments("C:/temp/infiniframe-icon-d.ico")]
    public async Task AtBuilderStage_ExtensionAssignment(string value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.SetIconFile(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Decorations.IconFilePath).IsEqualTo(value);
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
        await Assert.That(initParameters.WindowIconFile).IsEqualTo(value);
    }
}
