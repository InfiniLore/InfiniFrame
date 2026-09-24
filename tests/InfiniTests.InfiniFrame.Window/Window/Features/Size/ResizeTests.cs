// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.Size;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ResizeTests {
    [Test]
    [SkipOnLinux]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_DirectAssignment(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            builder.Features.Size.SetSize(700, 420);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;
        int initialWidth = window.Features.Size.Width;
        int initialHeight = window.Features.Size.Height;

        // Act
        window.Features.Size.Resize(120, 60, ResizeOrigin.BottomRight);

        // Assert
        int newWidth = await PollUtility.WaitForChangeAsync(getValue: () => window.Features.Size.Width, initialWidth, TimeSpan.FromSeconds(5), ct);
        int newHeight = await PollUtility.WaitForChangeAsync(getValue: () => window.Features.Size.Height, initialHeight, TimeSpan.FromSeconds(5), ct);
        await Assert.That(newWidth).IsGreaterThan(initialWidth);
        await Assert.That(newHeight).IsGreaterThan(initialHeight);
    }

    [Test]
    [SkipOnLinux]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_ExtensionAssignment(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            builder.Features.Size.SetSize(720, 440);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;
        int initialWidth = window.Features.Size.Width;
        int initialHeight = window.Features.Size.Height;

        // Act
        IInfiniFrameWindow returnedWindow = window.Resize(100, 40, ResizeOrigin.BottomRight);

        // Assert
        int newWidth = await PollUtility.WaitForChangeAsync(getValue: () => window.Features.Size.Width, initialWidth, TimeSpan.FromSeconds(5), ct);
        int newHeight = await PollUtility.WaitForChangeAsync(getValue: () => window.Features.Size.Height, initialHeight, TimeSpan.FromSeconds(5), ct);
        await Assert.That(newWidth).IsGreaterThan(initialWidth);
        await Assert.That(newHeight).IsGreaterThan(initialHeight);
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
    }
}
