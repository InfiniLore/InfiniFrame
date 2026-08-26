// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge.Dialogs;

namespace InfiniTests.InfiniFrame.Window.Features.Lifecycle;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class DisposeAsyncTests {
    [Test]
    [NotInParallelInfiniTests]
    [DefaultInfiniTestsTimeout(30_000)]
    public async Task DisposeAsync_Extension_ShouldDrainWindowToNativeHandleReleasedOrDisposed(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        await ((IAsyncDisposable)window).DisposeAsync();

        // Assert
        await Assert.That((int)window.LifecycleState)
            .IsGreaterThanOrEqualTo((int)InfiniFrameWindowLifecycleState.NativeHandleReleased);
    }

    [Test]
    [NotInParallelInfiniTests]
    [DefaultInfiniTestsTimeout(30_000)]
    public async Task DisposeAsync_OutstandingOperations_ShouldBeDrained(CancellationToken ct = default) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        Task<InfiniFrameDialogResult> dialog = window.ShowMessageAsync("title", "body", ct: ct);
        await WaitForOutstandingOperation(window, "ShowMessage", ct);

        // Act
        await ((IAsyncDisposable)window).DisposeAsync();

        // Assert: the outstanding dialog was drained during teardown.
        InfiniFrameDialogResult result = await dialog.WaitAsync(TimeSpan.FromSeconds(5), ct);
        await Assert.That(result).IsEqualTo(InfiniFrameDialogResult.Cancel);
    }

    private static async Task WaitForOutstandingOperation(
        IInfiniFrameWindow window,
        string name,
        CancellationToken ct
    ) {
        DateTime timeoutAt = DateTime.UtcNow.AddSeconds(5);
        while (DateTime.UtcNow < timeoutAt) {
            if (window.GetDebugDiagnostics().OutstandingOperations.Any(operation => operation.Name == name))
                return;

            await Task.Delay(25, ct);
        }

        throw new TimeoutException($"The {name} operation was not registered.");
    }
}
