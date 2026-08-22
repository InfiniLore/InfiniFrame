// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;
using InfiniFrame.Utilities;

namespace InfiniTests.InfiniFrame.Shared.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class PositionCalculationsTests {

    // -----------------------------------------------------------------------------------------------------------------
    // ComputeCenter
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task ComputeCenter_CenterInMiddleOfMonitor(CancellationToken ct = default) {
        var monitorArea = new Rectangle(0, 0, 1920, 1080);

        Point result = PositionCalculations.ComputeCenter(monitorArea, 800, 600);

        await Assert.That(result.X).IsEqualTo(560);
        await Assert.That(result.Y).IsEqualTo(240);
    }

    [Test]
    public async Task ComputeCenter_SmallWindowInLargeMonitor(CancellationToken ct = default) {
        var monitorArea = new Rectangle(0, 0, 3840, 2160);

        Point result = PositionCalculations.ComputeCenter(monitorArea, 100, 100);

        await Assert.That(result.X).IsEqualTo(1870);
        await Assert.That(result.Y).IsEqualTo(1030);
    }

    [Test]
    public async Task ComputeCenter_WindowSameSizeAsMonitor(CancellationToken ct = default) {
        var monitorArea = new Rectangle(0, 0, 1920, 1080);

        Point result = PositionCalculations.ComputeCenter(monitorArea, 1920, 1080);

        await Assert.That(result.X).IsEqualTo(0);
        await Assert.That(result.Y).IsEqualTo(0);
    }

    [Test]
    public async Task ComputeCenter_NonZeroOriginMonitor(CancellationToken ct = default) {
        var monitorArea = new Rectangle(100, 200, 1920, 1080);

        Point result = PositionCalculations.ComputeCenter(monitorArea, 800, 600);

        await Assert.That(result.X).IsEqualTo(660);
        await Assert.That(result.Y).IsEqualTo(440);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // ClampToMonitorArea
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task ClampToMonitorArea_WindowFullyInside_NoChange(CancellationToken ct = default) {
        var workArea = new Rectangle(0, 0, 1920, 1040);

        (int left, int top) = PositionCalculations.ClampToMonitorArea(100, 100, 800, 600, workArea);

        await Assert.That(left).IsEqualTo(100);
        await Assert.That(top).IsEqualTo(100);
    }

    [Test]
    public async Task ClampToMonitorArea_WindowExceedsRightEdge_ClampsLeft(CancellationToken ct = default) {
        var workArea = new Rectangle(0, 0, 1920, 1040);

        (int left, int top) = PositionCalculations.ClampToMonitorArea(1500, 100, 800, 600, workArea);

        await Assert.That(left).IsEqualTo(1120);
        await Assert.That(top).IsEqualTo(100);
    }

    [Test]
    public async Task ClampToMonitorArea_WindowExceedsBottomEdge_ClampsTop(CancellationToken ct = default) {
        var workArea = new Rectangle(0, 0, 1920, 1040);

        (int left, int top) = PositionCalculations.ClampToMonitorArea(100, 800, 800, 600, workArea);

        await Assert.That(left).IsEqualTo(100);
        await Assert.That(top).IsEqualTo(440);
    }

    [Test]
    public async Task ClampToMonitorArea_WindowExceedsLeftEdge_ClampsToLeftBound(CancellationToken ct = default) {
        var workArea = new Rectangle(100, 0, 1920, 1040);

        (int left, int top) = PositionCalculations.ClampToMonitorArea(-500, 100, 800, 600, workArea);

        await Assert.That(left).IsEqualTo(100);
        await Assert.That(top).IsEqualTo(100);
    }

    [Test]
    public async Task ClampToMonitorArea_WindowExceedsTopEdge_ClampsToTopBound(CancellationToken ct = default) {
        var workArea = new Rectangle(0, 100, 1920, 1040);

        (int left, int top) = PositionCalculations.ClampToMonitorArea(100, -500, 800, 600, workArea);

        await Assert.That(left).IsEqualTo(100);
        await Assert.That(top).IsEqualTo(100);
    }

    [Test]
    public async Task ClampToMonitorArea_WindowLargerThanWorkArea_ClampsToOrigin(CancellationToken ct = default) {
        var workArea = new Rectangle(0, 0, 800, 600);

        (int left, int top) = PositionCalculations.ClampToMonitorArea(0, 0, 1920, 1080, workArea);

        await Assert.That(left).IsEqualTo(0);
        await Assert.That(top).IsEqualTo(0);
    }
}
