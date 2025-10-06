// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace Tests.InfiniLore.Photino.NET.Functionalities;
using global::InfiniLore.Photino.NET;
using System.Drawing;
using Tests.InfiniLore.Photino.NET.TestUtilities;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class SizeTests {
    private const int Width = 10;
    private const int Height = 20;
    
    [Test]
    public async Task Builder_ShouldSetSize() {
        // Arrange
        
        var builder = PhotinoWindowBuilder.Create();
        PhotinoNativeParameters expectedConfigParameters = new PhotinoConfiguration() {
            Width = Width,
            Height = Height,
        }.ToParameters();
        
        // Act
        builder.SetUseOsDefaultSize(true);
        builder.SetSize(Width, Height);
        
        // Assert
        await Assert.That(builder.Configuration.Width).IsEqualTo(Width);
        await Assert.That(builder.Configuration.Height).IsEqualTo(Height);
        
        PhotinoNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters).IsEqualTo(expectedConfigParameters);
    }
    
    [Test]
    public async Task Builder_ShouldOverwriteOsDefaultSizeAndCentered() {
        // Arrange
        
        var builder = PhotinoWindowBuilder.Create();
        PhotinoNativeParameters expectedConfigParameters = new PhotinoConfiguration {
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
        
        PhotinoNativeParameters configParameters = builder.Configuration.ToParameters();
        await Assert.That(configParameters).IsEqualTo(expectedConfigParameters);
    }
    

    [Test]
    [SkipUtility.OnMacOs]
    public async Task Window_ShouldSetSize() {
        
        // Arrange
        using var windowUtility = WindowTestUtility.Create();
        IPhotinoWindow window = windowUtility.Window;

        // Act
        window.SetSize(Width, Height);

        // Assert
        await Assert.That(window.Size).IsEqualTo(new Size(Width, Height));
    }
    
    [Test]
    [SkipUtility.OnMacOs]
    public async Task FullIntegration_ShouldSetSize() {
        // Arrange

        // Act
        using var windowUtility = WindowTestUtility.Create(
            builder => builder.SetSize(Width, Height)
        );
        IPhotinoWindow window = windowUtility.Window;
        
        // Assert
        await Assert.That(window.Size).IsEqualTo(new Size(Width, Height));
    }
}
