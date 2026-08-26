// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.Window.Features.Size;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class SetMaxSizeTests {
    [Test]
    [Arguments(1600, 900)]
    [Arguments(1920, 1080)]
    public async Task AtBuilderStage_DirectAssignment(int width, int height, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.Features.Size.SetMaxSize(width, height);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Size.MaxWidth).IsEqualTo(width);
        await Assert.That(builder.Features.Size.MaxHeight).IsEqualTo(height);
        await Assert.That(initParameters.MaxWidth).IsEqualTo(width);
        await Assert.That(initParameters.MaxHeight).IsEqualTo(height);
    }

    [Test]
    [Arguments(1700, 950)]
    [Arguments(2000, 1120)]
    public async Task AtBuilderStage_ExtensionAssignment(int width, int height, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.SetMaxSize(width, height);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Size.MaxWidth).IsEqualTo(width);
        await Assert.That(builder.Features.Size.MaxHeight).IsEqualTo(height);
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
        await Assert.That(initParameters.MaxWidth).IsEqualTo(width);
        await Assert.That(initParameters.MaxHeight).IsEqualTo(height);
    }

    [Test]
    [NotInParallelInfiniTests]
    [Arguments(1800, 1000)]
    public async Task AtWindowStage_DirectAssignment(int width, int height, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        int originalMaxWidth = window.Features.Size.MaxWidth;
        int originalMaxHeight = window.Features.Size.MaxHeight;
        int targetMaxWidth = width == originalMaxWidth ? width + 20 : width;
        int targetMaxHeight = height == originalMaxHeight ? height + 20 : height;

        // Act
        window.Features.Size.SetMaxSize(targetMaxWidth, targetMaxHeight);

        // Assert
        int newMaxWidth = await PollUtility.WaitForChangeAsync(getValue: () => window.Features.Size.MaxWidth, originalMaxWidth, TimeSpan.FromSeconds(5), ct);
        int newMaxHeight = await PollUtility.WaitForChangeAsync(getValue: () => window.Features.Size.MaxHeight, originalMaxHeight, TimeSpan.FromSeconds(5), ct);
        await Assert.That(newMaxWidth).IsEqualTo(targetMaxWidth);
        await Assert.That(newMaxHeight).IsEqualTo(targetMaxHeight);
    }
}
