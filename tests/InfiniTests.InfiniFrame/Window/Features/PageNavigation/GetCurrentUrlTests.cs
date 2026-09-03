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
        string? currentUrl = window.Features.PageNavigation.GetCurrentUrl();

        // Assert - a window loaded via StartString has no meaningful URL
        await Assert.That(currentUrl is null or "about:blank").IsTrue();
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
        string? currentUrl = window.Features.PageNavigation.GetCurrentUrl();
        await Assert.That(currentUrl is null or "about:blank").IsTrue();
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
        string? currentUrl = window.Features.PageNavigation.GetCurrentUrl();
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
        Uri? currentUri = window.Features.PageNavigation.GetCurrentUri();
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
        string? currentUrl = window.Features.PageNavigation.GetCurrentUrl();

        // Assert - fresh window has about:blank which is an absolute URI
        Uri? currentUri = window.Features.PageNavigation.GetCurrentUri();
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

        // Act - call the property multiple times to check for consistency,
        // since the window state may change between calls.
        string? viaProperty1 = window.Features.PageNavigation.GetCurrentUrl();
        string? viaProperty2 = window.Features.PageNavigation.GetCurrentUrl();
        string? viaExtension = window.GetCurrentUrl();

        // Assert - the extension delegates to the same property, so both should
        // agree on whether a URL exists at any given point in time.
        bool propertyHasUrl = !string.IsNullOrEmpty(viaProperty1);
        bool extensionHasUrl = !string.IsNullOrEmpty(viaExtension);
        await Assert.That(extensionHasUrl).IsEqualTo(propertyHasUrl);

        // A fresh window started via StartString has no meaningful URL
        if (propertyHasUrl) await Assert.That(viaProperty1).IsEqualTo("about:blank");
        if (extensionHasUrl) await Assert.That(viaExtension).IsEqualTo("about:blank");
    }
}
