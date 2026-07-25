// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.Position;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class CenterTests {
    [Test]
    [NotInParallelInfiniTests]
    [SkipOnLinux]
    public async Task AtWindowStage_DirectAssignment(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            builder.Features.Size.SetSize(700, 420);
            builder.Features.Position.SetLocation(100, 100);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;
        int initialLeft = window.Features.Position.Left;

        // Act
        window.Features.Position.Center();

        // Assert
        int updatedLeft = await PollUtility.WaitForChangeAsync(getValue: () => window.Features.Position.Left, initialLeft, TimeSpan.FromSeconds(5), ct);
        await Assert.That(updatedLeft).IsNotEqualTo(initialLeft);
        await Assert.That(window.IsClosedOrClosing()).IsFalse();
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnLinux]
    public async Task AtWindowStage_ExtensionAssignment(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            builder.Features.Size.SetSize(700, 420);
            builder.Features.Position.SetLocation(120, 120);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;
        int initialLeft = window.Features.Position.Left;

        // Act
        IInfiniFrameWindow returnedWindow = window.Center();

        // Assert
        int updatedLeft = await PollUtility.WaitForChangeAsync(getValue: () => window.Features.Position.Left, initialLeft, TimeSpan.FromSeconds(5), ct);
        await Assert.That(updatedLeft).IsNotEqualTo(initialLeft);
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
        await Assert.That(window.IsClosedOrClosing()).IsFalse();
    }
}
