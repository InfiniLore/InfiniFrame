// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.InfiniFrameWindowFeaturePageNavigation;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class LoadUriTests {
    [Test]
    [NotInParallelInfiniTests]
    [SkipOnMacOs]
    [SkipOnLinux]
    public async Task AtWindowStage_DirectAssignment(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        Uri uri = new("https://example.com/infini-frame-active-load-uri-direct");

        // Act
        window.Features.PageNavigation.Load(uri);

        // Assert
        await Assert.That(window.Features.PageNavigation).IsNotNull();
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnMacOs]
    [SkipOnLinux]
    public async Task AtWindowStage_ExtensionAssignment(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        Uri uri = new("https://example.com/infini-frame-active-load-uri-extension");

        // Act
        IInfiniFrameWindow returnedWindow = window.Load(uri);

        // Assert
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
    }
}
