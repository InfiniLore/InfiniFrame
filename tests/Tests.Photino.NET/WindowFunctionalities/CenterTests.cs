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
public class CenterTests {

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task Builder(bool state) {
        // Arrange
        var builder = PhotinoWindowBuilder.Create();

        // Act
        builder.Center(state);

        // Assert
        await Assert.That(builder.Configuration.Centered).IsEqualTo(state);

        PhotinoNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters.CenterOnInitialize).IsEqualTo(state);
    }
    
    [Test]
    [SkipUtility.OnMacOs]
    public async Task Window() {
        // Arrange
        using var windowUtility = WindowTestUtility.Create();
        IPhotinoWindow window = windowUtility.Window;

        // Act
        window.Center();

        // Assert
        MonitorsUtility.TryGetCurrentWindowAndMonitor(window, out Rectangle windowRect, out Monitor monitor);
        Size size = windowRect.Size;
        int centerX = monitor.MonitorArea.Width/2  - size.Width / 2;
        int centerY = monitor.MonitorArea.Height/2 - size.Height / 2;
        
        await Assert.That(window.Location).IsEqualTo(new Point(centerX, centerY));
    }

    [Test]
    [SkipUtility.OnMacOs]
    [Arguments(true)]
    [Arguments(false)]
    public async Task FullIntegration(bool state) {
        // Arrange

        // Act
        using var windowUtility = WindowTestUtility.Create(
            builder => builder
                .Center(state)
        );
        IPhotinoWindow window = windowUtility.Window;

        // Assert
        MonitorsUtility.TryGetCurrentWindowAndMonitor(window, out Rectangle windowRect, out Monitor monitor);
        Size size = windowRect.Size;
        int centerX = monitor.MonitorArea.Width/2  - size.Width / 2;
        int centerY = monitor.MonitorArea.Height/2 - size.Height / 2;
        
        if (state) await Assert.That(window.Location).IsEqualTo(new Point(centerX, centerY));
        else await Assert.That(window.Location).IsNotEqualTo(new Point(centerX, centerY));
    }
    
}
