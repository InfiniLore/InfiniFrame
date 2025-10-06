// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using global::InfiniLore.Photino.NET;

namespace Tests.InfiniLore.Photino.NET;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowBuilderTests {
    // -----------------------------------------------------------------------------------------------------------------
    // Tests
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    public async Task SetLocation_ShouldOverwriteOsDefaultLocation() {
        // Arrange
        var builder = PhotinoWindowBuilder.Create();
        var expectedConfigParameters = new PhotinoNativeParameters() {
            Left = 10,
            Top = 20,
            UseOsDefaultLocation = false,
        };
        
        // Act
        builder.SetUseOsDefaultLocation(true);
        builder.SetLocation(10, 20);
        
        PhotinoNativeParameters configParameters = builder.Configuration.ToParameters();

        // Assert
        await Assert.That(builder.Configuration.Left).IsEqualTo(10);
        await Assert.That(builder.Configuration.Top).IsEqualTo(20);
        await Assert.That(builder.Configuration.UseOsDefaultLocation).IsEqualTo(false);
        
        await Assert.That(configParameters).IsEquivalentTo(expectedConfigParameters);
        
    }
    
    [Test]
    public async Task SetSize_ShouldOverwriteOsDefaultSize() {
        // Arrange
        var builder = PhotinoWindowBuilder.Create();
        var expectedConfigParameters = new PhotinoNativeParameters() {
            Width = 10,
            Height = 10,
            UseOsDefaultSize = false,
        };
        
        // Act
        builder.SetUseOsDefaultSize(true);
        builder.SetSize(10, 20);
        
        PhotinoNativeParameters configParameters = builder.Configuration.ToParameters();

        // Assert
        await Assert.That(builder.Configuration.Width).IsEqualTo(10);
        await Assert.That(builder.Configuration.Height).IsEqualTo(20);
        await Assert.That(builder.Configuration.UseOsDefaultSize).IsEqualTo(false);
        
        await Assert.That(configParameters).IsEquivalentTo(expectedConfigParameters);
    }
}
