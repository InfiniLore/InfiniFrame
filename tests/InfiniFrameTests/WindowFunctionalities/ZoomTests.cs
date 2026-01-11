// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Native;
using InfiniFrameTests.Shared;

namespace InfiniFrameTests.WindowFunctionalities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ZoomTests {
    [Test]
    [DisplayName($"{nameof(ZoomTests)}.{nameof(Builder)}")]
    [Timeout(Timeout.Seconds10)]
    [MatrixDataSource]
    public async Task Builder([MatrixRange<int>(10, 200, 10)] int zoom, CancellationToken timeoutToken) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.SetZoom(zoom);

        // Assert
        await Assert.That(builder.Configuration.Zoom).IsEqualTo(zoom);

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters.Zoom).IsEqualTo(zoom);
    }

    [Test]
    [DisplayName($"{nameof(ZoomTests)}.{nameof(Window)}")]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(Timeout.Seconds10)]
    [SkipUtility.SkipOnLinux]
    [SkipUtility.SkipOnMacOs]
    public async Task Window(CancellationToken timeoutToken) {
        // Arrange
        const int zoom = 120;
        var windowUtility = InfiniFrameWindowThreadedTestUtility.Create(
            builder => builder
                .SetUseOsDefaultLocation(true)
                .SetUseOsDefaultSize(true)
                .SetZoomEnabled(true)
        );
        IInfiniFrameWindow window = windowUtility.Window;
        await Task.Delay(2000, timeoutToken);

        // Act
        window.SetZoom(zoom);

        // Assert
        await Assert.That(window.Zoom).IsEqualTo(zoom);
    }

    [Test]
    [DisplayName($"{nameof(ZoomTests)}.{nameof(FullIntegration)}")]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(Timeout.Seconds10)]
    [SkipUtility.SkipOnLinux]
    [SkipUtility.SkipOnMacOs]
    public async Task FullIntegration(CancellationToken timeoutToken) {
        // Arrange
        const int zoom = 120;

        // Act
        var windowUtility = InfiniFrameWindowThreadedTestUtility.Create(
            builder => builder
                .SetZoomEnabled(true)
                .SetZoom(zoom)
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.Zoom).IsEqualTo(zoom);
    }
}