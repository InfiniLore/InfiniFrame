// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Native;

namespace InfiniFrameTests.WindowFunctionalities;
using InfiniFrameTests.Shared;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class TopMostTests {

    [Test]
    [DisplayName($"{nameof(TopMostTests)}.{nameof(Builder)}")]
    [Arguments(true)]
    [Arguments(false)]
    public async Task Builder(bool state) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.SetTopMost(state);

        // Assert
        await Assert.That(builder.Configuration.TopMost).IsEqualTo(state);

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters.Topmost).IsEqualTo(state);
    }

    [Test]
    [DisplayName($"{nameof(TopMostTests)}.{nameof(Window)}")]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(Timeout.Seconds10)]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux]
    [Arguments(true)]
    [Arguments(false)]
    public async Task Window(bool state, CancellationToken timeoutToken) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create();
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetTopMost(state);

        // Assert
        await Assert.That(window.TopMost).IsEqualTo(state);
    }

    [Test]
    [DisplayName($"{nameof(TopMostTests)}.{nameof(FullIntegration)}")]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(Timeout.Seconds10)]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux]
    [Arguments(true)]
    [Arguments(false)]
    public async Task FullIntegration(bool state, CancellationToken timeoutToken) {
        // Arrange

        // Act
        using var windowUtility = InfiniFrameWindowTestUtility.Create(
            builder => builder
                .SetTopMost(state)
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.TopMost).IsEqualTo(state);
    }

}
