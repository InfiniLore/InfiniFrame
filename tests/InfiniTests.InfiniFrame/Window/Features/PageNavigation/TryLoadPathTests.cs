// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.PageNavigation;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class TryLoadPathTests {
    [Test]
    [NotInParallelInfiniTests]
    [SkipOnLinux]
    public async Task AtWindowStage_ExistingFilePath_ReturnsTrue(CancellationToken ct) {
        // Arrange
        string tempFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.html");
        await File.WriteAllTextAsync(tempFilePath, "<html><body>try-load-path</body></html>", ct);
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        try {
            // Act
            bool loaded = window.Features.PageNavigation.TryLoadPath(tempFilePath);

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
    public async Task AtWindowStage_DisallowedAbsoluteUriString_ReturnsFalse(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        const string disallowedAbsoluteUri = "ftp://example.com/blocked";

        // Act
        bool loaded = window.Features.PageNavigation.TryLoadPath(disallowedAbsoluteUri);

        // Assert
        await Assert.That(loaded).IsFalse();
    }
}