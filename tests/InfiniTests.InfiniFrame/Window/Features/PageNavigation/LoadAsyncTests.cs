// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.PageNavigation;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class LoadAsyncTests {
    [Test]
    [NotInParallelInfiniTests]
    [SkipOnLinux]
    public async Task LoadAsync_WithUri_ReturnsNavigationResult(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        Uri uri = new("https://example.com/infini-frame-async-load-uri");

        // Act
        NavigationResult result = await window.Features.PageNavigation.LoadAsync(uri, ct);

        // Assert
        await Assert.That(result).IsNotNull();
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnLinux]
    public async Task LoadAsync_WithStringUrl_ReturnsNavigationResult(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        const string url = "https://example.com/infini-frame-async-load-string-url";

        // Act
        NavigationResult result = await window.Features.PageNavigation.LoadAsync(url, ct);

        // Assert
        await Assert.That(result).IsNotNull();
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnLinux]
    public async Task LoadAsync_WithStringPath_ReturnsNavigationResult(CancellationToken ct) {
        // Arrange
        string tempFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.html");
        await File.WriteAllTextAsync(tempFilePath, "<html><body>async-file</body></html>", ct);
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        try {
            // Act
            NavigationResult result = await window.Features.PageNavigation.LoadAsync(tempFilePath, ct);

            // Assert
            await Assert.That(result).IsNotNull();
        }
        finally {
            try {
                File.Delete(tempFilePath);
            }
            catch {
                // ignored
            }
        }
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnLinux]
    public async Task LoadAsync_ExtensionWithUri_ReturnsNavigationResult(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        Uri uri = new("https://example.com/infini-frame-async-load-extension-uri");

        // Act
        NavigationResult result = await window.LoadAsync(uri, ct);

        // Assert
        await Assert.That(result).IsNotNull();
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnLinux]
    public async Task LoadAsync_ExtensionWithString_ReturnsNavigationResult(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        const string url = "https://example.com/infini-frame-async-load-extension-string";

        // Act
        NavigationResult result = await window.LoadAsync(url, ct);

        // Assert
        await Assert.That(result).IsNotNull();
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnLinux]
    public async Task LoadAsync_NullUri_ThrowsArgumentNull(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act & Assert
        await Assert.That(async () => await window.Features.PageNavigation.LoadAsync((Uri)null!, ct))
            .ThrowsExactly<ArgumentNullException>();
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnLinux]
    public async Task LoadAsync_NullString_ThrowsArgumentNull(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act & Assert
        await Assert.That(async () => await window.Features.PageNavigation.LoadAsync((string)null!, ct))
            .ThrowsExactly<ArgumentNullException>();
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnLinux]
    public async Task LoadAsync_AfterClose_DoesNotThrow(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        Uri uri = new("https://example.com/async-after-close-no-op", UriKind.Absolute);

        // Act
        window.Close();
        await EnsureWindowClosed(window, ct);
        NavigationResult _ = await window.Features.PageNavigation.LoadAsync(uri, ct);

        // Assert
        await Assert.That(window.IsClosedOrClosing()).IsTrue();
    }

    private static async Task EnsureWindowClosed(IInfiniFrameWindow window, CancellationToken ct) {
        DateTime timeoutAt = DateTime.UtcNow.AddSeconds(5);
        while (!window.IsClosedOrClosing() && DateTime.UtcNow < timeoutAt && !ct.IsCancellationRequested) {
            await Task.Delay(50, ct);
        }
    }
}
