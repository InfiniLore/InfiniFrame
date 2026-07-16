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
public class CenterOnCurrentMonitorTests {
    [Test]
    [DisplayName($"{nameof(CenterOnCurrentMonitorTests)}.{nameof(Window)}")]
    [SkipOnMacOs]
    [SkipOnLinux(SkipUtility.LinuxMovement)]
    [NotInParallelInfiniTests]
    public async Task Window(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.CenterOnCurrentMonitor();

        // Assert
        int centerX = 0;
        int centerY = 0;
        window.Invoke(() => {
            MonitorsUtility.TryGetCurrentWindowAndMonitor(window, out Rectangle windowRect, out InfiniMonitor monitor);
            Size size = windowRect.Size;
            centerX = monitor.MonitorArea.Width / 2 - size.Width / 2;
            centerY = monitor.MonitorArea.Height / 2 - size.Height / 2;
        });

        await Assert.That(window.Location).IsEqualTo(new Point(centerX, centerY));
    }
}
