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
public class FullScreenTests {

    [Test]
    [DisplayName($"{nameof(FullScreenTests)}.{nameof(Builder)}")]
    [Arguments(true)]
    [Arguments(false)]
    public async Task Builder(bool state) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.SetFullScreen(state);

        // Assert
        await Assert.That(builder.Configuration.FullScreen).IsEqualTo(state);

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters.FullScreen).IsEqualTo(state);
    }

    [Test]
    [DisplayName($"{nameof(FullScreenTests)}.{nameof(Window)}")]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(Timeout.Seconds10)]
    [SkipUtility.SkipOnMacOs]
    [Arguments(true)]
    [Arguments(false)]
    public async Task Window(bool state, CancellationToken timeoutToken) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create();
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetFullScreen(state);

        // Assert
        await Assert.That(window.FullScreen).IsEqualTo(state);
    }

    [Test]
    [DisplayName($"{nameof(FullScreenTests)}.{nameof(FullIntegration)}")]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(Timeout.Seconds10)]
    [SkipUtility.SkipOnMacOs]
    [Arguments(true)]
    [Arguments(false)]
    public async Task FullIntegration(bool state, CancellationToken timeoutToken) {
        // Arrange

        // Act
        using var windowUtility = InfiniFrameWindowTestUtility.Create(
            builder => builder
                .SetFullScreen(state)
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.FullScreen).IsEqualTo(state);
    }

}
