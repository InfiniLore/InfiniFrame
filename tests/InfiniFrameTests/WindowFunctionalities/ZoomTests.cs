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
    [MatrixDataSource]
    public async Task Builder([MatrixRange<int>(10, 200, 10)] int zoom) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.SetZoom(zoom);

        // Assert
        await Assert.That(builder.Configuration.Zoom).IsEqualTo(zoom);

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters.Zoom).IsEqualTo(zoom);
    }

    // TODO: fix this test
    [Test]
    [DisplayName($"{nameof(ZoomTests)}.{nameof(Window)}")]
    [Skip("THIS IS NOT WORKING IN TEST ENVIRONMENT, but is working in real application")]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [SkipUtility.SkipOnLinux]
    [SkipUtility.SkipOnMacOs]
    // [MatrixDataSource]
    public async Task Window() {
        // Arrange
        const int zoom = 120;
        var windowUtility = InfiniFrameWindowThreadedTestUtility.Create(
            builder => builder
                .SetUseOsDefaultLocation(true)
                .SetUseOsDefaultSize(true)
                .SetZoomEnabled(true)
        );
        IInfiniFrameWindow window = windowUtility.Window;
        await Task.Delay(2000);

        // Act
        window.SetZoom(zoom);

        // Assert
        await Assert.That(window.Zoom).IsEqualTo(zoom);
    }

    // TODO: fix this test
    [Test]
    [DisplayName($"{nameof(ZoomTests)}.{nameof(FullIntegration)}")]
    [Skip("THIS IS NOT WORKING IN TEST ENVIRONMENT, but is working in real application")]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [SkipUtility.SkipOnLinux]
    [SkipUtility.SkipOnMacOs]
    [MatrixDataSource]
    public async Task FullIntegration([MatrixRange<int>(26, 250, 10)] int zoom) {
        // Arrange

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