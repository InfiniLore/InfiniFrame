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
    [SkipOnMacOs]
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
    [SkipOnMacOs]
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
}
