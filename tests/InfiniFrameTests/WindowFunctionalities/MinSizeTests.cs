// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Native;
using InfiniFrameTests.Shared;
using System.Drawing;

namespace InfiniFrameTests.WindowFunctionalities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class MinSizeTests {
    private const int Width = 10;
    private const int Height = 20;

    [Test]
    [DisplayName($"{nameof(MinSizeTests)}.{nameof(Builder)}")]
    public async Task Builder() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.SetMinSize(Width, Height);

        // Assert
        await Assert.That(builder.Configuration.MinWidth).IsEqualTo(Width);
        await Assert.That(builder.Configuration.MinHeight).IsEqualTo(Height);

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters.MinWidth).IsEqualTo(Width);
        await Assert.That(configParameters.MinHeight).IsEqualTo(Height);
    }

    [Test]
    [DisplayName($"{nameof(MinSizeTests)}.{nameof(Window)}")]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(Timeout.Seconds10)]
    [SkipUtility.SkipOnMacOs]
    public async Task Window(CancellationToken timeoutToken) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create();
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetMinSize(400, 500);

        // Assert
        await Assert.That(window.MinSize).IsEqualTo(new Size(400, 500));
    }

    [Test]
    [DisplayName($"{nameof(MinSizeTests)}.{nameof(Window_AsSize)}")]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(Timeout.Seconds10)]
    [SkipUtility.SkipOnMacOs]
    public async Task Window_AsSize(CancellationToken timeoutToken) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create();
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetMinSize(new Size(400, 500));

        // Assert
        await Assert.That(window.MinSize).IsEqualTo(new Size(400, 500));
    }

    [Test]
    [DisplayName($"{nameof(MinSizeTests)}.{nameof(FullIntegration)}")]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(Timeout.Seconds10)]
    [SkipUtility.SkipOnMacOs]
    public async Task FullIntegration(CancellationToken timeoutToken) {
        // Arrange

        // Act
        using var windowUtility = InfiniFrameWindowTestUtility.Create(
            builder => builder
                .SetChromeless(true)
                .SetMinSize(400, 500)
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.MinSize).IsEqualTo(new Size(400, 500));
    }
}
