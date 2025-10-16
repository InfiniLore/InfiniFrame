// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.InfiniFrame.Native;
using InfiniLore.InfiniFrame.NET;
using System.Drawing;

namespace Tests.Photino.NET.WindowFunctionalities;
using Tests.Shared.Photino;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class SizeTests {
    private const int Width = 10;
    private const int Height = 20;

    [Test]
    public async Task Builder() {
        // Arrange
        var builder = PhotinoWindowBuilder.Create();

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
    public async Task Builder_ShouldOverwriteOsDefaultSizeAndCentered() {
        // Arrange
        var builder = PhotinoWindowBuilder.Create();
        InfiniFrameNativeParameters expectedConfigParameters = new PhotinoConfiguration {
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
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux(SkipUtility.LinuxMovement)]
    [NotInParallel(ParallelControl.Photino)]
    public async Task Window() {
        // Arrange
        using var windowUtility = WindowTestUtility.Create();
        IPhotinoWindow window = windowUtility.Window;

        // Act
        window.SetSize(400, 500);

        // Assert
        await Assert.That(window.Size).IsEqualTo(new Size(400, 500));
    }
    
    [Test]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux(SkipUtility.LinuxMovement)]
    [NotInParallel(ParallelControl.Photino)]
    public async Task Window_AsSize() {
        // Arrange
        using var windowUtility = WindowTestUtility.Create();
        IPhotinoWindow window = windowUtility.Window;

        // Act
        window.SetSize(new Size(400, 500));

        // Assert
        await Assert.That(window.Size).IsEqualTo(new Size(400, 500));
    }

    [Test]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.Photino)]
    public async Task FullIntegration() {
        // Arrange

        // Act
        using var windowUtility = WindowTestUtility.Create(
            builder => builder
                .SetChromeless(true)
                .SetSize(400, 500)
        );
        IPhotinoWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.Size).IsEqualTo(new Size(400, 500));
    }
    
    [Test]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux(SkipUtility.LinuxMovement)]
    [NotInParallel(ParallelControl.Photino)]
    public async Task Window_WithChromelessToGetSmallestSize() {
        // Arrange
        using var windowUtility = WindowTestUtility.Create(builder => builder.SetChromeless(true));
        IPhotinoWindow window = windowUtility.Window;

        // Act
        window.SetSize(Width, Height);

        // Assert
        await Assert.That(window.Size).IsEqualTo(new Size(Width, Height));
    }

    [Test]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.Photino)]
    public async Task FullIntegration_WithChromelessToGetSmallestSize() {
        // Arrange

        // Act
        using var windowUtility = WindowTestUtility.Create(
            builder => builder
                .SetChromeless(true)
                .SetSize(Width, Height)
        );
        IPhotinoWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.Size).IsEqualTo(new Size(Width, Height));
    }
}
