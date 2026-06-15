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
    [SkipOnMacOs]
    [SkipOnLinux]
    public async Task AtWindowStage_DirectAssignment_IntOverload(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder => {
            builder.Features.Size.SetSize(500, 320);
            builder.Features.Position.SetLocation(100, 100);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Features.Position.MoveWithinCurrentMonitorArea(100_000, 100_000);

        // Assert
        await Assert.That(window.IsClosedOrClosing()).IsFalse();
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnMacOs]
    [SkipOnLinux]
    public async Task AtWindowStage_ExtensionAssignment_PointOverload(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder => {
            builder.Features.Size.SetSize(500, 320);
            builder.Features.Position.SetLocation(120, 120);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;
        Point location = new(100_000, 100_000);

        // Act
        IInfiniFrameWindow returnedWindow = window.MoveWithinCurrentMonitorArea(location);

        // Assert
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
        await Assert.That(window.IsClosedOrClosing()).IsFalse();
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnMacOs]
    [SkipOnLinux]
    public async Task AtWindowStage_ExtensionAssignment_DoubleOverload(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder => {
            builder.Features.Size.SetSize(500, 320);
            builder.Features.Position.SetLocation(140, 140);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        IInfiniFrameWindow returnedWindow = window.MoveWithinCurrentMonitorArea(100_000d, 100_000d);

        // Assert
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
        await Assert.That(window.IsClosedOrClosing()).IsFalse();
    }
}
