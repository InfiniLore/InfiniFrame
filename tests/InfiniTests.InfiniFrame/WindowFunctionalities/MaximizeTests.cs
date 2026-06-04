// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.WindowFunctionalities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class MaximizeTests {

    [Test]
    [DisplayName($"{nameof(MaximizeTests)}.{nameof(Builder)}")]
    [Arguments(true)]
    [Arguments(false)]
    public async Task Builder(bool state, CancellationToken ct = default) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.SetMaximized(state);

        // Assert
        await Assert.That(builder.Configuration.Maximized).IsEqualTo(state);

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToNativeParameters();
        await Assert.That(configParameters.Maximized).IsEqualTo(state);
    }

    [Test]
    [DisplayName($"{nameof(MaximizeTests)}.{nameof(Window)}")]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    [Arguments(true)]
    [Arguments(false)]
    public async Task Window(bool state, CancellationToken ct = default) {
        SkipUtility.SkipOnLinux(state);

        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetMaximized(state);

        // Assert
        await Assert.That(window.Maximized).IsEqualTo(state);
    }

    [Test]
    [DisplayName($"{nameof(MaximizeTests)}.{nameof(Window_Toggle)}")]
    [SkipOnMacOs]
    [SkipOnLinux]
    [NotInParallelInfiniTests]
    [Arguments(true)]
    [Arguments(false)]
    public async Task Window_Toggle(bool state, CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetMaximized(state);
        window.ToggleMaximized();

        // Assert
        await Assert.That(window.Maximized).IsEqualTo(!state);
    }

    [Test]
    [DisplayName($"{nameof(MaximizeTests)}.{nameof(FullIntegration)}")]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    [Arguments(true)]
    [Arguments(false)]
    public async Task FullIntegration(bool state, CancellationToken ct = default) {
        SkipUtility.SkipOnLinux(state);

        // Arrange

        // Act
        using var windowUtility = InfiniFrameTestWindow.Create(
            builder: builder => builder.SetMaximized(state),
            ct
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.Maximized).IsEqualTo(state);
    }
}
