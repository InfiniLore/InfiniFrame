// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.Window.Features.Browser;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class TemporaryFilesPathTests {

    [Test]
    public async Task AtBuilderStage_DefaultValueIsAppliedToNativeParameters(CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(initParameters.TemporaryFilesPath).IsEqualTo(builder.Features.Browser.TemporaryFilesPath);
        await Assert.That(initParameters.TemporaryFilesPath).IsNotNull();
        await Assert.That(initParameters.TemporaryFilesPath).IsNotEmpty();
    }

    [Test]
    public async Task AtBuilderStage_ExtensionAssignmentIsAppliedToNativeParameters(CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        const string expectedPath = "C:/temp/infiniframe-test";

        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.SetTemporaryFilesPath(expectedPath);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Browser.TemporaryFilesPath).IsEqualTo(expectedPath);
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
        await Assert.That(initParameters.TemporaryFilesPath).IsEqualTo(expectedPath);
    }
}