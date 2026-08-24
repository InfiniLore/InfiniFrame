// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge;

namespace InfiniTests.InfiniFrame.Shared.Records;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniMonitorTests {

    [Test]
    public async Task Constructor_WithRectangles_SetsProperties(CancellationToken ct = default) {
        // Arrange
        var monitorArea = new System.Drawing.Rectangle(0, 0, 1920, 1080);
        var workArea = new System.Drawing.Rectangle(0, 0, 1920, 1040);

        // Act
        var monitor = new InfiniMonitor(monitorArea, workArea, 1.5);

        // Assert
        await Assert.That(monitor.MonitorArea).IsEqualTo(monitorArea);
        await Assert.That(monitor.WorkArea).IsEqualTo(workArea);
        await Assert.That(monitor.Scale).IsEqualTo(1.5);
    }

    [Test]
    public async Task Constructor_WithNativeRects_ConvertsCorrectly(CancellationToken ct = default) {
        // Arrange
        var monitorRect = new NativeRect { X = 10, Y = 20, Width = 1920, Height = 1080 };
        var workRect = new NativeRect { X = 10, Y = 20, Width = 1920, Height = 1040 };

        // Act
        var monitor = new InfiniMonitor(monitorRect, workRect, 2.0);

        // Assert
        await Assert.That(monitor.MonitorArea.X).IsEqualTo(10);
        await Assert.That(monitor.MonitorArea.Y).IsEqualTo(20);
        await Assert.That(monitor.MonitorArea.Width).IsEqualTo(1920);
        await Assert.That(monitor.MonitorArea.Height).IsEqualTo(1080);
        await Assert.That(monitor.WorkArea.Width).IsEqualTo(1920);
        await Assert.That(monitor.WorkArea.Height).IsEqualTo(1040);
        await Assert.That(monitor.Scale).IsEqualTo(2.0);
    }

    [Test]
    public async Task Equality_SameValues_ReturnsTrue(CancellationToken ct = default) {
        // Arrange
        var area = new System.Drawing.Rectangle(0, 0, 1920, 1080);
        var work = new System.Drawing.Rectangle(0, 0, 1920, 1040);
        var m1 = new InfiniMonitor(area, work, 1.0);
        var m2 = new InfiniMonitor(area, work, 1.0);

        // Act & Assert
        await Assert.That(m1).IsEqualTo(m2);
    }

    [Test]
    public async Task Equality_DifferentScale_ReturnsFalse(CancellationToken ct = default) {
        // Arrange
        var area = new System.Drawing.Rectangle(0, 0, 1920, 1080);
        var work = new System.Drawing.Rectangle(0, 0, 1920, 1040);
        var m1 = new InfiniMonitor(area, work, 1.0);
        var m2 = new InfiniMonitor(area, work, 2.0);

        // Act & Assert
        await Assert.That(m1).IsNotEqualTo(m2);
    }

    [Test]
    public async Task Scale_One_IsDefault(CancellationToken ct = default) {
        // Arrange
        var area = new System.Drawing.Rectangle(0, 0, 1920, 1080);

        // Act
        var monitor = new InfiniMonitor(area, area, 1.0);

        // Assert
        await Assert.That(monitor.Scale).IsEqualTo(1.0);
    }
}
