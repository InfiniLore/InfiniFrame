// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;
using InfiniFrame.Utilities;

namespace InfiniTests.InfiniFrame.Features.Position;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class PositionCalculationsTests {

    // -----------------------------------------------------------------------------------------------------------------
    // ComputeCenter
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task ComputeCenter_WindowSmallerThanMonitor_CentersCorrectly(CancellationToken ct = default) {
        // Arrange
        var monitorArea = new Rectangle(0, 0, 1920, 1080);

        // Act
        Point result = PositionCalculations.ComputeCenter(monitorArea, 800, 600);

        // Assert
        await Assert.That(result.X).IsEqualTo(560);
        await Assert.That(result.Y).IsEqualTo(240);
    }

    [Test]
    public async Task ComputeCenter_WindowSameSizeAsMonitor_ReturnsTopLeft(CancellationToken ct = default) {
        // Arrange
        var monitorArea = new Rectangle(0, 0, 1920, 1080);

        // Act
        Point result = PositionCalculations.ComputeCenter(monitorArea, 1920, 1080);

        // Assert
        await Assert.That(result.X).IsEqualTo(0);
        await Assert.That(result.Y).IsEqualTo(0);
    }

    [Test]
    public async Task ComputeCenter_WindowLargerThanMonitor_ReturnsNegative(CancellationToken ct = default) {
        // Arrange
        var monitorArea = new Rectangle(0, 0, 1920, 1080);

        // Act
        Point result = PositionCalculations.ComputeCenter(monitorArea, 2500, 1500);

        // Assert
        await Assert.That(result.X).IsEqualTo(-290);
        await Assert.That(result.Y).IsEqualTo(-210);
    }

    [Test]
    public async Task ComputeCenter_MonitorAtOffset_CentersWithinOffset(CancellationToken ct = default) {
        // Arrange, second monitor at 1920,0
        var monitorArea = new Rectangle(1920, 0, 1920, 1080);

        // Act
        Point result = PositionCalculations.ComputeCenter(monitorArea, 800, 600);

        // Assert
        await Assert.That(result.X).IsEqualTo(2480);
        await Assert.That(result.Y).IsEqualTo(240);
    }

    [Test]
    public async Task ComputeCenter_WindowSizeOnePixel_CentersCorrectly(CancellationToken ct = default) {
        // Arrange
        var monitorArea = new Rectangle(0, 0, 1920, 1080);

        // Act
        Point result = PositionCalculations.ComputeCenter(monitorArea, 1, 1);

        // Assert
        await Assert.That(result.X).IsEqualTo(960);
        await Assert.That(result.Y).IsEqualTo(540);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // ClampToMonitorArea
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task ClampToMonitorArea_WithinBounds_NoChange(CancellationToken ct = default) {
        // Arrange
        var workArea = new Rectangle(0, 0, 1920, 1080);

        // Act
        (int left, int top) = PositionCalculations.ClampToMonitorArea(100, 100, 800, 600, workArea);

        // Assert
        await Assert.That(left).IsEqualTo(100);
        await Assert.That(top).IsEqualTo(100);
    }

    [Test]
    public async Task ClampToMonitorArea_ExceedsRightBound_ClampsToLeft(CancellationToken ct = default) {
        // Arrange, window right edge at 100+2000=2100 > 1920
        var workArea = new Rectangle(0, 0, 1920, 1080);

        // Act
        (int left, int top) = PositionCalculations.ClampToMonitorArea(100, 100, 2000, 600, workArea);

        // Assert, clamped so right edge = 1920 => left = 1920 - 2000 = -80, but >= 0 so left = 0
        await Assert.That(left).IsEqualTo(0);
    }

    [Test]
    public async Task ClampToMonitorArea_ExceedsBottomBound_ClampsToTop(CancellationToken ct = default) {
        // Arrange, window bottom edge at 100+1200=1300 > 1080
        var workArea = new Rectangle(0, 0, 1920, 1080);

        // Act
        (int left, int top) = PositionCalculations.ClampToMonitorArea(100, 100, 800, 1200, workArea);

        // Assert, clamped so bottom edge = 1080 => top = 1080 - 1200 = -120, but >= 0 so top = 0
        await Assert.That(top).IsEqualTo(0);
    }

    [Test]
    public async Task ClampToMonitorArea_NegativePosition_ClampsToPositive(CancellationToken ct = default) {
        // Arrange
        var workArea = new Rectangle(0, 0, 1920, 1080);

        // Act
        (int left, int top) = PositionCalculations.ClampToMonitorArea(-500, -300, 800, 600, workArea);

        // Assert
        await Assert.That(left).IsEqualTo(0);
        await Assert.That(top).IsEqualTo(0);
    }

    [Test]
    public async Task ClampToMonitorArea_WindowLargerThanWorkArea_ClampsToTopLeft(CancellationToken ct = default) {
        // Arrange, window 2500x1500 > workArea 1920x1080
        var workArea = new Rectangle(0, 0, 1920, 1080);

        // Act
        (int left, int top) = PositionCalculations.ClampToMonitorArea(0, 0, 2500, 1500, workArea);

        // Assert
        await Assert.That(left).IsEqualTo(0);
        await Assert.That(top).IsEqualTo(0);
    }

    [Test]
    public async Task ClampToMonitorArea_MonitorWithOffset_RespectsOffset(CancellationToken ct = default) {
        // Arrange, second monitor at 1920,0 with 1920x1080 work area
        var workArea = new Rectangle(1920, 0, 1920, 1080);

        // Act
        (int left, int top) = PositionCalculations.ClampToMonitorArea(2000, 100, 800, 600, workArea);

        // Assert, within bounds
        await Assert.That(left).IsEqualTo(2000);
        await Assert.That(top).IsEqualTo(100);
    }

    [Test]
    public async Task ClampToMonitorArea_AtExactRightBound_NoChange(CancellationToken ct = default) {
        // Arrange, right edge = 1120 + 800 = 1920 (exactly at bound)
        var workArea = new Rectangle(0, 0, 1920, 1080);

        // Act
        (int left, int top) = PositionCalculations.ClampToMonitorArea(1120, 100, 800, 600, workArea);

        // Assert
        await Assert.That(left).IsEqualTo(1120);
    }

    [Test]
    public async Task ClampToMonitorArea_AtExactBottomBound_NoChange(CancellationToken ct = default) {
        // Arrange, bottom edge = 480 + 600 = 1080 (exactly at bound)
        var workArea = new Rectangle(0, 0, 1920, 1080);

        // Act
        (int left, int top) = PositionCalculations.ClampToMonitorArea(100, 480, 800, 600, workArea);

        // Assert
        await Assert.That(top).IsEqualTo(480);
    }

    [Test]
    public async Task ClampToMonitorArea_ZeroWindowSize_PositionClampsToBounds(CancellationToken ct = default) {
        // Arrange, position 5000 exceeds right bound 1920, but window width is 0
        // rightBound - windowWidth = 1920 - 0 = 1920, Math.Max(1920, 0) = 1920
        var workArea = new Rectangle(0, 0, 1920, 1080);

        // Act
        (int left, int top) = PositionCalculations.ClampToMonitorArea(5000, 5000, 0, 0, workArea);

        // Assert
        await Assert.That(left).IsEqualTo(1920);
        await Assert.That(top).IsEqualTo(1080);
    }
}
