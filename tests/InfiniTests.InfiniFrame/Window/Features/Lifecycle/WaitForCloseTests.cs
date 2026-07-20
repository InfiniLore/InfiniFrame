// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.Lifecycle;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WaitForCloseTests {
    [Test]
    [NotInParallelInfiniTests]
    [DefaultInfiniTestsTimeout(6_000)]
    public async Task AtWindowStage_ExtensionAssignment(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        Task waitTask = Task.Run(() => window.WaitForClose(), ct);
        await Task.Delay(200, ct);
        window.Close();

        // Assert
        await waitTask.WaitAsync(TimeSpan.FromSeconds(4), ct);
        await Assert.That(window.IsClosedOrClosing()).IsTrue();
    }

    [Test]
    [NotInParallelInfiniTests]
    [DefaultInfiniTestsTimeout(6_000)]
    public async Task AtWindowStage_DirectAssignment(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act:
        // The synchronous API owns the platform message loop. The async API only observes
        // the native closed callback and therefore never occupies a worker thread.
        Task messageLoop = Task.Run(window.WaitForClose, ct);
        Task waitTask = window.Features.Lifecycle.WaitForCloseAsync(ct).AsTask();
        await Task.Delay(200, ct);
        await window.Features.Lifecycle.CloseAsync(ct);

        // Assert
        await waitTask.WaitAsync(TimeSpan.FromSeconds(4), ct);
        await messageLoop.WaitAsync(TimeSpan.FromSeconds(4), ct);
        await Assert.That(window.Features.Lifecycle.IsClosedOrClosing()).IsTrue();
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task WaitForCloseAsync_CancellationOnlyCancelsTheCallerWait(CancellationToken ct) {
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        using var cancellation = new CancellationTokenSource();

        Task waitTask = window.WaitForCloseAsync(cancellation.Token).AsTask();
        cancellation.Cancel();

        await Assert.That(async () => await waitTask).Throws<OperationCanceledException>();
        await Assert.That(window.Features.Lifecycle.IsClosedOrClosing()).IsFalse();

        Task messageLoop = Task.Run(window.WaitForClose, ct);
        window.Close();
        await messageLoop.WaitAsync(TimeSpan.FromSeconds(4), ct);
    }
}
