// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Utilities;
using System.Collections.Immutable;
using System.Drawing;

namespace InfiniTests.InfiniFrame.Shared.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class MonitorsUtilityTests {
    [Test]
    public async Task TryGetCurrentMonitor_ReturnsFalse_WhenMonitorsIsDefault(CancellationToken ct = default) {
        // Arrange
        ImmutableArray<InfiniMonitor> monitors = default;
        Rectangle windowBounds = new(10, 10, 100, 100);

        // Act
        bool result = MonitorsUtility.TryGetCurrentMonitor(monitors, windowBounds, out InfiniMonitor monitor);

        // Assert
        await Assert.That(result).IsFalse();
        await Assert.That(monitor).IsEqualTo(default);
    }

    [Test]
    public async Task TryGetCurrentMonitor_ReturnsFalse_WhenMonitorsIsEmpty(CancellationToken ct = default) {
        // Arrange
        ImmutableArray<InfiniMonitor> monitors = [];
        Rectangle windowBounds = new(10, 10, 100, 100);

        // Act
        bool result = MonitorsUtility.TryGetCurrentMonitor(monitors, windowBounds, out InfiniMonitor monitor);

        // Assert
        await Assert.That(result).IsFalse();
        await Assert.That(monitor).IsEqualTo(default);
    }

    [Test]
    public async Task TryGetCurrentMonitor_ReturnsOverlappingMonitor_WhenWindowOverlapsSingleMonitor(CancellationToken ct = default) {
        // Arrange
        InfiniMonitor expected = new(
            MonitorArea: new Rectangle(0, 0, 500, 500),
            WorkArea: new Rectangle(0, 0, 500, 480),
            Scale: 1.0
        );
        ImmutableArray<InfiniMonitor> monitors = [expected];
        Rectangle windowBounds = new(100, 100, 200, 200);

        // Act
        bool result = MonitorsUtility.TryGetCurrentMonitor(monitors, windowBounds, out InfiniMonitor monitor);

        // Assert
        await Assert.That(result).IsTrue();
        await Assert.That(monitor).IsEqualTo(expected);
    }

    [Test]
    public async Task TryGetCurrentMonitor_ReturnsMonitorWithLargestWindowFraction_WhenWindowSpansMultipleMonitors(CancellationToken ct = default) {
        // Arrange
        InfiniMonitor leftMonitor = new(
            MonitorArea: new Rectangle(0, 0, 100, 100),
            WorkArea: new Rectangle(0, 0, 100, 100),
            Scale: 1.0
        );
        InfiniMonitor rightMonitor = new(
            MonitorArea: new Rectangle(100, 0, 100, 100),
            WorkArea: new Rectangle(100, 0, 100, 100),
            Scale: 1.25
        );
        ImmutableArray<InfiniMonitor> monitors = [leftMonitor, rightMonitor];
        Rectangle windowBounds = new(80, 0, 60, 100);

        // Act
        bool result = MonitorsUtility.TryGetCurrentMonitor(monitors, windowBounds, out InfiniMonitor monitor);

        // Assert
        await Assert.That(result).IsTrue();
        await Assert.That(monitor).IsEqualTo(rightMonitor);
    }

    [Test]
    public async Task TryGetCurrentMonitor_ReturnsNearestMonitor_WhenWindowHasNoOverlap(CancellationToken ct = default) {
        // Arrange
        InfiniMonitor leftMonitor = new(
            MonitorArea: new Rectangle(0, 0, 100, 100),
            WorkArea: new Rectangle(0, 0, 100, 100),
            Scale: 1.0
        );
        InfiniMonitor rightMonitor = new(
            MonitorArea: new Rectangle(300, 0, 100, 100),
            WorkArea: new Rectangle(300, 0, 100, 100),
            Scale: 1.0
        );
        ImmutableArray<InfiniMonitor> monitors = [leftMonitor, rightMonitor];
        Rectangle windowBounds = new(140, 0, 100, 100);

        // Act
        bool result = MonitorsUtility.TryGetCurrentMonitor(monitors, windowBounds, out InfiniMonitor monitor);

        // Assert
        await Assert.That(result).IsTrue();
        await Assert.That(monitor).IsEqualTo(leftMonitor);
    }

    [Test]
    public async Task TryGetCurrentMonitor_ReturnsFirstMonitor_WhenDistancesAreEqualAndNoOverlap(CancellationToken ct = default) {
        // Arrange
        InfiniMonitor leftMonitor = new(
            MonitorArea: new Rectangle(0, 0, 100, 100),
            WorkArea: new Rectangle(0, 0, 100, 100),
            Scale: 1.0
        );
        InfiniMonitor rightMonitor = new(
            MonitorArea: new Rectangle(200, 0, 100, 100),
            WorkArea: new Rectangle(200, 0, 100, 100),
            Scale: 1.0
        );
        ImmutableArray<InfiniMonitor> monitors = [leftMonitor, rightMonitor];
        Rectangle windowBounds = new(100, 0, 100, 100);

        // Act
        bool result = MonitorsUtility.TryGetCurrentMonitor(monitors, windowBounds, out InfiniMonitor monitor);

        // Assert
        await Assert.That(result).IsTrue();
        await Assert.That(monitor).IsEqualTo(leftMonitor);
    }

    [Test]
    public async Task TryGetCurrentMonitor_ReturnsNearestMonitor_WhenWindowHasZeroArea(CancellationToken ct = default) {
        // Arrange
        InfiniMonitor topMonitor = new(
            MonitorArea: new Rectangle(0, 0, 100, 100),
            WorkArea: new Rectangle(0, 0, 100, 100),
            Scale: 1.0
        );
        InfiniMonitor bottomMonitor = new(
            MonitorArea: new Rectangle(0, 200, 100, 100),
            WorkArea: new Rectangle(0, 200, 100, 100),
            Scale: 1.0
        );
        ImmutableArray<InfiniMonitor> monitors = [topMonitor, bottomMonitor];
        Rectangle windowBounds = new(10, 180, 0, 0);

        // Act
        bool result = MonitorsUtility.TryGetCurrentMonitor(monitors, windowBounds, out InfiniMonitor monitor);

        // Assert
        await Assert.That(result).IsTrue();
        await Assert.That(monitor).IsEqualTo(bottomMonitor);
    }

    [Test]
    public async Task TryGetCurrentMonitor_ReturnsNearestMonitor_WhenWindowHasNegativeDimensions(CancellationToken ct = default) {
        // Arrange
        InfiniMonitor topMonitor = new(
            MonitorArea: new Rectangle(0, 0, 100, 100),
            WorkArea: new Rectangle(0, 0, 100, 100),
            Scale: 1.0
        );
        InfiniMonitor bottomMonitor = new(
            MonitorArea: new Rectangle(0, 200, 100, 100),
            WorkArea: new Rectangle(0, 200, 100, 100),
            Scale: 1.0
        );
        ImmutableArray<InfiniMonitor> monitors = [topMonitor, bottomMonitor];
        Rectangle windowBounds = new(10, 220, -50, -50);

        // Act
        bool result = MonitorsUtility.TryGetCurrentMonitor(monitors, windowBounds, out InfiniMonitor monitor);

        // Assert
        await Assert.That(result).IsTrue();
        await Assert.That(monitor).IsEqualTo(bottomMonitor);
    }
}
