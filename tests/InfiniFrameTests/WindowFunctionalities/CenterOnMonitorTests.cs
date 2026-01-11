// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Utilities;
using InfiniFrameTests.Shared;
using System.Drawing;

namespace InfiniFrameTests.WindowFunctionalities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class CenterOnMonitorTests {
    [Test]
    [DisplayName($"{nameof(CenterOnMonitorTests)}.{nameof(Window)}")]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(Timeout.Seconds10)]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux(SkipUtility.LinuxMovement)]
    public async Task Window(CancellationToken timeoutToken) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create();
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.CenterOnMonitor(0);

        // Assert
        int centerX = 0;
        int centerY = 0;
        window.Invoke(() => {
            MonitorsUtility.TryGetCurrentWindowAndMonitor(window, out Rectangle windowRect, out InfiniMonitor monitor);
            Size size = windowRect.Size;
            centerX = monitor.MonitorArea.X + monitor.MonitorArea.Width / 2 - size.Width / 2;
            centerY = monitor.MonitorArea.Y + monitor.MonitorArea.Height / 2 - size.Height / 2;
        });

        await Assert.That(window.Location).IsEqualTo(new Point(centerX, centerY));
    }

}
