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
public class WidthTests {
    private const int Width = 20;

    [Test]
    [DisplayName($"{nameof(WidthTests)}.{nameof(Builder)}")]
    public async Task Builder() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.SetUseOsDefaultSize(true);
        builder.SetWidth(Width);

        // Assert
        await Assert.That(builder.Configuration.Width).IsEqualTo(Width);

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters.Width).IsEqualTo(Width);
    }

    [Test]
    [DisplayName($"{nameof(WidthTests)}.{nameof(Builder_ShouldOverwriteOsDefaultSizeAndCentered)}")]
    public async Task Builder_ShouldOverwriteOsDefaultSizeAndCentered() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        InfiniFrameNativeParameters expectedConfigParameters = new InfiniFrameWindowConfiguration {
            Width = Width,
            UseOsDefaultSize = false,
            Centered = false
        }.ToParameters();

        // Act
        builder.SetUseOsDefaultSize(true);
        builder.SetWidth(Width);

        // Assert
        await Assert.That(builder.Configuration.Width).IsEqualTo(Width);
        await Assert.That(builder.Configuration.UseOsDefaultSize).IsEqualTo(false);
        await Assert.That(builder.Configuration.Centered).IsEqualTo(false);

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters).IsEqualTo(expectedConfigParameters);
    }

    [Test]
    [DisplayName($"{nameof(WidthTests)}.{nameof(Window)}")]  
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(Timeout.Seconds10)]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux(SkipUtility.LinuxMovement)]
    public async Task Window(CancellationToken timeoutToken) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create();
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetWidth(500);

        // Assert
        await Assert.That(window.Width).IsEqualTo(500);
    }

    [Test]
    [DisplayName($"{nameof(WidthTests)}.{nameof(FullIntegration)}")] 
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(Timeout.Seconds10)]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux(SkipUtility.LinuxMovement)]
    public async Task FullIntegration(CancellationToken timeoutToken) {
        // Arrange

        // Act
        using var windowUtility = InfiniFrameWindowTestUtility.Create(
            builder => builder
                .SetChromeless(true)
                .SetWidth(500)
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.Width).IsEqualTo(500);
    }

    [Test]
    [DisplayName($"{nameof(WidthTests)}.{nameof(Window_WithChromelessToGetSmallestWidth)}")] 
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(Timeout.Seconds10)]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux(SkipUtility.LinuxMovement)]
    public async Task Window_WithChromelessToGetSmallestWidth(CancellationToken timeoutToken) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create(builder => builder.SetChromeless(true));
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetWidth(Width);

        // Assert
        await Assert.That(window.Width).IsEqualTo(Width);
    }

    [Test]
    [DisplayName($"{nameof(WidthTests)}.{nameof(FullIntegration_WithChromelessToGetSmallestWidth)}")]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(Timeout.Seconds10)]
    [SkipUtility.SkipOnMacOs]
    public async Task FullIntegration_WithChromelessToGetSmallestWidth(CancellationToken timeoutToken) {
        // Arrange

        // Act
        using var windowUtility = InfiniFrameWindowTestUtility.Create(
            builder => builder
                .SetChromeless(true)
                .SetWidth(Width)
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.Width).IsEqualTo(Width);
    }
}
