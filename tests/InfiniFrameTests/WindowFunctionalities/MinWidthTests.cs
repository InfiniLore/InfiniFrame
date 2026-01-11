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
public class MinWidthTests {
    private const int MinWidth = 20;

    [Test]
    [DisplayName($"{nameof(MinWidthTests)}.{nameof(Builder)}")]
    public async Task Builder() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.SetMinWidth(MinWidth);

        // Assert
        await Assert.That(builder.Configuration.MinWidth).IsEqualTo(MinWidth);

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters.MinWidth).IsEqualTo(MinWidth);
    }

    [Test]
    [DisplayName($"{nameof(MinWidthTests)}.{nameof(Window)}")]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(Timeout.Seconds10)]
    [SkipUtility.SkipOnMacOs]
    public async Task Window(CancellationToken timeoutToken) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create();
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetMinWidth(500);

        // Assert
        await Assert.That(window.MinWidth).IsEqualTo(500);
    }

    [Test]
    [DisplayName($"{nameof(MinWidthTests)}.{nameof(FullIntegration)}")]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(Timeout.Seconds10)]
    [SkipUtility.SkipOnMacOs]
    public async Task FullIntegration(CancellationToken timeoutToken) {
        // Arrange

        // Act
        using var windowUtility = InfiniFrameWindowTestUtility.Create(
            builder => builder
                .SetMinWidth(500)
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.MinWidth).IsEqualTo(500);
    }
}
