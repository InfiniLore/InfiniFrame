// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.Window.Features.Size;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class SetMinSizeTests {
    [Test]
    [Arguments(400, 250)]
    [Arguments(500, 300)]
    public async Task AtBuilderStage_DirectAssignment(int width, int height, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.Features.Size.SetMinSize(width, height);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Size.MinWidth).IsEqualTo(width);
        await Assert.That(builder.Features.Size.MinHeight).IsEqualTo(height);
        await Assert.That(initParameters.MinWidth).IsEqualTo(width);
        await Assert.That(initParameters.MinHeight).IsEqualTo(height);
    }

    [Test]
    [Arguments(420, 260)]
    [Arguments(520, 320)]
    public async Task AtBuilderStage_ExtensionAssignment(int width, int height, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.SetMinSize(width, height);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Size.MinWidth).IsEqualTo(width);
        await Assert.That(builder.Features.Size.MinHeight).IsEqualTo(height);
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
        await Assert.That(initParameters.MinWidth).IsEqualTo(width);
        await Assert.That(initParameters.MinHeight).IsEqualTo(height);
    }

    [Test]
    [NotInParallelInfiniTests]
    [Arguments(450, 280)]
    public async Task AtWindowStage_DirectAssignment(int width, int height, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Features.Size.SetMinSize(width, height);

        // Assert
        await Assert.That(window.Features.Size.MinWidth).IsEqualTo(width);
        await Assert.That(window.Features.Size.MinHeight).IsEqualTo(height);
    }
}
