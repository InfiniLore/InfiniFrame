// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;
using InfiniFrameTests.Shared;

namespace InfiniFrameTests.WindowFunctionalities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ZoomTests {
    [Test]
    [DisplayName($"{nameof(ZoomTests)}.{nameof(Builder)}")]
    [MatrixDataSource]
    public async Task Builder([MatrixRange<int>(10, 200, 10)] int zoom, CancellationToken ct = default) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.SetZoom(zoom);

        // Assert
        await Assert.That(builder.Configuration.Zoom).IsEqualTo(zoom);

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToNativeParameters();
        await Assert.That(configParameters.Zoom).IsEqualTo(zoom);
    }

    [Test]
    [DisplayName($"{nameof(ZoomTests)}.{nameof(Window)}")]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [SkipUtility.SkipOnLinux]
    [SkipUtility.SkipOnMacOs]
    public async Task Window(CancellationToken ct = default) {
        // Arrange
        const int zoom = 120;
        using var windowUtility = InfiniFrameWindowTestUtility.Create(
            builder => builder
                .SetUseOsDefaultLocation(true)
                .SetUseOsDefaultSize(true)
                .SetZoomEnabled(true),
            ct
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetZoom(zoom);

        // Assert
        await Assert.That(window.Zoom).IsEqualTo(zoom);
    }

    [Test]
    [DisplayName($"{nameof(ZoomTests)}.{nameof(FullIntegration)}")]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [SkipUtility.SkipOnLinux]
    [SkipUtility.SkipOnMacOs]
    [MatrixDataSource]
    public async Task FullIntegration([MatrixRange<int>(26, 250, 10)] int zoom, CancellationToken ct = default) {
        // Arrange

        // Act
        using var windowUtility = InfiniFrameWindowTestUtility.Create(
            builder => builder
                .SetZoomEnabled(true)
                .SetZoom(zoom),
            ct
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.Zoom).IsEqualTo(zoom);
    }
}
