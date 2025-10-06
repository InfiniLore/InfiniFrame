// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace Tests.InfiniLore.Photino.NET.Functionalities;
using global::InfiniLore.Photino.NET;
using Tests.InfiniLore.Photino.NET.TestUtilities;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class IconFilePathTests {
    private const string IconFilePath = "Assets/favicon.ico";
    
    [Test]
    public async Task Builder_ShouldSetIconFilePath() {
        // Arrange
        var builder = PhotinoWindowBuilder.Create();
        PhotinoNativeParameters expectedConfigParameters = new PhotinoConfiguration() {
            IconFilePath = IconFilePath
        }.ToParameters();
        
        // Act
        builder.SetIconFile(IconFilePath);

        // Assert
        await Assert.That(builder.Configuration.IconFilePath).IsEqualTo(IconFilePath);
        await Assert.That(builder.Configuration.ToParameters()).IsEqualTo(expectedConfigParameters);
    }

    [Test]
    [SkipUtility.OnMacOs]
    public async Task Window_ShouldSetIconFilePath() {
        // Arrange
        using var windowUtility = WindowTestUtility.Create();
        IPhotinoWindow window = windowUtility.Window;

        // Act
        window.SetIconFile(IconFilePath);

        // Assert
        await Assert.That(window.IconFilePath).IsEqualTo(IconFilePath);
    }
    
    [Test]
    [SkipUtility.OnMacOs]
    public async Task FullIntegration_ShouldSetIconFilePath() {
        // Arrange

        // Act
        using var windowUtility = WindowTestUtility.Create(
            builder => builder.SetIconFile(IconFilePath)
        );
        IPhotinoWindow window = windowUtility.Window;
        
        // Assert
        await Assert.That(window.IconFilePath).IsEqualTo(IconFilePath);
    }
}
