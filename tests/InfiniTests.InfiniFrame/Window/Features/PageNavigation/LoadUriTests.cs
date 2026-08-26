// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.PageNavigation;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class LoadUriTests {
    [Test]
    [NotInParallelInfiniTests]
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

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnLinux]
    public async Task AtWindowStage_DirectAssignment_FileUri(CancellationToken ct) {
        // Arrange
        string tempFilePath = Path.Join(Path.GetTempPath(), $"{Guid.NewGuid():N}.html");
        await File.WriteAllTextAsync(tempFilePath, "<html><body>file-uri</body></html>", ct);
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        Uri uri = new(tempFilePath);

        try {
            // Act
            window.Features.PageNavigation.Load(uri);

            // Assert
            await Assert.That(window.Features.PageNavigation).IsNotNull();
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
    public async Task AtWindowStage_DirectAssignment_DisallowedSchemeUri_DoesNotThrow(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        Uri uri = new("ftp://example.com/not-allowed-uri");

        // Act
        window.Features.PageNavigation.Load(uri);

        // Assert
        await Assert.That(window.IsClosedOrClosing()).IsFalse();
    }

    [Test]
    [NotInParallelInfiniTests]
    [DefaultInfiniTestsTimeout(DefaultInfiniTestsTimeoutAttribute.TimeoutValue + 5_000)]
    public async Task AtWindowStage_AfterClose_DoesNotThrowAndNoOps(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        Uri uri = new("https://example.com/after-close-no-op", UriKind.Absolute);

        // Act
        window.Close();
        await EnsureWindowClosed(window, ct);
        window.Features.PageNavigation.Load(uri);
        window.Load(uri);

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
