// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections.Immutable;
using System.Drawing;
using InfiniFrame;
using InfiniFrame.Utilities;

namespace InfiniTests.InfiniFrame.Shared.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class MonitorOverlapCalculatorTests {

    [Test]
    public async Task TryFindBestMonitor_EmptyMonitors_ReturnsFalse(CancellationToken ct = default) {
        // Arrange
        ImmutableArray<InfiniMonitor> monitors = [];
        var bounds = new Rectangle(100, 100, 800, 600);

        // Act
        bool found = MonitorOverlapCalculator.TryFindBestMonitor(monitors, bounds, out int bestIndex);

        // Assert
        await Assert.That(found).IsFalse();
        await Assert.That(bestIndex).IsEqualTo(-1);
    }

    [Test]
    public async Task TryFindBestMonitor_SingleMonitor_FullOverlap_ReturnsTrue(CancellationToken ct = default) {
        // Arrange
        var monitor = new InfiniMonitor(new Rectangle(0, 0, 1920, 1080), new Rectangle(0, 0, 1920, 1040), 1.0);
        ImmutableArray<InfiniMonitor> monitors = [monitor];
        var bounds = new Rectangle(100, 100, 800, 600);

        // Act
        bool found = MonitorOverlapCalculator.TryFindBestMonitor(monitors, bounds, out int bestIndex);

        // Assert
        await Assert.That(found).IsTrue();
        await Assert.That(bestIndex).IsEqualTo(0);
    }

    [Test]
    public async Task TryFindBestMonitor_TwoMonitors_PicksBestOverlap(CancellationToken ct = default) {
        // Arrange
        var monitor1 = new InfiniMonitor(new Rectangle(0, 0, 1920, 1080), new Rectangle(0, 0, 1920, 1040), 1.0);
        var monitor2 = new InfiniMonitor(new Rectangle(1920, 0, 1920, 1080), new Rectangle(1920, 0, 1920, 1040), 1.0);
        ImmutableArray<InfiniMonitor> monitors = [monitor1, monitor2];
        // Window mostly on monitor1
        var bounds = new Rectangle(100, 100, 800, 600);

        // Act
        bool found = MonitorOverlapCalculator.TryFindBestMonitor(monitors, bounds, out int bestIndex);

        // Assert
        await Assert.That(found).IsTrue();
        await Assert.That(bestIndex).IsEqualTo(0);
    }

    [Test]
    public async Task TryFindBestMonitor_NoOverlap_FallsBackToNearest(CancellationToken ct = default) {
        // Arrange
        var monitor1 = new InfiniMonitor(new Rectangle(0, 0, 1920, 1080), new Rectangle(0, 0, 1920, 1040), 1.0);
        var monitor2 = new InfiniMonitor(new Rectangle(2000, 0, 1920, 1080), new Rectangle(2000, 0, 1920, 1040), 1.0);
        ImmutableArray<InfiniMonitor> monitors = [monitor1, monitor2];
        // Window positioned near monitor2, not overlapping either
        var bounds = new Rectangle(3000, 100, 800, 600);

        // Act
        bool found = MonitorOverlapCalculator.TryFindBestMonitor(monitors, bounds, out int bestIndex);

        // Assert
        await Assert.That(found).IsTrue();
        await Assert.That(bestIndex).IsEqualTo(1);
    }

    [Test]
    public async Task TryFindBestMonitor_WindowSpansMultipleMonitors_PicksLargestOverlap(CancellationToken ct = default) {
        // Arrange
        var monitor1 = new InfiniMonitor(new Rectangle(0, 0, 1920, 1080), new Rectangle(0, 0, 1920, 1040), 1.0);
        var monitor2 = new InfiniMonitor(new Rectangle(1920, 0, 1920, 1080), new Rectangle(1920, 0, 1920, 1040), 1.0);
        ImmutableArray<InfiniMonitor> monitors = [monitor1, monitor2];
        // Window centered on the boundary, more overlap on monitor2
        var bounds = new Rectangle(1700, 100, 800, 600);

        // Act
        bool found = MonitorOverlapCalculator.TryFindBestMonitor(monitors, bounds, out int bestIndex);

        // Assert
        await Assert.That(found).IsTrue();
        // 220px overlap on monitor1 vs 580px overlap on monitor2
        await Assert.That(bestIndex).IsEqualTo(1);
    }

    [Test]
    public async Task TryFindBestMonitor_ZeroAreaWindow_FallsBackToNearest(CancellationToken ct = default) {
        // Arrange
        var monitor1 = new InfiniMonitor(new Rectangle(0, 0, 1920, 1080), new Rectangle(0, 0, 1920, 1040), 1.0);
        ImmutableArray<InfiniMonitor> monitors = [monitor1];
        var bounds = new Rectangle(100, 100, 0, 0);

        // Act
        bool found = MonitorOverlapCalculator.TryFindBestMonitor(monitors, bounds, out int bestIndex);

        // Assert
        await Assert.That(found).IsTrue();
        await Assert.That(bestIndex).IsEqualTo(0);
    }
}
