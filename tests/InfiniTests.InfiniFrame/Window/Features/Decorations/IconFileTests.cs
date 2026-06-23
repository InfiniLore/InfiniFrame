// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniTests.InfiniFrame.Window.Features.Decorations;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class IconFileTests {
    [Test]
    public async Task AtBuilderStage_DirectAssignment_ResolvesIconForNativeParameters(CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        string value = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.ico");

        // Act
        InfiniFrameNativeParameters initParameters;
        try {
            await File.WriteAllTextAsync(value, "icon", ct);
            builder.Features.Decorations.SetIconFile(value);
            initParameters = builder.CollectNativeParameters();
        }
        finally {
            File.Delete(value);
        }

        // Assert
        await Assert.That(builder.Features.Decorations.IconFilePath).IsEqualTo(value);
        await Assert.That(initParameters.WindowIconFile).IsEqualTo(value);
    }

    [Test]
    public async Task AtBuilderStage_ExtensionAssignment_InvalidPath_DoesNotPassIconToNativeParameters(CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();
        const string value = "missing.ico";

        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.SetIconFile(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Decorations.IconFilePath).IsEqualTo(value);
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
        await Assert.That(initParameters.WindowIconFile).IsNull();
    }

    [Test]
    [SkipOnMacOs]
    [SkipOnLinux]
    public async Task AtWindowStage_DirectAssignment_InvalidPath_DoesNotReplaceCurrentIcon(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        string? originalIcon = window.Features.Decorations.IconFilePath;
        const string invalidIconPath = "invalid.ico";

        // Act
        window.Features.Decorations.SetIconFile(invalidIconPath);
        string? iconAfterInvalidAssignment = window.Features.Decorations.IconFilePath;

        // Assert
        await Assert.That(iconAfterInvalidAssignment).IsEqualTo(originalIcon);
        await Assert.That(iconAfterInvalidAssignment).IsNotEqualTo(invalidIconPath);
    }

    [Test]
    [SkipOnMacOs]
    [SkipOnLinux]
    public async Task AtWindowStage_ExtensionAssignment_InvalidPath_ReturnsSameWindow(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        string? originalIcon = window.Features.Decorations.IconFilePath;
        const string invalidIconPath = "invalid.ico";

        // Act
        IInfiniFrameWindow returnedWindow = window.SetIconFile(invalidIconPath);
        string? iconAfterInvalidAssignment = window.Features.Decorations.IconFilePath;

        // Assert
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
        await Assert.That(iconAfterInvalidAssignment).IsEqualTo(originalIcon);
        await Assert.That(iconAfterInvalidAssignment).IsNotEqualTo(invalidIconPath);
    }
}
