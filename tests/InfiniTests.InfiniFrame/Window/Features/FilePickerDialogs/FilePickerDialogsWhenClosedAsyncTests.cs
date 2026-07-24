// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.FilePickerDialogs;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class FilePickerDialogsWhenClosedAsyncTests {
    [Test]
    [NotInParallelInfiniTests]
    public async Task ShowDialogsAsync_Feature_ShouldReturnEmptyResults(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        await EnsureWindowClosed(window, ct);

        // Act
        string?[] openFiles = await window.Features.FilePickerDialogs.ShowOpenFileAsync(ct: ct);
        string?[] openFolders = await window.Features.FilePickerDialogs.ShowOpenFolderAsync(ct: ct);
        string? saveFile = await window.Features.FilePickerDialogs.ShowSaveFileAsync(ct: ct);

        // Assert
        await Assert.That(openFiles).IsEmpty();
        await Assert.That(openFolders).IsEmpty();
        await Assert.That(saveFile).IsNull();
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task ShowDialogsAsync_Extensions_ShouldReturnEmptyResults(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        await EnsureWindowClosed(window, ct);

        // Act
        string?[] openFiles = await window.ShowOpenFileAsync(ct: ct);
        string?[] openFolders = await window.ShowOpenFolderAsync(ct: ct);
        string? saveFile = await window.ShowSaveFileAsync(ct: ct);

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
