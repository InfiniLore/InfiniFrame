// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using System.Drawing;

namespace InfiniTests.InfiniFrame.Window.Features.Position;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class MoveWithinCurrentMonitorAreaTests {
    [Test]
    [NotInParallelInfiniTests]
    [SkipOnLinux]
    public async Task AtWindowStage_DirectAssignment_IntOverload(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            builder.Features.Size.SetSize(500, 320);
            builder.Features.Position.SetLocation(100, 100);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;
        int initialLeft = window.Features.Position.Left;

        // Act
        window.Features.Position.MoveWithinCurrentMonitorArea(100_000, 100_000);

        // Assert
        int updatedLeft = await PollUtility.WaitForChangeAsync(getValue: () => window.Features.Position.Left, initialLeft, TimeSpan.FromSeconds(5), ct);
        await Assert.That(updatedLeft).IsNotEqualTo(initialLeft);
        await Assert.That(window.IsClosedOrClosing()).IsFalse();
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnLinux]
    public async Task AtWindowStage_ExtensionAssignment_PointOverload(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            builder.Features.Size.SetSize(500, 320);
            builder.Features.Position.SetLocation(120, 120);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;
        Point location = new(100_000, 100_000);
        int initialLeft = window.Features.Position.Left;

        // Act
        IInfiniFrameWindow returnedWindow = window.MoveWithinCurrentMonitorArea(location);

        // Assert
        int updatedLeft = await PollUtility.WaitForChangeAsync(getValue: () => window.Features.Position.Left, initialLeft, TimeSpan.FromSeconds(5), ct);
        await Assert.That(updatedLeft).IsNotEqualTo(initialLeft);
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
        await Assert.That(window.IsClosedOrClosing()).IsFalse();
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnLinux]
    public async Task AtWindowStage_ExtensionAssignment_DoubleOverload(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            builder.Features.Size.SetSize(500, 320);
            builder.Features.Position.SetLocation(140, 140);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;
        int initialLeft = window.Features.Position.Left;

        // Act
        IInfiniFrameWindow returnedWindow = window.MoveWithinCurrentMonitorArea(100_000d, 100_000d);

        // Assert
        int updatedLeft = await PollUtility.WaitForChangeAsync(getValue: () => window.Features.Position.Left, initialLeft, TimeSpan.FromSeconds(5), ct);
        await Assert.That(updatedLeft).IsNotEqualTo(initialLeft);
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
        await Assert.That(window.IsClosedOrClosing()).IsFalse();
    }
}