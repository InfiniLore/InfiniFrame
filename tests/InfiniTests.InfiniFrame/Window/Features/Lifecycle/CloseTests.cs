// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.Lifecycle;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class CloseTests {
    [Test]
    [NotInParallelInfiniTests]
    [DefaultInfiniTestsTimeout(5_000)]
    public async Task CancelledClose_RestoresRunningState(CancellationToken ct = default) {
        // Arrange
        var closeAttempted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        int closeAttempts = 0;
        using var windowUtility = InfiniFrameTestWindow.Create(
            builder: builder => builder.RegisterWindowClosingHandler((_, _) => {
                if (Interlocked.Increment(ref closeAttempts) != 1) return WindowClosingResult.Close;

                closeAttempted.TrySetResult();
                return WindowClosingResult.Cancel;
            }),
            ct
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Close();
        await closeAttempted.Task.WaitAsync(TimeSpan.FromSeconds(3), ct);

        // Assert
        await Assert.That(window.LifecycleState).IsEqualTo(InfiniFrameWindowLifecycleState.Running);
        await Assert.That(window.Features.Lifecycle.IsClosedOrClosing()).IsFalse();
        await Assert.That(window.WindowHandle).IsNotEqualTo(IntPtr.Zero);
    }

    [Test]
    [NotInParallelInfiniTests]
    [DefaultInfiniTestsTimeout(5_000)]
    public async Task AtWindowStage_ExtensionAssignment(CancellationToken ct = default) {
        // Arrange
        var windowClosingTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        using var windowUtility = InfiniFrameTestWindow.Create(
            builder: builder => builder.EventsStore.WindowClosingRequested.Add(_ => {
                windowClosingTcs.TrySetResult(true);
            }),
            ct
        );
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        window.Close();

        // Assert
        bool windowClosing = await windowClosingTcs.Task.WaitAsync(TimeSpan.FromSeconds(3), ct);
        await Assert.That(windowClosing).IsTrue();
    }

    [Test]
    [NotInParallelInfiniTests]
    [DefaultInfiniTestsTimeout(2_000)]
    public async Task AtWindowStage_DirectAssignment(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        // Act
        await window.Features.Lifecycle.CloseAsync(ct);

        // Assert
        await Assert.That(window.Features.Lifecycle.IsClosedOrClosing()).IsTrue();
    }
}
