// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.Window.Features.Size;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class SetMaxWidthTests {
    [Test]
    [Arguments(1400)]
    [Arguments(1800)]
    public async Task AtBuilderStage_DirectAssignment(int value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.Features.Size.SetMaxWidth(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Size.MaxWidth).IsEqualTo(value);
        await Assert.That(initParameters.MaxWidth).IsEqualTo(value);
    }

    [Test]
    [Arguments(1500)]
    [Arguments(1900)]
    public async Task AtBuilderStage_ExtensionAssignment(int value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.SetMaxWidth(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Size.MaxWidth).IsEqualTo(value);
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
        await Assert.That(initParameters.MaxWidth).IsEqualTo(value);
    }

    [Test]
    [NotInParallelInfiniTests]
    [Arguments(1600)]
    public async Task AtWindowStage_DirectAssignment(int value, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        int originalMaxWidth = window.Features.Size.MaxWidth;
        int targetMaxWidth = value == originalMaxWidth ? value + 20 : value;

        // Act
        window.Features.Size.SetMaxWidth(targetMaxWidth);

        // Assert
        int newMaxWidth = await PollUtility.WaitForChangeAsync(getValue: () => window.Features.Size.MaxWidth, originalMaxWidth, TimeSpan.FromSeconds(5), ct);
        await Assert.That(newMaxWidth).IsEqualTo(targetMaxWidth);
    }

    [Test]
    [NotInParallelInfiniTests]
    [Arguments(1700)]
    public async Task AtWindowStage_ExtensionAssignment(int value, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        int originalMaxWidth = window.Features.Size.MaxWidth;
        int targetMaxWidth = value == originalMaxWidth ? value + 20 : value;

        // Act
        IInfiniFrameWindow returnedWindow = window.SetMaxWidth(targetMaxWidth);

        // Assert
        int newMaxWidth = await PollUtility.WaitForChangeAsync(getValue: () => window.Features.Size.MaxWidth, originalMaxWidth, TimeSpan.FromSeconds(5), ct);
        await Assert.That(newMaxWidth).IsEqualTo(targetMaxWidth);
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
    }
}
