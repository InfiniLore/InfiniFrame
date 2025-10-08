// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.Photino.NET;
using Tests.Photino.NET.TestUtilities;

namespace Tests.Photino.NET.WindowFunctionalities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class LeftTests {
    private const int Left = 20;

    [Test]
    public async Task Builder() {
        // Arrange
        var builder = PhotinoWindowBuilder.Create();

        // Act
        builder.SetLeft(Left);

        // Assert
        await Assert.That(builder.Configuration.Left).IsEqualTo(Left);

        PhotinoNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters.Left).IsEqualTo(Left);
    }

    [Test]
    [NotInParallel(ParallelControl.Photino)]
    public async Task Builder_ShouldOverwriteOsDefaultLocationAndCentered() {
        // Arrange
        var builder = PhotinoWindowBuilder.Create();
        PhotinoNativeParameters expectedConfigParameters = new PhotinoConfiguration {
            Left = Left,
            UseOsDefaultLocation = false,
            Centered = false
        }.ToParameters();

        // Act
        builder.Center();
        builder.SetUseOsDefaultLocation(true);
        builder.SetLeft(Left);

        // Assert
        await Assert.That(builder.Configuration.Left).IsEqualTo(Left);
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
        window.SetLeft(Left);

        // Assert
        await Assert.That(window.Left).IsEqualTo(Left);
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
                .SetLeft(Left)
        );
        IPhotinoWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.Left).IsEqualTo(Left);
    }

}
