// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Debugging;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.WindowFunctionalities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class DevToolsTests {

    [Test]
    [DisplayName($"{nameof(DevToolsTests)}.{nameof(Builder)}")]
    [Arguments(true)]
    [Arguments(false)]
    public async Task Builder(bool state, CancellationToken ct = default) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.Debug.SetDevToolsEnabled(state);

        // Assert
        await Assert.That(builder.Debug.DevToolsEnabled).IsEqualTo(state);

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToNativeParameters();
        await Assert.That(configParameters.DevToolsEnabled).IsEqualTo(state);
    }

    [Test]
    [DisplayName($"{nameof(DevToolsTests)}.{nameof(Window)}")]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    [Arguments(true)]
    [Arguments(false)]
    public async Task Window(bool state, CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Debug.SetDevToolsEnabled(state);

        // Assert
        bool foundState = window.Debug.DevToolsEnabled;
        await Assert.That(foundState).IsEqualTo(state);
    }

    [Test]
    [DisplayName($"{nameof(DevToolsTests)}.{nameof(FullIntegration)}")]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    [Arguments(true)]
    [Arguments(false)]
    public async Task FullIntegration(bool state, CancellationToken ct = default) {
        // Arrange

        // Act
        using var windowUtility = InfiniFrameTestWindow.Create(
            builder: builder => builder.SetDevToolsEnabled(state),
            ct
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        bool foundState = window.Debug.DevToolsEnabled;
        await Assert.That(foundState).IsEqualTo(state);
    }
}
