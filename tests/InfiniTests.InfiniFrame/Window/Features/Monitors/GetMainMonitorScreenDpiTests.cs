// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.Monitors;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class GetMainMonitorScreenDpiTests {
    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_DirectAssignment(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        int dpi = window.Features.Monitors.GetMainMonitorScreenDpi();

        // Assert
        await Assert.That(dpi).IsGreaterThan(0);
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_ExtensionAssignment(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        int dpi = window.GetMainMonitorScreenDpi();

        // Assert
        await Assert.That(dpi).IsGreaterThan(0);
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_DpiIsConsistentAcrossCalls(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        int dpi1 = window.Features.Monitors.GetMainMonitorScreenDpi();
        int dpi2 = window.Features.Monitors.GetMainMonitorScreenDpi();
        int dpi3 = window.Features.Monitors.GetMainMonitorScreenDpi();

        // Assert
        await Assert.That(dpi1).IsEqualTo(dpi2);
        await Assert.That(dpi2).IsEqualTo(dpi3);
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_DpiIsAtLeastStandardMinimum(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        int dpi = window.Features.Monitors.GetMainMonitorScreenDpi();

        // Assert
        await Assert.That(dpi).IsGreaterThanOrEqualTo(96);
    }
}