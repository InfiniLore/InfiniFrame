// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.InfiniFrameWindowFeatureSize;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ResizeTests {
    [Test]
    [SkipOnMacOs]
    [SkipOnLinux]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_DirectAssignment(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder => {
            builder.Features.Size.SetSize(700, 420);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;
        int initialWidth = window.Features.Size.Width;
        int initialHeight = window.Features.Size.Height;

        // Act
        window.Features.Size.Resize(120, 60, ResizeOrigin.BottomRight);

        // Assert
        await Assert.That(window.Features.Size.Width).IsGreaterThan(initialWidth);
        await Assert.That(window.Features.Size.Height).IsGreaterThan(initialHeight);
    }

    [Test]
    [SkipOnMacOs]
    [SkipOnLinux]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_ExtensionAssignment(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder => {
            builder.Features.Size.SetSize(720, 440);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;
        int initialWidth = window.Features.Size.Width;
        int initialHeight = window.Features.Size.Height;

        // Act
        IInfiniFrameWindow returnedWindow = window.Resize(100, 40, ResizeOrigin.BottomRight);

        // Assert
        await Assert.That(window.Features.Size.Width).IsGreaterThan(initialWidth);
        await Assert.That(window.Features.Size.Height).IsGreaterThan(initialHeight);
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
    }
}
