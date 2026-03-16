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
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(TimeoutUtility.DefaultTimeout)]
    [Arguments(true)]
    [Arguments(false)]
    public async Task Window(bool state, CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetTopMost(state);

        // Assert
        await Assert.That(window.TopMost).IsEqualTo(state);
    }

    [Test]
    [DisplayName($"{nameof(TopMostTests)}.{nameof(FullIntegration)}")]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(TimeoutUtility.DefaultTimeout)]
    [Arguments(true)]
    [Arguments(false)]
    public async Task FullIntegration(bool state, CancellationToken ct) {
        // Arrange

        // Act
        using var windowUtility = InfiniFrameWindowTestUtility.Create(
            builder => builder.SetTopMost(state),
            ct
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.TopMost).IsEqualTo(state);
    }

}
