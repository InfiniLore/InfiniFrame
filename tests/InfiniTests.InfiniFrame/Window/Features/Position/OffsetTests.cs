// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using System.Drawing;

namespace InfiniTests.InfiniFrame.Window.Features.Position;
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
        int originalLeft = window.Features.Position.Left;
        int originalTop = window.Features.Position.Top;
        window.Features.Position.SetLocation(120, 120);
        int initialLeft = await PollUtility.WaitForChangeAsync(() => window.Features.Position.Left, originalLeft, TimeSpan.FromSeconds(5), ct);
        int initialTop = await PollUtility.WaitForChangeAsync(() => window.Features.Position.Top, originalTop, TimeSpan.FromSeconds(5), ct);

        // Act
        window.Features.Position.Offset(40, 30);

        // Assert
        int newLeft = await PollUtility.WaitForChangeAsync(() => window.Features.Position.Left, initialLeft, TimeSpan.FromSeconds(5), ct);
        int newTop = await PollUtility.WaitForChangeAsync(() => window.Features.Position.Top, initialTop, TimeSpan.FromSeconds(5), ct);
        await Assert.That(newLeft).IsEqualTo(initialLeft + 40);
        await Assert.That(newTop).IsEqualTo(initialTop + 30);
    }

    [Test]
    [SkipOnMacOs]
    [SkipOnLinux]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_ExtensionAssignment_PointOverload(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        int originalLeft = window.Features.Position.Left;
        int originalTop = window.Features.Position.Top;
        window.Features.Position.SetLocation(140, 140);
        int initialLeft = await PollUtility.WaitForChangeAsync(() => window.Features.Position.Left, originalLeft, TimeSpan.FromSeconds(5), ct);
        int initialTop = await PollUtility.WaitForChangeAsync(() => window.Features.Position.Top, originalTop, TimeSpan.FromSeconds(5), ct);
        Point offset = new(25, 35);

        // Act
        IInfiniFrameWindow returnedWindow = window.Offset(offset);

        // Assert
        int newLeft = await PollUtility.WaitForChangeAsync(() => window.Features.Position.Left, initialLeft, TimeSpan.FromSeconds(5), ct);
        int newTop = await PollUtility.WaitForChangeAsync(() => window.Features.Position.Top, initialTop, TimeSpan.FromSeconds(5), ct);
        await Assert.That(newLeft).IsEqualTo(initialLeft + offset.X);
        await Assert.That(newTop).IsEqualTo(initialTop + offset.Y);
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
    }

    [Test]
    [SkipOnMacOs]
    [SkipOnLinux]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_ExtensionAssignment_DoubleOverload(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        int originalLeft = window.Features.Position.Left;
        int originalTop = window.Features.Position.Top;
        window.Features.Position.SetLocation(160, 160);
        int initialLeft = await PollUtility.WaitForChangeAsync(() => window.Features.Position.Left, originalLeft, TimeSpan.FromSeconds(5), ct);
        int initialTop = await PollUtility.WaitForChangeAsync(() => window.Features.Position.Top, originalTop, TimeSpan.FromSeconds(5), ct);
        const double leftOffset = 21.9;
        const double topOffset = 33.4;

        // Act
        IInfiniFrameWindow returnedWindow = window.Offset(leftOffset, topOffset);

        // Assert
        int newLeft = await PollUtility.WaitForChangeAsync(() => window.Features.Position.Left, initialLeft, TimeSpan.FromSeconds(5), ct);
        int newTop = await PollUtility.WaitForChangeAsync(() => window.Features.Position.Top, initialTop, TimeSpan.FromSeconds(5), ct);
        await Assert.That(newLeft).IsEqualTo(initialLeft + (int)leftOffset);
        await Assert.That(newTop).IsEqualTo(initialTop + (int)topOffset);
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
    }
}
