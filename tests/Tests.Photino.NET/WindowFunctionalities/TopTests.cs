// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.Photino.NET;
using Tests.Photino.NET.TestUtilities;

namespace Tests.Photino.NET.WindowFunctionalities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class TopTests {
    private const int Top = 20;

    [Test]
    public async Task Builder() {
        // Arrange
        var builder = PhotinoWindowBuilder.Create();

        // Act
        builder.SetTop(Top);

        // Assert
        await Assert.That(builder.Configuration.Top).IsEqualTo(Top);

        PhotinoNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters.Top).IsEqualTo(Top);
    }

    [Test]
    [NotInParallel(ParallelControl.Photino)]
    public async Task Builder_ShouldOverwriteOsDefaultLocationAndCentered() {
        // Arrange
        var builder = PhotinoWindowBuilder.Create();
        PhotinoNativeParameters expectedConfigParameters = new PhotinoConfiguration {
            Top = Top,
            UseOsDefaultLocation = false,
            Centered = false
        }.ToParameters();

        // Act
        builder.Center();
        builder.SetUseOsDefaultLocation(true);
        builder.SetTop(Top);

        // Assert
        await Assert.That(builder.Configuration.Top).IsEqualTo(Top);
        await Assert.That(builder.Configuration.UseOsDefaultLocation).IsEqualTo(false);
        await Assert.That(builder.Configuration.Centered).IsEqualTo(false);

        PhotinoNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters).IsEqualTo(expectedConfigParameters);
    }
    
    [Test]
    [SkipUtility.OnMacOs]
    [SkipUtility.OnLinux(SkipUtility.LinuxMovement)]
    [NotInParallel(ParallelControl.Photino)]
    public async Task Window() {
        // Arrange
        using var windowUtility = WindowTestUtility.Create();
        IPhotinoWindow window = windowUtility.Window;

        // Act
        window.SetTop(Top);

        // Assert
        await Assert.That(window.Top).IsEqualTo(Top);
    }

    [Test]
    [SkipUtility.OnMacOs]
    [SkipUtility.OnLinux(SkipUtility.LinuxMovement)]
    [NotInParallel(ParallelControl.Photino)]
    public async Task FullIntegration() {
        // Arrange

        // Act
        using var windowUtility = WindowTestUtility.Create(
            builder => builder
                .SetTop(Top)
        );
        IPhotinoWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.Top).IsEqualTo(Top);
    }

}
