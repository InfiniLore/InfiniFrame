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
    [Arguments("C:/temp/infiniframe-icon-a.ico")]
    [Arguments("C:/temp/infiniframe-icon-b.ico")]
    public async Task AtBuilderStage_DirectAssignment(string value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        builder.Features.Decorations.SetIconFile(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Decorations.IconFilePath).IsEqualTo(value);
        await Assert.That(initParameters.WindowIconFile).IsEqualTo(value);
    }

    [Test]
    [Arguments("C:/temp/infiniframe-icon-c.ico")]
    [Arguments("C:/temp/infiniframe-icon-d.ico")]
    public async Task AtBuilderStage_ExtensionAssignment(string value, CancellationToken ct) {
        // Arrange
        var builder = InfiniFrameWindowBuilder.Create();

        // Act
        IInfiniFrameWindowBuilder returnedBuilder = builder.SetIconFile(value);
        InfiniFrameNativeParameters initParameters = builder.CollectNativeParameters();

        // Assert
        await Assert.That(builder.Features.Decorations.IconFilePath).IsEqualTo(value);
        await Assert.That(returnedBuilder).IsSameReferenceAs(builder);
        await Assert.That(initParameters.WindowIconFile).IsEqualTo(value);
    }

    [Test]
    [SkipOnMacOs]
    [SkipOnLinux]
    [NotInParallelInfiniTests]
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
    [NotInParallelInfiniTests]
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
