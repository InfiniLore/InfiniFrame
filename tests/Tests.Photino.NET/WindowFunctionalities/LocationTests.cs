// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.Photino.NET;
using System.Drawing;
using Tests.Photino.NET.TestUtilities;

namespace Tests.Photino.NET.WindowFunctionalities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class LocationTests {
    private const int Left = 10;
    private const int Top = 20;
    
    [Test]
    public async Task Builder() {
        // Arrange
        var builder = PhotinoWindowBuilder.Create();
        
        // Act
        builder.SetUseOsDefaultLocation(true);
        builder.SetLocation(Left, Top);
        
        // Assert
        await Assert.That(builder.Configuration.Left).IsEqualTo(Left);
        await Assert.That(builder.Configuration.Top).IsEqualTo(Top);
        
        PhotinoNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters.Left).IsEqualTo(Left);
        await Assert.That(configParameters.Top).IsEqualTo(Top);
    }
    
    [Test]
    public async Task Builder_ShouldOverwriteOsDefaultLocationAndCentered() {
        // Arrange
        var builder = PhotinoWindowBuilder.Create();
        PhotinoNativeParameters expectedConfigParameters = new PhotinoConfiguration {
            Left = Left,
            Top = Top,
            UseOsDefaultLocation = false,
            Centered = false
        }.ToParameters();
        
        // Act
        builder.SetUseOsDefaultLocation(true);
        builder.SetLocation(Left, Top);
        
        // Assert
        await Assert.That(builder.Configuration.Left).IsEqualTo(Left);
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
        window.SetLocation(Left, Top);

        // Assert
        await Assert.That(window.Location).IsEqualTo(new Point(Left, Top));
    }
    
    [Test]
    [SkipUtility.OnMacOs]
    [SkipUtility.OnLinux(SkipUtility.LinuxMovement)]
    [NotInParallel(ParallelControl.Photino)]
    public async Task Window_AsPoint() {
        // Arrange
        using var windowUtility = WindowTestUtility.Create();
        IPhotinoWindow window = windowUtility.Window;

        // Act
        window.SetLocation(new Point(Left, Top));

        // Assert
        await Assert.That(window.Location).IsEqualTo(new Point(Left, Top));
    }
    
    [Test]
    [SkipUtility.OnMacOs]
    public async Task FullIntegration() {
        // Arrange

        // Act
        using var windowUtility = WindowTestUtility.Create(
            builder => builder.SetLocation(Left, Top)
        );
        IPhotinoWindow window = windowUtility.Window;
        
        // Assert
        await Assert.That(window.Location).IsEqualTo(new Point(Left, Top));
    }
}
