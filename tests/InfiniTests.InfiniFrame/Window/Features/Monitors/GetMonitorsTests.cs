// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.Monitors;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class GetMonitorsTests {
    [Test]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_DirectAssignment(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        IEnumerable<InfiniMonitor> monitors = window.Features.Monitors.GetMonitors();

        // Assert
        await Assert.That(monitors).IsNotEmpty();
    }

    [Test]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_ExtensionAssignment(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        IEnumerable<InfiniMonitor> monitors = window.GetMonitors();

        // Assert
        await Assert.That(monitors).IsNotEmpty();
    }
}
