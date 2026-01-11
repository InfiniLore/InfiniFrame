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
public class MaximizeTests {

    [Test]
    [DisplayName($"{nameof(MaximizeTests)}.{nameof(Builder)}")]
    [Arguments(true)]
    [Arguments(false)]
    public async Task Builder(bool state) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.SetMaximized(state);

        // Assert
        await Assert.That(builder.Configuration.Maximized).IsEqualTo(state);

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters.Maximized).IsEqualTo(state);
    }

    [Test]
    [DisplayName($"{nameof(MaximizeTests)}.{nameof(Window)}")]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(Timeout.Seconds10)]
    [SkipUtility.SkipOnMacOs]
    [Arguments(true)]
    [Arguments(false)]
    public async Task Window(bool state, CancellationToken timeoutToken) {
        SkipUtility.SkipOnLinux(state);

        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create();
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetMaximized(state);

        // Assert
        await Assert.That(window.Maximized).IsEqualTo(state);
    }

    [Test]
    [DisplayName($"{nameof(MaximizeTests)}.{nameof(Window_Toggle)}")]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(Timeout.Seconds10)]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux]
    [Arguments(true)]
    [Arguments(false)]
    public async Task Window_Toggle(bool state, CancellationToken timeoutToken) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create();
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetMaximized(state);
        window.ToggleMaximized();

        // Assert
        await Assert.That(window.Maximized).IsEqualTo(!state);
    }

    [Test]
    [DisplayName($"{nameof(MaximizeTests)}.{nameof(FullIntegration)}")]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(Timeout.Seconds10)]
    [SkipUtility.SkipOnMacOs]
    [Arguments(true)]
    [Arguments(false)]
    public async Task FullIntegration(bool state, CancellationToken timeoutToken) {
        SkipUtility.SkipOnLinux(state);

        // Arrange

        // Act
        using var windowUtility = InfiniFrameWindowTestUtility.Create(
            builder => builder
                .SetMaximized(state)
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.Maximized).IsEqualTo(state);
    }

}
