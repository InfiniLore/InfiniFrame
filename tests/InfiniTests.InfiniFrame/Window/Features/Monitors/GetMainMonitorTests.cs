// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.Monitors;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class GetMainMonitorTests {
    [Test]
    [SkipOnMacOs]
    public async Task AtWindowStage_DirectAssignment(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        InfiniMonitor expected = window.Features.Monitors.GetMonitors().First();

        // Act
        InfiniMonitor mainMonitor = window.Features.Monitors.GetMainMonitor();

        // Assert
        await Assert.That(mainMonitor).IsEqualTo(expected);
    }

    [Test]
    [SkipOnMacOs]
    public async Task AtWindowStage_ExtensionAssignment(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        InfiniMonitor expected = window.GetMonitors().First();

        // Act
        InfiniMonitor mainMonitor = window.GetMainMonitor();

        // Assert
        await Assert.That(mainMonitor).IsEqualTo(expected);
    }
}
