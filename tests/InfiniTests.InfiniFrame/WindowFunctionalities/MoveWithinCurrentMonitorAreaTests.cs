// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Utilities;
using System.Drawing;

namespace InfiniTests.InfiniFrame.WindowFunctionalities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class MoveWithinCurrentMonitorAreaTests {

    [Test]
    [DisplayName($"{nameof(MoveWithinCurrentMonitorAreaTests)}.{nameof(Window)}")]
    [SkipOnMacOs]
    [SkipOnLinux(SkipUtility.LinuxMovement)]
    [NotInParallelInfiniTests]
    [Arguments(0, 0, 0, 0)]
    [Arguments(100, 100, 100, 100)]
    [Arguments(-100, -100, 0, 0)]
    public async Task Window(int x, int y, int expectedX, int expectedY, CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.MoveWithinCurrentMonitorArea(x, y);

        // Assert
        int offsetX = 0;
        int offsetY = 0;
        window.Invoke(() => {
            MonitorsUtility.TryGetCurrentWindowAndMonitor(window, out _, out InfiniMonitor monitor);
            offsetX = monitor.MonitorArea.X;
            offsetY = monitor.MonitorArea.Y;
        });

        Point location = window.Location;
        await Assert.That(location.X).IsEqualTo(offsetX + expectedX);
        await Assert.That(location.Y).IsEqualTo(offsetY + expectedY);
    }

    [Test]
    [DisplayName($"{nameof(MoveWithinCurrentMonitorAreaTests)}.{nameof(Window_AsPoint)}")]
    [SkipOnMacOs]
    [SkipOnLinux(SkipUtility.LinuxMovement)]
    [NotInParallelInfiniTests]
    [Arguments(0, 0, 0, 0)]
    [Arguments(100, 100, 100, 100)]
    [Arguments(-100, -100, 0, 0)]
    public async Task Window_AsPoint(int x, int y, int expectedX, int expectedY, CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.MoveWithinCurrentMonitorArea(new Point(x, y));

        // Assert
        int offsetX = 0;
        int offsetY = 0;
        window.Invoke(() => {
            MonitorsUtility.TryGetCurrentWindowAndMonitor(window, out _, out InfiniMonitor monitor);
            offsetX = monitor.MonitorArea.X;
            offsetY = monitor.MonitorArea.Y;
        });

        Point location = window.Location;
        await Assert.That(location.X).IsEqualTo(offsetX + expectedX);
        await Assert.That(location.Y).IsEqualTo(offsetY + expectedY);
    }

    [Test]
    [DisplayName($"{nameof(MoveWithinCurrentMonitorAreaTests)}.{nameof(Window_AsDouble)}")]
    [SkipOnMacOs]
    [SkipOnLinux(SkipUtility.LinuxMovement)]
    [NotInParallelInfiniTests]
    [Arguments(0, 0, 0, 0)]
    [Arguments(100, 100, 100, 100)]
    [Arguments(-100, -100, 0, 0)]
    public async Task Window_AsDouble(double x, double y, int expectedX, int expectedY, CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.MoveWithinCurrentMonitorArea(x, y);

        // Assert
        int offsetX = 0;
        int offsetY = 0;
        window.Invoke(() => {
            MonitorsUtility.TryGetCurrentWindowAndMonitor(window, out _, out InfiniMonitor monitor);
            offsetX = monitor.MonitorArea.X;
            offsetY = monitor.MonitorArea.Y;
        });

        Point location = window.Location;
        await Assert.That(location.X).IsEqualTo(offsetX + expectedX);
        await Assert.That(location.Y).IsEqualTo(offsetY + expectedY);
    }
}
