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

    [Test]
    [DisplayName($"{nameof(ZoomTests)}.{nameof(Window)}")]
    [NotInParallel(ParallelControl.InfiniFrame)]
    // [SkipUtility.SkipOnLinux]
    // [SkipUtility.SkipOnMacOs]
    public async Task Window() {
        // Arrange
        const int zoom = 120;
        using var windowUtility = InfiniFrameWindowThreadedTestUtility.Create(
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

    [Test]
    [DisplayName($"{nameof(ZoomTests)}.{nameof(FullIntegration)}")]
    [NotInParallel(ParallelControl.InfiniFrame)]
    // [SkipUtility.SkipOnLinux]
    // [SkipUtility.SkipOnMacOs]
    [Arguments(26)]
    [Arguments(36)]
    [Arguments(46)]
    [Arguments(56)]
    [Arguments(66)]
    [Arguments(76)]
    [Arguments(86)]
    [Arguments(96)]
    [Arguments(106)]
    [Arguments(116)]
    [Arguments(126)]
    [Arguments(136)]
    [Arguments(146)]
    [Arguments(156)]
    [Arguments(166)]
    [Arguments(176)]
    [Arguments(186)]
    [Arguments(196)]
    [Arguments(206)]
    [Arguments(216)]
    [Arguments(226)]
    [Arguments(236)]
    [Arguments(246)]
    [Arguments(250)]
    public async Task FullIntegration(int zoom) {
        // Arrange

        // Act
        using var windowUtility = InfiniFrameWindowThreadedTestUtility.Create(
            builder => builder
                .SetZoomEnabled(true)
                .SetZoom(zoom)
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.Zoom).IsEqualTo(zoom);
    }
}
