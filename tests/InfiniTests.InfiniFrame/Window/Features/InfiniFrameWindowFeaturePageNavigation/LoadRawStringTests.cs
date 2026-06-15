// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.InfiniFrameWindowFeaturePageNavigation;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class LoadRawStringTests {
    [Test]
    [NotInParallelInfiniTests]
    [SkipOnMacOs]
    [SkipOnLinux]
    public async Task AtWindowStage_DirectAssignment(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        const string html = "<html><body>load-raw-string-direct</body></html>";

        // Act
        window.Features.PageNavigation.LoadRawString(html);

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
        const string html = "<html><body>load-raw-string-extension</body></html>";

        // Act
        IInfiniFrameWindow returnedWindow = window.LoadRawString(html);

        // Assert
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
    }
}
