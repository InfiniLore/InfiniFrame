// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.Position;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Regression tests for native centering with monitor offsets (audit finding 1):
///     <c>Center()</c> (and the start-centered path) must center within the target
///     monitor's geometry including monitors placed away from the origin - multi-monitor
///     layouts must not center against (0, 0). Runs where window positioning is
///     enforceable and observable (X11 window managers); Wayland is soft-skipped because
///     the compositor owns placement there.
/// </summary>
public class CenterMonitorOffsetTests {
    private const int Tolerance = 32;

    private static bool ShouldSkip() {
        if (!OperatingSystem.IsLinux()) return true; // Linux migration regression tests
        // Wayland compositors own window placement; absolute positions are unenforceable.
        return Environment.GetEnvironmentVariable("WAYLAND_DISPLAY") != null;
    }

    private static async Task<Point> WaitForPositionAsync(
        Func<Point> getPosition,
        Func<Point, bool> accept,
        TimeSpan timeout,
        CancellationToken ct
        ) {
        DateTime deadline = DateTime.UtcNow + timeout;
        Point current = getPosition();
        while (DateTime.UtcNow < deadline) {
            if (accept(current)) return current;
            await Task.Delay(50, ct);
            current = getPosition();
        }
        return current;
    }

    [Test]
    [NotInParallelInfiniTests]
    [DefaultTimeout(60_000)]
    public async Task Center_OnMonitorWithOffsetOrigin_CentersWithinThatMonitor(CancellationToken ct = default) {
        // Arrange
        if (ShouldSkip()) return;
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            builder.Features.Size.SetSize(700, 420);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Pick a monitor placed away from the origin when the environment provides one
        // (multi-monitor Xvfb via `xrandr --setmonitor`); otherwise fall back to the main
        // monitor, which still validates the offset math at origin (0, 0).
        InfiniMonitor[] monitors = window.Features.Monitors.GetMonitors().ToArray();
        InfiniMonitor target = monitors.FirstOrDefault(
            m => m.MonitorArea.X != 0 || m.MonitorArea.Y != 0, window.Features.Monitors.GetMainMonitor());
        Rectangle area = target.MonitorArea;

        // Move the window onto the target monitor first so native centering resolves
        // this monitor as the window's current monitor.
        window.Features.Position.SetLocation(area.X + 8, area.Y + 8);
        Point placed = await WaitForPositionAsync(
            () => window.Features.Position.Location,
            p => Math.Abs(p.X - (area.X + 8)) <= Tolerance && Math.Abs(p.Y - (area.Y + 8)) <= Tolerance,
            TimeSpan.FromSeconds(10),
            ct);

        // Act
        window.Features.Position.Center();

        // Assert: centered within the target monitor geometry including its origin.
        int width = window.Features.Size.Width;
        int height = window.Features.Size.Height;
        int expectedLeft = area.X + (area.Width - width) / 2;
        int expectedTop = area.Y + (area.Height - height) / 2;
        Point final = await WaitForPositionAsync(
            () => window.Features.Position.Location,
            p => Math.Abs(p.X - expectedLeft) <= Tolerance && Math.Abs(p.Y - expectedTop) <= Tolerance,
            TimeSpan.FromSeconds(10),
            ct);

        await Assert.That(Math.Abs(final.X - expectedLeft)).IsLessThanOrEqualTo(Tolerance);
        await Assert.That(Math.Abs(final.Y - expectedTop)).IsLessThanOrEqualTo(Tolerance);
        await Assert.That(window.IsClosedOrClosing()).IsFalse();
    }

    [Test]
    [NotInParallelInfiniTests]
    [DefaultTimeout(60_000)]
    public async Task StartCentered_CentersWithinMainMonitorGeometry_IncludingOrigin(CancellationToken ct = default) {
        // Arrange + Act: the start-centered path must use the same offset-inclusive math.
        if (ShouldSkip()) return;
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            builder.Features.Size.SetSize(700, 420);
            builder.Features.Position.CenteredOnMainMonitor(true);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        Rectangle area = window.Features.Monitors.GetMainMonitor().MonitorArea;
        int width = window.Features.Size.Width;
        int height = window.Features.Size.Height;
        int expectedLeft = area.X + (area.Width - width) / 2;
        int expectedTop = area.Y + (area.Height - height) / 2;
        Point final = await WaitForPositionAsync(
            () => window.Features.Position.Location,
            p => Math.Abs(p.X - expectedLeft) <= Tolerance && Math.Abs(p.Y - expectedTop) <= Tolerance,
            TimeSpan.FromSeconds(10),
            ct);

        await Assert.That(Math.Abs(final.X - expectedLeft)).IsLessThanOrEqualTo(Tolerance);
        await Assert.That(Math.Abs(final.Y - expectedTop)).IsLessThanOrEqualTo(Tolerance);
        await Assert.That(window.IsClosedOrClosing()).IsFalse();
    }

    [Test]
    [NotInParallelInfiniTests]
    [DefaultTimeout(60_000)]
    public async Task CenterAfterMovingBetweenMonitors_RecentersOnNewMonitor(CancellationToken ct = default) {
        // Arrange: center on one monitor, then re-center after moving to an offset
        // monitor - the second centering must use the new monitor's origin (audit
        // finding 1's exact regression vector).
        if (ShouldSkip()) return;
        using var windowUtility = InfiniFrameTestWindow.Create(builder: builder => {
            builder.Features.Size.SetSize(700, 420);
        }, ct);
        IInfiniFrameWindow window = windowUtility.Window;

        InfiniMonitor[] monitors = window.Features.Monitors.GetMonitors().ToArray();
        InfiniMonitor offsetMonitor = monitors.FirstOrDefault(
            m => m.MonitorArea.X != 0 || m.MonitorArea.Y != 0, window.Features.Monitors.GetMainMonitor());
        InfiniMonitor originMonitor = monitors.FirstOrDefault(
            m => m.MonitorArea.X == 0 && m.MonitorArea.Y == 0, window.Features.Monitors.GetMainMonitor());
        if (offsetMonitor.MonitorArea == originMonitor.MonitorArea) return; // no offset monitor available

        // Act: first center on the origin monitor, then move to and center on the
        // offset monitor.
        window.Features.Position.SetLocation(originMonitor.MonitorArea.X + 8, originMonitor.MonitorArea.Y + 8);
        window.Features.Position.Center();
        Rectangle area = offsetMonitor.MonitorArea;
        window.Features.Position.SetLocation(area.X + 8, area.Y + 8);
        await WaitForPositionAsync(
            () => window.Features.Position.Location,
            p => Math.Abs(p.X - (area.X + 8)) <= Tolerance,
            TimeSpan.FromSeconds(10),
            ct);
        window.Features.Position.Center();

        // Assert
        int width = window.Features.Size.Width;
        int height = window.Features.Size.Height;
        int expectedLeft = area.X + (area.Width - width) / 2;
        int expectedTop = area.Y + (area.Height - height) / 2;
        Point final = await WaitForPositionAsync(
            () => window.Features.Position.Location,
            p => Math.Abs(p.X - expectedLeft) <= Tolerance && Math.Abs(p.Y - expectedTop) <= Tolerance,
            TimeSpan.FromSeconds(10),
            ct);

        await Assert.That(Math.Abs(final.X - expectedLeft)).IsLessThanOrEqualTo(Tolerance);
        await Assert.That(Math.Abs(final.Y - expectedTop)).IsLessThanOrEqualTo(Tolerance);
        await Assert.That(window.IsClosedOrClosing()).IsFalse();
    }
}
