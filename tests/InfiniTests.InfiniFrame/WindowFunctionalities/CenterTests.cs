// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;
using InfiniFrame.Utilities;
using System.Drawing;

namespace InfiniTests.InfiniFrame.WindowFunctionalities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class CenterTests {

    [Test]
    [DisplayName($"{nameof(CenterTests)}.{nameof(Builder)}")]
    [Arguments(true)]
    [Arguments(false)]
    public async Task Builder(bool state, CancellationToken ct = default) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.Center(state);

        // Assert
        await Assert.That(builder.Configuration.Centered).IsEqualTo(state);

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToNativeParameters();
        await Assert.That(configParameters.CenterOnInitialize).IsEqualTo(state);
    }

    [Test]
    [DisplayName($"{nameof(CenterTests)}.{nameof(Window)}")]
    [SkipOnMacOs]
    [SkipOnLinux(SkipUtility.LinuxMovement)]
    [NotInParallelInfiniTests]
    public async Task Window(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Center();

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

    [Test]
    [DisplayName($"{nameof(CenterTests)}.{nameof(FullIntegration)}")]
    [SkipOnMacOs]
    [SkipOnLinux(SkipUtility.LinuxMovement)]
    [NotInParallelInfiniTests]
    [Arguments(true)]
    [Arguments(false)]
    public async Task FullIntegration(bool state, CancellationToken ct = default) {
        // Arrange

        // Act
        using var windowUtility = InfiniFrameTestWindow.Create(
            builder: builder => builder.Center(state),
            ct
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        int centerX = 0;
        int centerY = 0;
        window.Invoke(() => {
            MonitorsUtility.TryGetCurrentWindowAndMonitor(window, out Rectangle windowRect, out InfiniMonitor monitor);
            Size size = windowRect.Size;
            centerX = monitor.MonitorArea.Width / 2 - size.Width / 2;
            centerY = monitor.MonitorArea.Height / 2 - size.Height / 2;
        });

        if (state) await Assert.That(window.Location).IsEqualTo(new Point(centerX, centerY));
        else await Assert.That(window.Location).IsNotEqualTo(new Point(centerX, centerY));
    }
}
