// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.FilePickerDialogs;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class FilePickerDialogsWhenClosedSyncTests {
    [Test]
    [NotInParallelInfiniTests]
    public async Task ShowDialogs_Feature_ShouldReturnEmptyResults(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        await EnsureWindowClosed(window, ct);

        // Act
        string?[] openFiles = window.Features.FilePickerDialogs.ShowOpenFile();
        string?[] openFolders = window.Features.FilePickerDialogs.ShowOpenFolder();
        string? saveFile = window.Features.FilePickerDialogs.ShowSaveFile();

        // Assert
        await Assert.That(openFiles).IsEmpty();
        await Assert.That(openFolders).IsEmpty();
        await Assert.That(saveFile).IsNull();
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task ShowDialogs_Extensions_ShouldReturnEmptyResults(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        await EnsureWindowClosed(window, ct);

        // Act
        string?[] openFiles = window.ShowOpenFile();
        string?[] openFolders = window.ShowOpenFolder();
        string? saveFile = window.ShowSaveFile();

        // Assert
        await Assert.That(openFiles).IsEmpty();
        await Assert.That(openFolders).IsEmpty();
        await Assert.That(saveFile).IsNull();
    }

    private static async Task EnsureWindowClosed(IInfiniFrameWindow window, CancellationToken ct) {
        window.Close();
        DateTime timeoutAt = DateTime.UtcNow.AddSeconds(5);
        while (!window.IsClosedOrClosing() && DateTime.UtcNow < timeoutAt) {
            await Task.Delay(50, ct);
        }
    }
}