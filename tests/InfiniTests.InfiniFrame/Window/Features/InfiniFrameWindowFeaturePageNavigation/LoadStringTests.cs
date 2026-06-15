// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.InfiniFrameWindowFeaturePageNavigation;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class LoadStringTests {
    [Test]
    [NotInParallelInfiniTests]
    [SkipOnMacOs]
    [SkipOnLinux]
    public async Task AtWindowStage_DirectAssignment(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        const string url = "https://example.com/infini-frame-active-load-string-direct";

        // Act
        window.Features.PageNavigation.Load(url);

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
        const string url = "https://example.com/infini-frame-active-load-string-extension";

        // Act
        IInfiniFrameWindow returnedWindow = window.Load(url);

        // Assert
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
    }
}
