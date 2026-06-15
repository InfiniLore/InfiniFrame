// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using System.Drawing;

namespace InfiniTests.InfiniFrame.Window.Features.InfiniFrameWindowFeaturePosition;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class OffsetTests {
    [Test]
    [SkipOnMacOs]
    [SkipOnLinux]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_DirectAssignment_IntOverload(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        window.Features.Position.SetLocation(120, 120);
        int initialLeft = window.Features.Position.Left;
        int initialTop = window.Features.Position.Top;

        // Act
        window.Features.Position.Offset(40, 30);

        // Assert
        await Assert.That(window.Features.Position.Left).IsEqualTo(initialLeft + 40);
        await Assert.That(window.Features.Position.Top).IsEqualTo(initialTop + 30);
    }

    [Test]
    [SkipOnMacOs]
    [SkipOnLinux]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_ExtensionAssignment_PointOverload(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        window.Features.Position.SetLocation(140, 140);
        int initialLeft = window.Features.Position.Left;
        int initialTop = window.Features.Position.Top;
        Point offset = new(25, 35);

        // Act
        IInfiniFrameWindow returnedWindow = window.Offset(offset);

        // Assert
        await Assert.That(window.Features.Position.Left).IsEqualTo(initialLeft + offset.X);
        await Assert.That(window.Features.Position.Top).IsEqualTo(initialTop + offset.Y);
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
    }
}
