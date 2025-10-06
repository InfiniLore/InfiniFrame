// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace Tests.InfiniLore.Photino.NET;
using global::InfiniLore.Photino.NET;
using Tests.InfiniLore.Photino.NET.TestUtilities;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowIntegrationTests {
    [Test]
    public async Task IconFilePath() {
        // Arrange
        const string iconFilePath = "Assets/favicon.ico";

        // Act
        using var windowUtility = WindowTestUtility.Create(
            builder => builder.SetIconFile(iconFilePath)
        );
        IPhotinoWindow window = windowUtility.Window;
        
        // Assert
        await Assert.That(window.IconFilePath).IsEqualTo(iconFilePath);
    }
}
