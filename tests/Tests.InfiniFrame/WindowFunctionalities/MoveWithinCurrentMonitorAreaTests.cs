// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.InfiniFrame;
using InfiniLore.InfiniFrame.Utilities;
using System.Drawing;
using Monitor=InfiniLore.InfiniFrame.Monitor;

namespace Tests.InfiniFrame.WindowFunctionalities;
using Tests.Shared;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class MoveWithinCurrentMonitorAreaTests {
    
    [Test]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux(SkipUtility.LinuxMovement)]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Arguments(0, 0, 0,0)]
    [Arguments(100, 100, 100,100)]
    [Arguments(-100, -100, 0,0)]
    public async Task Window(int x, int y, int expectedX, int expectedY) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create();
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.MoveWithinCurrentMonitorArea(x,y);

        // Assert
        int offsetX = 0;
        int offsetY = 0;
        window.Invoke(() => {
            MonitorsUtility.TryGetCurrentWindowAndMonitor(window, out _, out Monitor monitor);
            offsetX = monitor.MonitorArea.X;
            offsetY = monitor.MonitorArea.Y;
        });
        
        Point location = window.Location;
        await Assert.That(location.X).IsEqualTo(offsetX + expectedX);
        await Assert.That(location.Y).IsEqualTo(offsetY + expectedY);
    }
    
    [Test]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux(SkipUtility.LinuxMovement)]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Arguments(0, 0, 0,0)]
    [Arguments(100, 100, 100,100)]
    [Arguments(-100, -100, 0,0)]
    public async Task Window_AsPoint(int x, int y, int expectedX, int expectedY) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create();
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.MoveWithinCurrentMonitorArea(new Point(x,y));

        // Assert
        int offsetX = 0;
        int offsetY = 0;
        window.Invoke(() => {
            MonitorsUtility.TryGetCurrentWindowAndMonitor(window, out _, out Monitor monitor);
            offsetX = monitor.MonitorArea.X;
            offsetY = monitor.MonitorArea.Y;
        });
        
        Point location = window.Location;
        await Assert.That(location.X).IsEqualTo(offsetX + expectedX);
        await Assert.That(location.Y).IsEqualTo(offsetY + expectedY);
    }
    
    [Test]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux(SkipUtility.LinuxMovement)]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Arguments(0, 0, 0,0)]
    [Arguments(100, 100, 100,100)]
    [Arguments(-100, -100, 0,0)]
    public async Task Window_AsDouble(double x, double y, int expectedX, int expectedY) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create();
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.MoveWithinCurrentMonitorArea(x,y);

        // Assert
        int offsetX = 0;
        int offsetY = 0;
        window.Invoke(() => {
            MonitorsUtility.TryGetCurrentWindowAndMonitor(window, out _, out Monitor monitor);
            offsetX = monitor.MonitorArea.X;
            offsetY = monitor.MonitorArea.Y;
        });
        
        Point location = window.Location;
        await Assert.That(location.X).IsEqualTo(offsetX + expectedX);
        await Assert.That(location.Y).IsEqualTo(offsetY + expectedY);
    }
}
