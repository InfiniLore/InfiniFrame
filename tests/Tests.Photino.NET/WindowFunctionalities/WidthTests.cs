// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.Photino.NET;

namespace Tests.Photino.NET.WindowFunctionalities;
using Tests.Shared.Photino;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WidthTests {
    private const int Width = 20;

    [Test]
    public async Task Builder() {
        // Arrange
        var builder = PhotinoWindowBuilder.Create();

        // Act
        builder.SetUseOsDefaultSize(true);
        builder.SetWidth(Width);

        // Assert
        await Assert.That(builder.Configuration.Width).IsEqualTo(Width);

        PhotinoNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters.Width).IsEqualTo(Width);
    }

    [Test]
    public async Task Builder_ShouldOverwriteOsDefaultSizeAndCentered() {
        // Arrange
        var builder = PhotinoWindowBuilder.Create();
        PhotinoNativeParameters expectedConfigParameters = new PhotinoConfiguration {
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

        PhotinoNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters).IsEqualTo(expectedConfigParameters);
    }
    
    [Test]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux(SkipUtility.LinuxMovement)]
    [NotInParallel(ParallelControl.Photino)]
    public async Task Window() {
        // Arrange
        using var windowUtility = WindowTestUtility.Create();
        IPhotinoWindow window = windowUtility.Window;

        // Act
        window.SetWidth(500);

        // Assert
        await Assert.That(window.Width).IsEqualTo(500);
    }

    [Test]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux(SkipUtility.LinuxMovement)]
    [NotInParallel(ParallelControl.Photino)]
    public async Task FullIntegration() {
        // Arrange

        // Act
        using var windowUtility = WindowTestUtility.Create(
            builder => builder
                .SetChromeless(true)
                .SetWidth(500)
        );
        IPhotinoWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.Width).IsEqualTo(500);
    }
    
    [Test]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux(SkipUtility.LinuxMovement)]
    [NotInParallel(ParallelControl.Photino)]
    public async Task Window_WithChromelessToGetSmallestWidth() {
        // Arrange
        using var windowUtility = WindowTestUtility.Create(builder => builder.SetChromeless(true));
        IPhotinoWindow window = windowUtility.Window;

        // Act
        window.SetWidth(Width);

        // Assert
        await Assert.That(window.Width).IsEqualTo(Width);
    }

    [Test]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.Photino)]
    public async Task FullIntegration_WithChromelessToGetSmallestWidth() {
        // Arrange

        // Act
        using var windowUtility = WindowTestUtility.Create(
            builder => builder
                .SetChromeless(true)
                .SetWidth(Width)
        );
        IPhotinoWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.Width).IsEqualTo(Width);
    }
}
