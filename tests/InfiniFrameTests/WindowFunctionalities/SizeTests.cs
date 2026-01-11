// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Native;
using InfiniFrameTests.Shared;
using System.Drawing;

namespace InfiniFrameTests.WindowFunctionalities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class SizeTests {
    private const int Width = 10;
    private const int Height = 20;

    [Test]
    [DisplayName($"{nameof(SizeTests)}.{nameof(Builder)}")]
    public async Task Builder() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.SetSize(Width, Height);

        // Assert
        await Assert.That(builder.Configuration.Width).IsEqualTo(Width);
        await Assert.That(builder.Configuration.Height).IsEqualTo(Height);

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters.Width).IsEqualTo(Width);
        await Assert.That(configParameters.Height).IsEqualTo(Height);
    }

    [Test]
    [DisplayName($"{nameof(SizeTests)}.{nameof(Builder_ShouldOverwriteOsDefaultSizeAndCentered)}")]
    public async Task Builder_ShouldOverwriteOsDefaultSizeAndCentered() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        InfiniFrameNativeParameters expectedConfigParameters = new InfiniFrameWindowConfiguration {
            Width = Width,
            Height = Height,
            UseOsDefaultSize = false,
            Centered = false
        }.ToParameters();

        // Act
        builder.SetUseOsDefaultSize(true);
        builder.SetSize(Width, Height);

        // Assert
        await Assert.That(builder.Configuration.Width).IsEqualTo(Width);
        await Assert.That(builder.Configuration.Height).IsEqualTo(Height);
        await Assert.That(builder.Configuration.UseOsDefaultSize).IsEqualTo(false);
        await Assert.That(builder.Configuration.Centered).IsEqualTo(false);

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters).IsEqualTo(expectedConfigParameters);
    }

    [Test]
    [DisplayName($"{nameof(SizeTests)}.{nameof(Window)}")]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(Timeout.Seconds10)]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux(SkipUtility.LinuxMovement)]
    public async Task Window(CancellationToken timeoutToken) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create();
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetSize(400, 500);

        // Assert
        await Assert.That(window.Size).IsEqualTo(new Size(400, 500));
    }

    [Test]
    [DisplayName($"{nameof(SizeTests)}.{nameof(Window_AsSize)}")] 
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(Timeout.Seconds10)]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux(SkipUtility.LinuxMovement)]
    public async Task Window_AsSize(CancellationToken timeoutToken) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create();
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetSize(new Size(400, 500));

        // Assert
        await Assert.That(window.Size).IsEqualTo(new Size(400, 500));
    }

    [Test]
    [DisplayName($"{nameof(SizeTests)}.{nameof(FullIntegration)}")]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(Timeout.Seconds10)]
    [SkipUtility.SkipOnMacOs]
    public async Task FullIntegration(CancellationToken timeoutToken) {
        // Arrange

        // Act
        using var windowUtility = InfiniFrameWindowTestUtility.Create(
            builder => builder
                .SetChromeless(true)
                .SetSize(400, 500)
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.Size).IsEqualTo(new Size(400, 500));
    }

    [Test]
    [DisplayName($"{nameof(SizeTests)}.{nameof(Window_WithChromelessToGetSmallestSize)}")]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(Timeout.Seconds10)]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux(SkipUtility.LinuxMovement)]
    public async Task Window_WithChromelessToGetSmallestSize(CancellationToken timeoutToken) {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create(builder => builder.SetChromeless(true));
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetSize(Width, Height);

        // Assert
        await Assert.That(window.Size).IsEqualTo(new Size(Width, Height));
    }

    [Test]
    [DisplayName($"{nameof(SizeTests)}.{nameof(FullIntegration_WithChromelessToGetSmallestSize)}")]
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(Timeout.Seconds10)]
    [SkipUtility.SkipOnMacOs]
    public async Task FullIntegration_WithChromelessToGetSmallestSize(CancellationToken timeoutToken) {
        // Arrange

        // Act
        using var windowUtility = InfiniFrameWindowTestUtility.Create(
            builder => builder
                .SetChromeless(true)
                .SetSize(Width, Height)
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.Size).IsEqualTo(new Size(Width, Height));
    }
}
