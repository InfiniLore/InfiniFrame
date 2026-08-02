// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.PageNavigation;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class GetCurrentUrlTests {
    [Test]
    [NotInParallelInfiniTests]
    [SkipOnLinux]
    public async Task FreshWindow_WithStartString_ReturnsNullOrAboutBlank(CancellationToken ct) {
        // Arrange - test window starts with StartString (HTML content, no URL)
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        string? currentUrl = window.Features.PageNavigation.CurrentUrl;

        // Assert - a window loaded via StartString has no meaningful URL
        await Assert.That(currentUrl == null || currentUrl == "about:blank").IsTrue();
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnLinux]
    public async Task AfterLoadRawString_ReturnsNullOrAboutBlank(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Features.PageNavigation.LoadRawString("<html><body>raw content</body></html>");

        // Assert - macOS WKWebView assigns "about:blank" to raw string content
        string? currentUrl = window.Features.PageNavigation.CurrentUrl;
        await Assert.That(currentUrl == null || currentUrl == "about:blank").IsTrue();
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnLinux]
    public async Task AfterClose_ReturnsDefault(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Close();

        // Assert - should not throw
        string? currentUrl = window.Features.PageNavigation.CurrentUrl;
        await Assert.That(currentUrl).IsNull();
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnLinux]
    public async Task CurrentUri_WhenUrlIsNull_ReturnsNullOrAboutBlank(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Features.PageNavigation.LoadRawString("<html><body>raw</body></html>");

        // Assert - macOS WKWebView assigns "about:blank" to raw string content
        Uri? currentUri = window.Features.PageNavigation.CurrentUri;
        await Assert.That(currentUri == null || currentUri.ToString() == "about:blank").IsTrue();
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnLinux]
    public async Task CurrentUri_WhenUrlIsAbsolute_ParsesCorrectly(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        string? currentUrl = window.Features.PageNavigation.CurrentUrl;

        // Assert - fresh window has about:blank which is an absolute URI
        Uri? currentUri = window.Features.PageNavigation.CurrentUri;
        if (currentUrl != null) {
            await Assert.That(currentUri).IsNotNull();
            await Assert.That(currentUri!.IsAbsoluteUri).IsTrue();
        }
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnLinux]
    public async Task ExtensionGetCurrentUrl_ReturnsSameAsProperty(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        string? viaProperty = window.Features.PageNavigation.CurrentUrl;
        string? viaExtension = window.GetCurrentUrl();

        // Assert
        await Assert.That(viaExtension).IsEqualTo(viaProperty);
    }
}
