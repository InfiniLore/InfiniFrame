// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.InfiniFrame.NET;

namespace Tests.Photino.NET.WindowFunctionalities;
using InfiniLore.InfiniFrame;
using InfiniLore.Photino.Utilities;
using System.Drawing;
using Tests.Shared.Photino;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class CenterOnMonitorTests {
    [Test]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux(SkipUtility.LinuxMovement)]
    [NotInParallel(ParallelControl.Photino)]
    public async Task Window() {
        // Arrange
        using var windowUtility = WindowTestUtility.Create();
        IInfiniWindow window = windowUtility.Window;

        // Act
        window.CenterOnMonitor();

        // Assert
        int centerX = 0;
        int centerY = 0;
        window.Invoke(() => {
            MonitorsUtility.TryGetCurrentWindowAndMonitor(window, out Rectangle windowRect, out Monitor monitor);
            Size size = windowRect.Size;
            centerX = monitor.MonitorArea.Width/2  - size.Width / 2;
            centerY = monitor.MonitorArea.Height/2 - size.Height / 2;
        });
        
        await Assert.That(window.Location).IsEqualTo(new Point(centerX, centerY));
    }
    
}
