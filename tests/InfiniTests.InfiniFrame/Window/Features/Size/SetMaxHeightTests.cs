// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.Window.Features.Size;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class SetMaxHeightTests {
    [Test]
    [Arguments(800)]
    [Arguments(1000)]
    public async Task AtBuilderStage_DirectAssignment(int value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.Features.Size.SetMaxHeight(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Size.MaxHeight).IsEqualTo(value);
        await Assert.That(initParameters.MaxHeight).IsEqualTo(value);
    }

    [Test]
    [Arguments(860)]
    [Arguments(1080)]
    public async Task AtBuilderStage_ExtensionAssignment(int value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.SetMaxHeight(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Size.MaxHeight).IsEqualTo(value);
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
        await Assert.That(initParameters.MaxHeight).IsEqualTo(value);
    }

    [Test]
    [NotInParallelInfiniTests]
    [Arguments(920)]
    public async Task AtWindowStage_DirectAssignment(int value, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        int originalMaxHeight = window.Features.Size.MaxHeight;
        int targetMaxHeight = value == originalMaxHeight ? value + 20 : value;

        // Act
        window.Features.Size.SetMaxHeight(targetMaxHeight);

        // Assert
        int newMaxHeight = await PollUtility.WaitForChangeAsync(getValue: () => window.Features.Size.MaxHeight, originalMaxHeight, TimeSpan.FromSeconds(5), ct);
        await Assert.That(newMaxHeight).IsEqualTo(targetMaxHeight);
    }

    [Test]
    [NotInParallelInfiniTests]
    [Arguments(980)]
    public async Task AtWindowStage_ExtensionAssignment(int value, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        int originalMaxHeight = window.Features.Size.MaxHeight;
        int targetMaxHeight = value == originalMaxHeight ? value + 20 : value;

        // Act
        IInfiniFrameWindow returnedWindow = window.SetMaxHeight(targetMaxHeight);

        // Assert
        int newMaxHeight = await PollUtility.WaitForChangeAsync(getValue: () => window.Features.Size.MaxHeight, originalMaxHeight, TimeSpan.FromSeconds(5), ct);
        await Assert.That(newMaxHeight).IsEqualTo(targetMaxHeight);
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
    }
}
