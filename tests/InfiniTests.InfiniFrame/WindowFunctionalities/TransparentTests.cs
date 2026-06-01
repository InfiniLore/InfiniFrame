// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.WindowFunctionalities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class TransparentTests {

    [Test, DisplayName($"{nameof(TransparentTests)}.{nameof(Builder)}"), Arguments(true), Arguments(false)]
    public async Task Builder(bool state, CancellationToken ct = default) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.SetTransparent(state);

        // Assert
        await Assert.That(builder.Configuration.Transparent).IsEqualTo(state);

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToNativeParameters();
        await Assert.That(configParameters.Transparent).IsEqualTo(state);
    }

    [Test, DisplayName($"{nameof(TransparentTests)}.{nameof(Window)}"), SkipOnMacOs, SkipOnLinux("Headless display lacks compositing support for post-init transparency changes"), NotInParallelInfiniTests, Arguments(true), Arguments(false)]
    public async Task Window(bool state, CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetTransparent(true);

        // Assert
        if (OperatingSystem.IsWindows()) state = false;// Windows does not support transparency after initialization
        await Assert.That(window.Transparent).IsEqualTo(state);
    }

    [Test, DisplayName($"{nameof(TransparentTests)}.{nameof(FullIntegration)}"), SkipOnMacOs, NotInParallelInfiniTests, Arguments(true), Arguments(false)]
    public async Task FullIntegration(bool state, CancellationToken ct = default) {
        // Arrange

        // Act
        using var windowUtility = InfiniFrameTestWindow.Create(
            builder: builder => builder.SetTransparent(state),
            ct
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.Transparent).IsEqualTo(state);
    }
}
