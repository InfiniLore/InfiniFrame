// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.Window.Features.Size;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class SetWidthTests {
    [Test]
    [Arguments(640)]
    [Arguments(980)]
    public async Task AtBuilderStage_DirectAssignment(int value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.Features.Size.SetWidth(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Size.Width).IsEqualTo(value);
        await Assert.That(builder.Features.Size.StartWithOsDefaultSize).IsFalse();
        await Assert.That(initParameters.Width).IsEqualTo(value);
        await Assert.That(initParameters.UseOsDefaultSize).IsFalse();
    }

    [Test]
    [Arguments(660)]
    [Arguments(1000)]
    public async Task AtBuilderStage_ExtensionAssignment(int value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.SetWidth(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Size.Width).IsEqualTo(value);
        await Assert.That(builder.Features.Size.StartWithOsDefaultSize).IsFalse();
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
        await Assert.That(initParameters.Width).IsEqualTo(value);
        await Assert.That(initParameters.UseOsDefaultSize).IsFalse();
    }

    [Test]
    [NotInParallelInfiniTests]
    [Arguments(700)]
    public async Task AtWindowStage_DirectAssignment(int value, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        int originalWidth = window.Features.Size.Width;
        int targetWidth = value == originalWidth ? value + 20 : value;

        // Act
        window.Features.Size.SetWidth(targetWidth);

        // Assert
        int newWidth = await PollUtility.WaitForChangeAsync(getValue: () => window.Features.Size.Width, originalWidth, TimeSpan.FromSeconds(5), ct);
        await Assert.That(newWidth).IsEqualTo(targetWidth);
    }

    [Test]
    [NotInParallelInfiniTests]
    [Arguments(740)]
    public async Task AtWindowStage_ExtensionAssignment(int value, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        int originalWidth = window.Features.Size.Width;
        int targetWidth = value == originalWidth ? value + 20 : value;

        // Act
        IInfiniFrameWindow returnedWindow = window.SetWidth(targetWidth);

        // Assert
        int newWidth = await PollUtility.WaitForChangeAsync(getValue: () => window.Features.Size.Width, originalWidth, TimeSpan.FromSeconds(5), ct);
        await Assert.That(newWidth).IsEqualTo(targetWidth);
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
    }
}
