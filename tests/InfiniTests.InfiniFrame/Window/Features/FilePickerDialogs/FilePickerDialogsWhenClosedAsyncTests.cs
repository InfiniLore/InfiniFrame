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

    [Test]
    [NotInParallelInfiniTests]
    [DefaultInfiniTestsTimeout(30_000)]
    public async Task  ShowOpenFileAsync_Cancellation_ClosesNativeDialog(CancellationToken ct) {
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        using var cancellation = CancellationTokenSource.CreateLinkedTokenSource(ct);

        Task<string?[]> operation = window.ShowOpenFileAsync(
            title: "InfiniFrame cancellation test", ct: cancellation.Token
        );
        await WaitForOutstandingOperation(window, "OpenFile", ct);
        cancellation.Cancel();

        bool cancelled = false;
        try {
            await operation;
        }
        catch (OperationCanceledException) when (cancellation.IsCancellationRequested) {
            cancelled = true;
        }

        await WaitForOperationCompletion(window, ct);
        await Assert.That(cancelled).IsTrue();
        await Assert.That(window.GetDebugDiagnostics().LastOperation?.FinalState).IsEqualTo("Cancelled");
    }

    private static async Task EnsureWindowClosed(IInfiniFrameWindow window, CancellationToken ct) {
        window.Close();
        DateTime timeoutAt = DateTime.UtcNow.AddSeconds(5);
        while (!window.IsClosedOrClosing() && DateTime.UtcNow < timeoutAt) {
            await Task.Delay(50, ct);
        }
    }

    private static async Task WaitForOutstandingOperation(
        IInfiniFrameWindow window, string name, CancellationToken ct
    ) {
        DateTime timeoutAt = DateTime.UtcNow.AddSeconds(5);
        while (DateTime.UtcNow < timeoutAt) {
            if (window.GetDebugDiagnostics().OutstandingOperations.Any(operation => operation.Name == name))
                return;
            await Task.Delay(25, ct);
        }
        throw new TimeoutException($"The {name} operation was not registered.");
    }

    private static async Task WaitForOperationCompletion(IInfiniFrameWindow window, CancellationToken ct) {
        DateTime timeoutAt = DateTime.UtcNow.AddSeconds(5);
        while (DateTime.UtcNow < timeoutAt) {
            if (window.GetDebugDiagnostics().OutstandingOperations.Count == 0)
                return;
            await Task.Delay(25, ct);
        }
        throw new TimeoutException("The cancelled native dialog did not complete.");
    }
}
