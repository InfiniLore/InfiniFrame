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
public class DevToolsTests {

    [Test]
    [DisplayName($"{nameof(DevToolsTests)}.{nameof(Builder)}")]
    [Arguments(true)]
    [Arguments(false)]
    public async Task Builder(bool state) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.SetDevToolsEnabled(state);

        // Assert
        await Assert.That(builder.Configuration.DevToolsEnabled).IsEqualTo(state);

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters.DevToolsEnabled).IsEqualTo(state);
    }

    [Test]
    [DisplayName($"{nameof(DevToolsTests)}.{nameof(Window)}")]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(Timeout.Seconds10)]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnWindows("For some reason it keeps tripping up the transport connection")]
    [Arguments(true)]
    [Arguments(false)]
    public async Task Window(bool state, CancellationToken timeoutToken) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create();
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetDevToolsEnabled(state);

        // Assert
        bool foundState = window.DevToolsEnabled;
        await Assert.That(foundState).IsEqualTo(state);
    }

    [Test]
    [DisplayName($"{nameof(DevToolsTests)}.{nameof(FullIntegration)}")]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(Timeout.Seconds10)]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnWindows("For some reason it keeps tripping up the transport connection")]
    [Arguments(true)]
    [Arguments(false)]
    public async Task FullIntegration(bool state, CancellationToken timeoutToken) {
        // Arrange

        // Act
        var windowUtility = InfiniFrameWindowTestUtility.Create(
            builder => builder
                .SetDevToolsEnabled(state)
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        bool foundState = window.DevToolsEnabled;
        await Assert.That(foundState).IsEqualTo(state);
    }

}
