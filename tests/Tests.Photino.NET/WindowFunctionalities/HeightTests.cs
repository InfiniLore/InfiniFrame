// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.InfiniFrame.Native;
using InfiniLore.InfiniFrame.NET;

namespace Tests.Photino.NET.WindowFunctionalities;
using Tests.Shared.Photino;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class HeightTests {
    private const int Height = 20;

    [Test]
    public async Task Builder() {
        // Arrange
        var builder = PhotinoWindowBuilder.Create();

        // Act
        builder.SetUseOsDefaultSize(true);
        builder.SetHeight(Height);

        // Assert
        await Assert.That(builder.Configuration.Height).IsEqualTo(Height);

        InfiniFrameNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters.Height).IsEqualTo(Height);
    }

    [Test]
    [NotInParallel(ParallelControl.Photino)]
    public async Task Builder_ShouldOverwriteOsDefaultSizeAndCentered() {
        // Arrange
        var builder = PhotinoWindowBuilder.Create();
        InfiniFrameNativeParameters expectedConfigParameters = new PhotinoConfiguration {
            Height = Height,
            UseOsDefaultSize = false,
            Centered = false
        }.ToParameters();

        // Act
        builder.SetUseOsDefaultSize(true);
        builder.SetHeight(Height);

        // Assert
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
        window.SetHeight(500);

        // Assert
        await Assert.That(window.Height).IsEqualTo(500);
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
                .SetHeight(500)
        );
        IPhotinoWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.Height).IsEqualTo(500);
    }
    
    [Test]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux(SkipUtility.LinuxMovement)]
    [NotInParallel(ParallelControl.Photino)]
    public async Task Window_WithChromelessToGetSmallestHeight() {
        // Arrange
        using var windowUtility = WindowTestUtility.Create(builder => builder.SetChromeless(true));
        IPhotinoWindow window = windowUtility.Window;

        // Act
        window.SetHeight(Height);

        // Assert
        await Assert.That(window.Height).IsEqualTo(Height);
    }

    [Test]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.Photino)]
    public async Task FullIntegration_WithChromelessToGetSmallestHeight() {
        // Arrange

        // Act
        using var windowUtility = WindowTestUtility.Create(
            builder => builder
                .SetChromeless(true)
                .SetHeight(Height)
        );
        IPhotinoWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.Height).IsEqualTo(Height);
    }
}
