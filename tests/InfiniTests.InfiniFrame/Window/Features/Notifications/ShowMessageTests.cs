// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Dialogs;

namespace InfiniTests.InfiniFrame.Window.Features.Notifications;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class ShowMessageTests {
    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_DirectAssignment_WhenClosed_ReturnsCancel(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        await EnsureWindowClosed(window, ct);

        // Act
        InfiniFrameDialogResult result = window.Features.Notifications.ShowMessage("title", "body");

        // Assert
        await Assert.That(result).IsEqualTo(InfiniFrameDialogResult.Cancel);
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_ExtensionAssignment_WhenClosed_ReturnsCancel(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        await EnsureWindowClosed(window, ct);

        // Act
        InfiniFrameDialogResult result = window.ShowMessage("title", "body");

        // Assert
        await Assert.That(result).IsEqualTo(InfiniFrameDialogResult.Cancel);
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task ShowMessageAsync_WhenClosed_ReturnsCancel(CancellationToken ct) {
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        await EnsureWindowClosed(window, ct);

        InfiniFrameDialogResult result = await window.ShowMessageAsync("title", "body", ct: ct);

        await Assert.That(result).IsEqualTo(InfiniFrameDialogResult.Cancel);
    }

    [Test]
    [NotInParallelInfiniTests]
    [DefaultInfiniTestsTimeout(30_000)]
    public async Task ShowMessageAsync_Cancellation_ClosesNativeDialog(CancellationToken ct) {
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        using var cancellation = CancellationTokenSource.CreateLinkedTokenSource(ct);

        Task<InfiniFrameDialogResult> operation = window.ShowMessageAsync(
            "InfiniFrame cancellation test", "This dialog should close automatically.",
            InfiniFrameDialogButtons.OkCancel, ct: cancellation.Token
        );
        await WaitForOutstandingOperation(window, "ShowMessage", ct);
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