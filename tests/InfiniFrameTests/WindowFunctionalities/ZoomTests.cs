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
    public async Task Builder([MatrixRange<int>(0, 200, 10)] int zoom) {
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
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [MatrixDataSource]
    public async Task Window([MatrixRange<int>(10, 200, 10)] int zoom) {
        // Arrange
        var windowUtility = InfiniFrameWindowTestUtility.Create();
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetZoom(zoom);

        // Assert
        await Assert.That(window.Zoom).IsEqualTo(zoom);
    }

    [Test]
    [DisplayName($"{nameof(ZoomTests)}.{nameof(FullIntegration)}")]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [MatrixDataSource]
    public async Task FullIntegration([MatrixRange<int>(10, 200, 10)] int zoom) {
        // Arrange

        // Act
        var windowUtility = InfiniFrameWindowTestUtility.Create(
            builder => builder
                .SetZoom(zoom)
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.Zoom).IsEqualTo(zoom);
    }
}
