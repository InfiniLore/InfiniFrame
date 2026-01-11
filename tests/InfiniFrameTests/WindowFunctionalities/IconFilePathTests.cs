// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Native;
using InfiniFrameTests.Shared;

namespace InfiniFrameTests.WindowFunctionalities;
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
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(Timeout.Seconds10)]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux]
    public async Task Window(CancellationToken timeoutToken) {
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
    [NotInParallel(ParallelControl.InfiniFrame)]  
    [Timeout(Timeout.Seconds10)]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux]
    public async Task Window_ShouldNotSetInvalidIconFilePath(CancellationToken timeoutToken) {
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
    [NotInParallel(ParallelControl.InfiniFrame)]
    [Timeout(Timeout.Seconds10)]
    [SkipUtility.SkipOnMacOs]
    [SkipUtility.SkipOnLinux]
    public async Task FullIntegration(CancellationToken timeoutToken) {
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
