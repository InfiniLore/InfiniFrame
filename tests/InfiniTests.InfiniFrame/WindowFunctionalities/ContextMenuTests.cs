// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.WindowFunctionalities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ContextMenuTests {

    [Test]
    [DisplayName($"{nameof(ContextMenuTests)}.{nameof(Builder)}")]
    [Arguments(true)]
    [Arguments(false)]
    public async Task Builder(bool state, CancellationToken ct = default) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.SetContextMenuEnabled(state);

        // Assert
        await Assert.That(builder.Configuration.ContextMenuEnabled).IsEqualTo(state);

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToNativeParameters();
        await Assert.That(configParameters.ContextMenuEnabled).IsEqualTo(state);
    }

    [Test]
    [DisplayName($"{nameof(ContextMenuTests)}.{nameof(Window)}")]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    [Arguments(true)]
    [Arguments(false)]
    public async Task Window(bool state, CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetContextMenuEnabled(state);

        // Assert
        bool foundState = window.ContextMenuEnabled;
        await Assert.That(foundState).IsEqualTo(state);
    }

    [Test]
    [DisplayName($"{nameof(ContextMenuTests)}.{nameof(FullIntegration)}")]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    [Arguments(true)]
    [Arguments(false)]
    public async Task FullIntegration(bool state, CancellationToken ct = default) {
        // Arrange

        // Act
        using var windowUtility = InfiniFrameTestWindow.Create(
            builder => builder.SetContextMenuEnabled(state),
            ct
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        bool foundState = window.ContextMenuEnabled;
        await Assert.That(foundState).IsEqualTo(state);
    }

}
