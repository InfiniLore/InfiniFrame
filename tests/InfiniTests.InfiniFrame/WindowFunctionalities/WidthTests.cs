// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.WindowFunctionalities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WidthTests {
    private const int Width = 20;

    // -----------------------------------------------------------------------------------------------------------------
    // Test Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Test, DisplayName($"{nameof(WidthTests)}.{nameof(Builder)}")]
    public async Task Builder(CancellationToken ct = default) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.SetUseOsDefaultSize(true);
        builder.SetWidth(Width);

        // Assert
        await Assert.That(builder.Configuration.Width).IsEqualTo(Width);

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToNativeParameters();
        await Assert.That(configParameters.Width).IsEqualTo(Width);
    }

    [Test, DisplayName($"{nameof(WidthTests)}.{nameof(Builder_ShouldOverwriteOsDefaultSizeAndCentered)}")]
    public async Task Builder_ShouldOverwriteOsDefaultSizeAndCentered() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        InfiniFrameNativeParameters expectedConfigParameters = new InfiniFrameOptionsBuilder {
            Width = Width,
            UseOsDefaultSize = false,
            Centered = false
        }.ToNativeParameters();

        // Act
        builder.SetUseOsDefaultSize(true);
        builder.SetWidth(Width);

        // Assert
        await Assert.That(builder.Configuration.Width).IsEqualTo(Width);
        await Assert.That(builder.Configuration.UseOsDefaultSize).IsFalse();
        await Assert.That(builder.Configuration.Centered).IsFalse();

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToNativeParameters();
        await Assert.That(configParameters).IsEqualTo(expectedConfigParameters);
    }

    [Test, DisplayName($"{nameof(WidthTests)}.{nameof(Window)}"), SkipOnMacOs, SkipOnLinux(SkipUtility.LinuxMovement), NotInParallelInfiniTests]
    public async Task Window(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetWidth(500);

        // Assert
        await Assert.That(window.Width).IsEqualTo(500);
    }

    [Test, DisplayName($"{nameof(WidthTests)}.{nameof(FullIntegration)}"), SkipOnMacOs, SkipOnLinux(SkipUtility.LinuxMovement), NotInParallelInfiniTests]
    public async Task FullIntegration(CancellationToken ct = default) {
        // Arrange

        // Act
        using var windowUtility = InfiniFrameTestWindow.Create(
            builder: builder => builder
                .SetChromeless(true)
                .SetWidth(500),
            ct
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.Width).IsEqualTo(500);
    }

    [Test, DisplayName($"{nameof(WidthTests)}.{nameof(Window_WithChromelessToGetSmallestWidth)}"), SkipOnMacOs, SkipOnLinux(SkipUtility.LinuxMovement), NotInParallelInfiniTests]
    public async Task Window_WithChromelessToGetSmallestWidth(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(
            builder: builder => builder.SetChromeless(true),
            ct
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetWidth(Width);

        // Assert
        await Assert.That(window.Width).IsEqualTo(Width);
    }

    [Test, DisplayName($"{nameof(WidthTests)}.{nameof(FullIntegration_WithChromelessToGetSmallestWidth)}"), SkipOnMacOs, NotInParallelInfiniTests]
    public async Task FullIntegration_WithChromelessToGetSmallestWidth(CancellationToken ct = default) {
        // Arrange

        // Act
        using var windowUtility = InfiniFrameTestWindow.Create(
            builder: builder => builder
                .SetChromeless(true)
                .SetWidth(Width),
            ct
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.Width).IsEqualTo(Width);
    }
}
