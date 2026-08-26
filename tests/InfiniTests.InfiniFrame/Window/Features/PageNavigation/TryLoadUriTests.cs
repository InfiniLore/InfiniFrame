// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.PageNavigation;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class TryLoadUriTests {
    [Test]
    [NotInParallelInfiniTests]
    [SkipOnLinux]
    public async Task AtWindowStage_FileUri_ReturnsTrue(CancellationToken ct) {
        // Arrange
        string tempFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.html");
        await File.WriteAllTextAsync(tempFilePath, "<html><body>try-load-uri</body></html>", ct);
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        Uri uri = new(tempFilePath);

        try {
            // Act
            bool loaded = window.Features.PageNavigation.TryLoadUri(uri);

            // Assert
            await Assert.That(loaded).IsTrue();
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
    public async Task AtWindowStage_DisallowedScheme_ReturnsFalse(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        Uri uri = new("ftp://example.com/blocked");

        // Act
        bool loaded = window.Features.PageNavigation.TryLoadUri(uri);

        // Assert
        await Assert.That(loaded).IsFalse();
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnLinux]
    public async Task AtWindowStage_NavigationStartingCancel_ReturnsFalse(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        window.RegisterNavigationStartingHandler((_, _) => NavigationStartingResult.Cancel);
        Uri uri = new("https://example.com");

        // Act
        bool loaded = window.Features.PageNavigation.TryLoadUri(uri);

        // Assert
        await Assert.That(loaded).IsFalse();
    }
}
