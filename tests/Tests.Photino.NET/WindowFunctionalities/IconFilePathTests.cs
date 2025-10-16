// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.InfiniFrame.NET;

namespace Tests.Photino.NET.WindowFunctionalities;
using InfiniLore.Photino.Native;
using Tests.Shared.Photino;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class IconFilePathTests {
    private const string IconFilePath = "Assets/favicon.ico";
    private const string InvalidIconFilePath = "invalid.ico";
    
    [Test]
    public async Task Builder() {
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
    public async Task Builder_ShouldNotSetInvalidIconFilePath() {
        // Arrange
        var builder = PhotinoWindowBuilder.Create();
        PhotinoNativeParameters expectedConfigParameters = new PhotinoConfiguration().ToParameters();
        
        // Act
        builder.SetIconFile(InvalidIconFilePath);

        // Assert
        await Assert.That(builder.Configuration.IconFilePath).IsEqualTo(null);
        await Assert.That(builder.Configuration.ToParameters()).IsEqualTo(expectedConfigParameters);
    }

    [Test]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.Photino)]
    public async Task Window() {
        // Arrange
        using var windowUtility = WindowTestUtility.Create();
        IPhotinoWindow window = windowUtility.Window;

        // Act
        window.SetIconFile(IconFilePath);

        // Assert
        await Assert.That(window.IconFilePath).IsEqualTo(IconFilePath);
    }

    [Test]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.Photino)]
    public async Task Window_ShouldNotSetInvalidIconFilePath() {
        // Arrange
        using var windowUtility = WindowTestUtility.Create();
        IPhotinoWindow window = windowUtility.Window;
        
        // Act
        window.SetIconFile(InvalidIconFilePath);

        // Assert
        await Assert.That(window.IconFilePath).IsEqualTo(null);
    }
    
    [Test]
    [SkipUtility.SkipOnMacOs]
    [NotInParallel(ParallelControl.Photino)]
    public async Task FullIntegration() {
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
