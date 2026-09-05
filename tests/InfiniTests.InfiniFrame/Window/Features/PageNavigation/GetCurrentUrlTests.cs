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

        // Act - call the property and extension back-to-back, retrying until
        // the window state stabilises.  WebView2 controller initialisation on
        // slower runners (e.g. Windows ARM64) can cause the URL to flip
        // between "about:blank" and null across consecutive calls.
        const int maxAttempts = 10;
        for (int attempt = 0; attempt < maxAttempts; attempt++) {
            string? viaProperty = window.Features.PageNavigation.GetCurrentUrl();
            string? viaExtension = window.GetCurrentUrl();

            bool propertyHasUrl = !string.IsNullOrEmpty(viaProperty);
            bool extensionHasUrl = !string.IsNullOrEmpty(viaExtension);

            if (propertyHasUrl == extensionHasUrl) {
                // Both agree — verify the actual values too
                if (propertyHasUrl) {
                    await Assert.That(viaProperty).IsEqualTo("about:blank");
                    await Assert.That(viaExtension).IsEqualTo("about:blank");
                }
                return;
            }

            await Task.Delay(100, ct);
        }

        // If we exhausted retries, the values still disagree — fail with a clear message
        string? finalProperty = window.Features.PageNavigation.GetCurrentUrl();
        string? finalExtension = window.GetCurrentUrl();
        await Assert.That(!string.IsNullOrEmpty(finalExtension))
            .IsEqualTo(!string.IsNullOrEmpty(finalProperty));
    }
}
