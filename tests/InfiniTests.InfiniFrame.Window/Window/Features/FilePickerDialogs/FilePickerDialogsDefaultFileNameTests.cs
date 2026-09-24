// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.FilePickerDialogs;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class FilePickerDialogsDefaultFileNameTests {
    [Test]
    [NotInParallelInfiniTests]
    public async Task ShowSaveFile_WithDefaultFileName_ShouldAcceptParameter(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        await EnsureWindowClosed(window, ct);

        // Act
        string? result = window.Features.FilePickerDialogs.ShowSaveFile(defaultFileName: "document.txt");

        // Assert
        await Assert.That(result).IsNull();
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task ShowSaveFile_Extension_WithDefaultFileName_ShouldAcceptParameter(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        await EnsureWindowClosed(window, ct);

        // Act
        string? result = window.ShowSaveFile(defaultFileName: "document.txt");

        // Assert
        await Assert.That(result).IsNull();
    }

    private static async Task EnsureWindowClosed(IInfiniFrameWindow window, CancellationToken ct) {
        window.Close();
        DateTime timeoutAt = DateTime.UtcNow.AddSeconds(5);
        while (!window.IsClosedOrClosing() && DateTime.UtcNow < timeoutAt) {
            await Task.Delay(50, ct);
        }
    }
}
