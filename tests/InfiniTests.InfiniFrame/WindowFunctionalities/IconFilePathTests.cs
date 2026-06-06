// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.WindowFunctionalities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class IconFilePathTests {
    private const string IconFilePath = "Assets/favicon.ico";
    private const string InvalidIconFilePath = "invalid.ico";
    private static readonly string ResolvedIconFilePath = Path.GetFullPath(IconFilePath, AppContext.BaseDirectory);

    // -----------------------------------------------------------------------------------------------------------------
    // Test Methods
    // -----------------------------------------------------------------------------------------------------------------
    [Test]
    [DisplayName($"{nameof(IconFilePathTests)}.{nameof(Builder)}")]
    public async Task Builder(CancellationToken ct = default) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.SetIconFile(IconFilePath);

        // Assert
        await Assert.That(builder.Configuration.IconFilePath).IsEqualTo(ResolvedIconFilePath);
        await Assert.That(builder.Configuration.ToNativeParameters().WindowIconFile).IsEqualTo(ResolvedIconFilePath);
    }

    [Test]
    [DisplayName($"{nameof(IconFilePathTests)}.{nameof(Builder_ShouldNotSetInvalidIconFilePath)}")]
    public async Task Builder_ShouldNotSetInvalidIconFilePath(CancellationToken ct = default) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.SetIconFile(InvalidIconFilePath);

        // Assert
        await Assert.That(builder.Configuration.IconFilePath).IsNull();
        await Assert.That(builder.Configuration.ToNativeParameters().WindowIconFile).IsNull();
    }

    [Test]
    [DisplayName($"{nameof(IconFilePathTests)}.{nameof(Window)}")]
    [SkipOnMacOs]
    [SkipOnLinux]
    [NotInParallelInfiniTests]
    public async Task Window(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetIconFile(IconFilePath);

        // Assert
        string? foundPath = window.IconFilePath;
        await Assert.That(foundPath).IsNotNull()
            .And.IsEqualTo(ResolvedIconFilePath);
    }

    [Test]
    [DisplayName($"{nameof(IconFilePathTests)}.{nameof(Window_ShouldNotSetInvalidIconFilePath)}")]
    [SkipOnMacOs]
    [SkipOnLinux]
    [NotInParallelInfiniTests]
    public async Task Window_ShouldNotSetInvalidIconFilePath(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.SetIconFile(InvalidIconFilePath);

        // Assert
        await Assert.That(window.IconFilePath).IsEqualTo(string.Empty);
    }

    [Test]
    [DisplayName($"{nameof(IconFilePathTests)}.{nameof(FullIntegration)}")]
    [SkipOnMacOs]
    [SkipOnLinux]
    [NotInParallelInfiniTests]
    public async Task FullIntegration(CancellationToken ct = default) {
        // Arrange

        // Act
        using var windowUtility = InfiniFrameTestWindow.Create(
            builder: builder => builder.SetIconFile(IconFilePath),
            ct
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Assert
        await Assert.That(window.IconFilePath).IsEqualTo(ResolvedIconFilePath);
    }
}
