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
    [Arguments(450, 280)]
    public async Task AtWindowStage_DirectAssignment(int width, int height, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        int originalMinWidth = window.Features.Size.MinWidth;
        int originalMinHeight = window.Features.Size.MinHeight;
        int targetMinWidth = width == originalMinWidth ? width + 20 : width;
        int targetMinHeight = height == originalMinHeight ? height + 20 : height;

        // Act
        window.Features.Size.SetMinSize(targetMinWidth, targetMinHeight);

        // Assert
        int newMinWidth = await PollUtility.WaitForChangeAsync(() => window.Features.Size.MinWidth, originalMinWidth, TimeSpan.FromSeconds(5), ct);
        int newMinHeight = await PollUtility.WaitForChangeAsync(() => window.Features.Size.MinHeight, originalMinHeight, TimeSpan.FromSeconds(5), ct);
        await Assert.That(newMinWidth).IsEqualTo(targetMinWidth);
        await Assert.That(newMinHeight).IsEqualTo(targetMinHeight);
    }
}
