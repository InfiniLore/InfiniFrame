// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.InfiniFrame.Native;
using InfiniLore.InfiniFrame;

namespace Tests.InfiniFrame.WindowFunctionalities;
using Tests.Shared;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class IconFilePathTests {
    private const string IconFilePath = "Assets/favicon.ico";
    private const string InvalidIconFilePath = "invalid.ico";

    [Test]
    [DisplayName($"{nameof(IconFilePathTests)}.{nameof(Builder)}")]
    public async Task Builder() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        InfiniFrameNativeParameters expectedConfigParameters = new InfiniFrameWindowConfiguration {
            IconFilePath = IconFilePath
        }.ToParameters();

        // Act
        builder.SetIconFile(IconFilePath);

        // Assert
        await Assert.That(builder.Configuration.IconFilePath).IsEqualTo(IconFilePath);
        await Assert.That(builder.Configuration.ToParameters()).IsEqualTo(expectedConfigParameters);
    }

    [Test]
    [DisplayName($"{nameof(IconFilePathTests)}.{nameof(Builder_ShouldNotSetInvalidIconFilePath)}")]
    public async Task Builder_ShouldNotSetInvalidIconFilePath() {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        InfiniFrameNativeParameters expectedConfigParameters = new InfiniFrameWindowConfiguration().ToParameters();

        // Act
        builder.SetIconFile(InvalidIconFilePath);

        // Assert
        await Assert.That(builder.Configuration.IconFilePath).IsEqualTo(string.Empty);
        await Assert.That(builder.Configuration.ToParameters()).IsEqualTo(expectedConfigParameters);
    }

    [Test]
    [DisplayName($"{nameof(IconFilePathTests)}.{nameof(Window)}")]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux]
    [NotInParallel(ParallelControl.InfiniFrame)]
    public async Task Window() {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create();
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetIconFile(IconFilePath);

        // Assert
        string foundPath = window.IconFilePath;
        await Assert.That(foundPath).IsEqualTo(IconFilePath);
    }

    [Test]
    [DisplayName($"{nameof(IconFilePathTests)}.{nameof(Window_ShouldNotSetInvalidIconFilePath)}")]   
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux]
    [NotInParallel(ParallelControl.InfiniFrame)]
    public async Task Window_ShouldNotSetInvalidIconFilePath() {
        // Arrange
        using var windowUtility = InfiniFrameWindowTestUtility.Create();
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetIconFile(InvalidIconFilePath);

        // Assert
        await Assert.That(window.IconFilePath).IsEqualTo(string.Empty);
    }

    [Test]
    [DisplayName($"{nameof(IconFilePathTests)}.{nameof(FullIntegration)}")] 
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux]
    [NotInParallel(ParallelControl.InfiniFrame)]
    public async Task FullIntegration() {
        // Arrange

        // Act
        using var windowUtility = InfiniFrameWindowTestUtility.Create(
            builder => builder.SetIconFile(IconFilePath)
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.IconFilePath).IsEqualTo(IconFilePath);
    }
}
