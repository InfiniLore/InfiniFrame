// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.InfiniFrameWindowFeatureFilePickerDialogs;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class FilePickerDialogsWhenClosedTests {
    [Test]
    [NotInParallelInfiniTests]
    [SkipOnMacOs]
    public async Task AtWindowStage_DirectAssignment(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        await EnsureWindowClosed(window, ct);

        // Act
        string?[] openFiles = window.Features.FilePickerDialogs.ShowOpenFile();
        string?[] openFolders = window.Features.FilePickerDialogs.ShowOpenFolder();
        string? saveFile = window.Features.FilePickerDialogs.ShowSaveFile();

        string?[] openFilesAsync = await window.Features.FilePickerDialogs.ShowOpenFileAsync(ct: ct);
        string?[] openFoldersAsync = await window.Features.FilePickerDialogs.ShowOpenFolderAsync(ct: ct);
        string? saveFileAsync = await window.Features.FilePickerDialogs.ShowSaveFileAsync(ct: ct);

        // Assert
        await Assert.That(openFiles).IsEmpty();
        await Assert.That(openFolders).IsEmpty();
        await Assert.That(saveFile).IsNull();
        await Assert.That(openFilesAsync).IsEmpty();
        await Assert.That(openFoldersAsync).IsEmpty();
        await Assert.That(saveFileAsync).IsNull();
    }

    [Test]
    [NotInParallelInfiniTests]
    [SkipOnMacOs]
    public async Task AtWindowStage_ExtensionAssignment(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        await EnsureWindowClosed(window, ct);

        // Act
        string?[] openFiles = window.ShowOpenFile();
        string?[] openFolders = window.ShowOpenFolder();
        string? saveFile = window.ShowSaveFile();

        string?[] openFilesAsync = await window.ShowOpenFileAsync(ct: ct);
        string?[] openFoldersAsync = await window.ShowOpenFolderAsync(ct: ct);
        string? saveFileAsync = await window.ShowSaveFileAsync(ct: ct);

        // Assert
        await Assert.That(openFiles).IsEmpty();
        await Assert.That(openFolders).IsEmpty();
        await Assert.That(saveFile).IsNull();
        await Assert.That(openFilesAsync).IsEmpty();
        await Assert.That(openFoldersAsync).IsEmpty();
        await Assert.That(saveFileAsync).IsNull();
    }

    private static async Task EnsureWindowClosed(IInfiniFrameWindow window, CancellationToken ct) {
        window.Close();
        DateTime timeoutAt = DateTime.UtcNow.AddSeconds(5);
        while (!window.IsClosedOrClosing() && DateTime.UtcNow < timeoutAt) {
            await Task.Delay(50, ct);
        }
    }
}
