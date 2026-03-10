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
public class CenterOnCurrentMonitorTests {
    [Test]
    [DisplayName($"{nameof(CenterOnCurrentMonitorTests)}.{nameof(Window)}")]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux(SkipUtility.LinuxMovement)]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(TimeoutUtility.DefaultTimeout)]
    public async Task Window(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create(ct);
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
