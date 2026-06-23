// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.Position;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class CenterOnMonitorTests {
    [Test]
    [SkipOnMacOs]
    [SkipOnLinux]
    public async Task AtWindowStage_DirectAssignment_ValidMonitorIndex(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder => {
            builder.Features.Size.SetSize(700, 420);
            builder.Features.Position.SetLocation(100, 100);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;
        int initialLeft = window.Features.Position.Left;

        // Act
        window.Features.Position.CenterOnMonitor(0);

        // Assert
        int updatedLeft = await PollUtility.WaitForChangeAsync(() => window.Features.Position.Left, initialLeft, TimeSpan.FromSeconds(5), ct);
        await Assert.That(updatedLeft).IsNotEqualTo(initialLeft);
        await Assert.That(window.IsClosedOrClosing()).IsFalse();
    }

    [Test]
    [SkipOnMacOs]
    [SkipOnLinux]
    public async Task AtWindowStage_DirectAssignment_InvalidMonitorIndex_DoesNotThrow(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder => {
            builder.Features.Size.SetSize(700, 420);
            builder.Features.Position.SetLocation(140, 140);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;
        // Act
        window.Features.Position.CenterOnMonitor(-1);

        // Assert
        await Assert.That(window.IsClosedOrClosing()).IsFalse();
    }

    [Test]
    [SkipOnMacOs]
    [SkipOnLinux]
    public async Task AtWindowStage_ExtensionAssignment(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(builder => {
            builder.Features.Size.SetSize(700, 420);
            builder.Features.Position.SetLocation(160, 160);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;
        int initialLeft = window.Features.Position.Left;

        // Act
        IInfiniFrameWindow returnedWindow = window.CenterOnMonitor(0);

        // Assert
        int updatedLeft = await PollUtility.WaitForChangeAsync(() => window.Features.Position.Left, initialLeft, TimeSpan.FromSeconds(5), ct);
        await Assert.That(updatedLeft).IsNotEqualTo(initialLeft);
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
        await Assert.That(window.IsClosedOrClosing()).IsFalse();
    }
}
