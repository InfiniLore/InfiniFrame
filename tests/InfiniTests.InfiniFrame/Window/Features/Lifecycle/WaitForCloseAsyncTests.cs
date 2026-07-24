// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.Lifecycle;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WaitForCloseAsyncTests {
    [Test]
    [NotInParallelInfiniTests]
    [DefaultInfiniTestsTimeout(6_000)]
    public async Task WaitForCloseAsync_Extension_ShouldCompleteWhenWindowCloses(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        Task messageLoop = Task.Run(window.WaitForClose, ct);
        Task waitTask = window.WaitForCloseAsync(ct).AsTask();
        await Task.Delay(200, ct);
        await window.CloseAsync(ct);

        // Assert
        await waitTask.WaitAsync(TimeSpan.FromSeconds(4), ct);
        await messageLoop.WaitAsync(TimeSpan.FromSeconds(4), ct);
        await Assert.That(window.IsClosedOrClosing()).IsTrue();
    }

    [Test]
    [NotInParallelInfiniTests]
    [DefaultInfiniTestsTimeout(6_000)]
    public async Task WaitForCloseAsync_Feature_ShouldCompleteWhenWindowCloses(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        Task messageLoop = Task.Run(window.Features.Lifecycle.WaitForClose, ct);
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
    public async Task WaitForCloseAsync_CancellationOnlyCancelsCallerWait(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        using var cancellation = new CancellationTokenSource();

        // Act
        Task waitTask = window.WaitForCloseAsync(cancellation.Token).AsTask();
        cancellation.Cancel();

        // Assert
        await Assert.That(async () => await waitTask).Throws<OperationCanceledException>();
        await Assert.That(window.IsClosedOrClosing()).IsFalse();

        Task messageLoop = Task.Run(window.WaitForClose, ct);
        window.Close();
        await messageLoop.WaitAsync(TimeSpan.FromSeconds(4), ct);
    }
}
