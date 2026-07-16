// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.PageNavigation;
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

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnMacOs]
    [SkipOnLinux]
    public async Task AtWindowStage_DirectAssignment_DisallowedAbsoluteUriString_DoesNotThrow(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        const string disallowedAbsoluteUri = "ftp://example.com/not-allowed";

        // Act
        window.Features.PageNavigation.Load(disallowedAbsoluteUri);

        // Assert
        await Assert.That(window.IsClosedOrClosing()).IsFalse();
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnMacOs]
    [SkipOnLinux]
    public async Task AtWindowStage_ExtensionAssignment_DisallowedAbsoluteUriString_DoesNotThrow(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        const string disallowedAbsoluteUri = "ftp://example.com/not-allowed-extension";

        // Act
        IInfiniFrameWindow returnedWindow = window.Load(disallowedAbsoluteUri);

        // Assert
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
        await Assert.That(window.IsClosedOrClosing()).IsFalse();
    }
}
