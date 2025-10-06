// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.Photino.NET;
using InfiniLore.Photino.Utilities;
using Tests.Photino.NET.TestUtilities;
using System.Drawing;
using Monitor=InfiniLore.Photino.Monitor;

namespace Tests.Photino.NET.WindowFunctionalities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class MoveWithinCurrentMonitorAreaTests {
    
    [Test]
    [SkipUtility.OnMacOs]
    [NotInParallel(ParallelControl.Photino)]
    [Arguments(0, 0, 0,0)]
    [Arguments(100, 100, 100,100)]
    [Arguments(-100, -100, 0,0)]
    public async Task Window(int x, int y, int expectedX, int expectedY) {
        // Arrange
        using var windowUtility = WindowTestUtility.Create();
        IPhotinoWindow window = windowUtility.Window;

        // Act
        window.MoveWithinCurrentMonitorArea(x,y);

        // Assert
        int offsetX = 0;
        int offsetY = 0;
        window.Invoke(() => {
            MonitorsUtility.TryGetCurrentWindowAndMonitor(window, out Rectangle _, out Monitor monitor);
            offsetX = monitor.MonitorArea.X;
            offsetY = monitor.MonitorArea.Y;
        });
        await Assert.That(window.Location).IsEqualTo(new Point(offsetX + expectedX,offsetY + expectedY));
    }
}
