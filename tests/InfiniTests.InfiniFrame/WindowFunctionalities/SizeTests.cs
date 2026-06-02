// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;
using System.Drawing;

namespace InfiniTests.InfiniFrame.WindowFunctionalities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class SizeTests {
    private const int Width = 10;
    private const int Height = 20;

    // -----------------------------------------------------------------------------------------------------------------
    // Test Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    [DisplayName($"{nameof(SizeTests)}.{nameof(Builder)}")]
    public async Task Builder(CancellationToken ct = default) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.SetSize(Width, Height);

        // Assert
        await Assert.That(builder.Configuration.Width).IsEqualTo(Width);
        await Assert.That(builder.Configuration.Height).IsEqualTo(Height);

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToNativeParameters();
        await Assert.That(configParameters.Width).IsEqualTo(Width);
        await Assert.That(configParameters.Height).IsEqualTo(Height);
    }

    [Test]
    [DisplayName($"{nameof(SizeTests)}.{nameof(Builder_ShouldOverwriteOsDefaultSizeAndCentered)}")]
    public async Task Builder_ShouldOverwriteOsDefaultSizeAndCentered(CancellationToken ct = default) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        InfiniFrameNativeParameters expectedConfigParameters = new InfiniFrameOptionsBuilder {
            Width = Width,
            Height = Height,
            UseOsDefaultSize = false,
            Centered = false
        }.ToNativeParameters();

        // Act
        builder.SetUseOsDefaultSize(true);
        builder.SetSize(Width, Height);

        // Assert
        await Assert.That(builder.Configuration.Width).IsEqualTo(Width);
        await Assert.That(builder.Configuration.Height).IsEqualTo(Height);
        await Assert.That(builder.Configuration.UseOsDefaultSize).IsFalse();
        await Assert.That(builder.Configuration.Centered).IsFalse();

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToNativeParameters();
        await Assert.That(configParameters).IsEqualTo(expectedConfigParameters);
    }

    [Test]
    [DisplayName($"{nameof(SizeTests)}.{nameof(Window)}")]
    [SkipOnMacOs]
    [SkipOnLinux(SkipUtility.LinuxMovement)]
    [NotInParallelInfiniTests]
    public async Task Window(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetSize(400, 500);

        // Assert
        await Assert.That(window.Size).IsEqualTo(new Size(400, 500));
    }

    [Test]
    [DisplayName($"{nameof(SizeTests)}.{nameof(Window_AsSize)}")]
    [SkipOnMacOs]
    [SkipOnLinux(SkipUtility.LinuxMovement)]
    [NotInParallelInfiniTests]
    public async Task Window_AsSize(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetSize(new Size(400, 500));

        // Assert
        await Assert.That(window.Size).IsEqualTo(new Size(400, 500));
    }

    [Test]
    [DisplayName($"{nameof(SizeTests)}.{nameof(FullIntegration)}")]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    public async Task FullIntegration(CancellationToken ct = default) {
        // Arrange

        // Act
        using var windowUtility = InfiniFrameTestWindow.Create(
            builder: builder => builder
                .SetChromeless(true)
                .SetSize(400, 500),
            ct
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.Size).IsEqualTo(new Size(400, 500));
    }

    [Test]
    [DisplayName($"{nameof(SizeTests)}.{nameof(Window_WithChromelessToGetSmallestSize)}")]
    [SkipOnMacOs]
    [SkipOnLinux(SkipUtility.LinuxMovement)]
    [NotInParallelInfiniTests]
    public async Task Window_WithChromelessToGetSmallestSize(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(
            builder: builder => builder.SetChromeless(true),
            ct
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetSize(Width, Height);

        // Assert
        await Assert.That(window.Size).IsEqualTo(new Size(Width, Height));
    }

    [Test]
    [DisplayName($"{nameof(SizeTests)}.{nameof(FullIntegration_WithChromelessToGetSmallestSize)}")]
    [SkipOnMacOs]
    [NotInParallelInfiniTests]
    public async Task FullIntegration_WithChromelessToGetSmallestSize(CancellationToken ct = default) {
        // Arrange

        // Act
        using var windowUtility = InfiniFrameTestWindow.Create(
            builder: builder => builder
                .SetChromeless(true)
                .SetSize(Width, Height),
            ct
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.Size).IsEqualTo(new Size(Width, Height));
    }
}
