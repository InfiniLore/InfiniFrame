// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.Window.Features.Size;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class SetSizeTests {
    [Test]
    [Arguments(640, 360)]
    [Arguments(900, 540)]
    public async Task AtBuilderStage_DirectAssignment(int width, int height, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.Features.Size.SetSize(width, height);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Size.Width).IsEqualTo(width);
        await Assert.That(builder.Features.Size.Height).IsEqualTo(height);
        await Assert.That(builder.Features.Size.StartWithOsDefaultSize).IsFalse();
        await Assert.That(initParameters.Width).IsEqualTo(width);
        await Assert.That(initParameters.Height).IsEqualTo(height);
        await Assert.That(initParameters.UseOsDefaultSize).IsFalse();
    }

    [Test]
    [Arguments(800, 500)]
    [Arguments(1024, 768)]
    public async Task AtBuilderStage_ExtensionAssignment(int width, int height, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        System.Drawing.Size value = new(width, height);

        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.SetSize(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Size.Width).IsEqualTo(width);
        await Assert.That(builder.Features.Size.Height).IsEqualTo(height);
        await Assert.That(builder.Features.Size.StartWithOsDefaultSize).IsFalse();
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
        await Assert.That(initParameters.Width).IsEqualTo(width);
        await Assert.That(initParameters.Height).IsEqualTo(height);
        await Assert.That(initParameters.UseOsDefaultSize).IsFalse();
    }

    [Test]
    [NotInParallelInfiniTests]
    [Arguments(760, 420)]
    [Arguments(840, 460)]
    public async Task AtWindowStage_DirectAssignment(int width, int height, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        int originalWidth = window.Features.Size.Width;
        int originalHeight = window.Features.Size.Height;
        int targetWidth = width == originalWidth ? width + 20 : width;
        int targetHeight = height == originalHeight ? height + 20 : height;

        // Act
        window.Features.Size.SetSize(targetWidth, targetHeight);

        // Assert
        int updatedWidth = await PollUtility.WaitForChangeAsync(() => window.Features.Size.Width, originalWidth, TimeSpan.FromSeconds(5), ct);
        int updatedHeight = await PollUtility.WaitForChangeAsync(() => window.Features.Size.Height, originalHeight, TimeSpan.FromSeconds(5), ct);
        await Assert.That(updatedWidth).IsEqualTo(targetWidth);
        await Assert.That(updatedHeight).IsEqualTo(targetHeight);
    }

    [Test]
    [NotInParallelInfiniTests]
    [Arguments(780, 430)]
    [Arguments(860, 470)]
    public async Task AtWindowStage_ExtensionAssignment(int width, int height, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        int originalWidth = window.Features.Size.Width;
        int originalHeight = window.Features.Size.Height;
        int targetWidth = width == originalWidth ? width + 20 : width;
        int targetHeight = height == originalHeight ? height + 20 : height;

        // Act
        IInfiniFrameWindow returnedWindow = window.SetSize(targetWidth, targetHeight);

        // Assert
        int updatedWidth = await PollUtility.WaitForChangeAsync(() => window.Features.Size.Width, originalWidth, TimeSpan.FromSeconds(5), ct);
        int updatedHeight = await PollUtility.WaitForChangeAsync(() => window.Features.Size.Height, originalHeight, TimeSpan.FromSeconds(5), ct);
        await Assert.That(updatedWidth).IsEqualTo(targetWidth);
        await Assert.That(updatedHeight).IsEqualTo(targetHeight);
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
    }

    [Test]
    [NotInParallelInfiniTests]
    [Arguments(720, 410)]
    public async Task AtWindowStage_ThroughBuilderAssignment(int width, int height, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder => {
            builder.Features.Size.SetSize(width, height);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;
        IInfiniFrameWindowBuilder builder = windowUtility.BuilderSnapshot;

        // Assert
        await Assert.That(builder.Features.Size.Width).IsEqualTo(width);
        await Assert.That(builder.Features.Size.Height).IsEqualTo(height);
        await Assert.That(window.Features.Size.Width).IsEqualTo(width);
        await Assert.That(window.Features.Size.Height).IsEqualTo(height);
    }
}
