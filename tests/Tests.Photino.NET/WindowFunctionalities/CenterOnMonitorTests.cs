// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.Photino.NET;
using Tests.Photino.NET.TestUtilities;

namespace Tests.Photino.NET.WindowFunctionalities;
using InfiniLore.Photino;
using InfiniLore.Photino.Utilities;
using System.Drawing;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class CenterOnMonitorTests {
    [Test]
    [SkipUtility.OnMacOs]
    public async Task Window() {
        // Arrange
        using var windowUtility = WindowTestUtility.Create();
        IPhotinoWindow window = windowUtility.Window;

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
