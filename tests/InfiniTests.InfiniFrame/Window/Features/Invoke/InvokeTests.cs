// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Window.Features.Invoke;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InvokeTests {
    [Test]
    [NotInParallelInfiniTests]
    public async Task DispatchAsync_NestedDispatch_CompletesWithoutDeadlock(CancellationToken ct) {
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        int callbacks = 0;

        Task<InfiniFrameDispatchResult>[] dispatches = Enumerable.Range(0, 32)
            .Select(_ => window.DispatchAsync(() => {
                Interlocked.Increment(ref callbacks);
                InfiniFrameDispatchResult nested = window.Features.Invoke.Invoke(() => Interlocked.Increment(ref callbacks));
                if (nested != InfiniFrameDispatchResult.Completed)
                    throw new InvalidOperationException($"Nested dispatch ended with {nested}.");
            }, TimeSpan.FromSeconds(5), ct))
            .ToArray();

        InfiniFrameDispatchResult[] results = await Task.WhenAll(dispatches);

        await Assert.That(results.All(x => x == InfiniFrameDispatchResult.Completed)).IsTrue();
        await Assert.That(callbacks).IsEqualTo(64);
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task DispatchAsync_Timeout_SuppressesLateCallback(CancellationToken ct) {
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        bool lateCallbackRan = false;

        Task<InfiniFrameDispatchResult> blocker = window.DispatchAsync(() => {
            entered.SetResult();
            Thread.Sleep(250);
        }, TimeSpan.FromSeconds(5), ct);
        // macOS UI tests begin on the AppKit main queue. Do not capture that queue here:
        // the continuation must enqueue the short-timeout operation while the callback
        // above is still blocking the main queue, not after its sleep has completed.
        await entered.Task.ConfigureAwait(false);

        InfiniFrameDispatchResult result = await window.DispatchAsync(
            () => lateCallbackRan = true,
            TimeSpan.FromMilliseconds(25),
            ct).ConfigureAwait(false);
        await blocker.ConfigureAwait(false);
        await Task.Delay(50, ct).ConfigureAwait(false);

        await Assert.That(result).IsEqualTo(InfiniFrameDispatchResult.TimedOut);
        await Assert.That(lateCallbackRan).IsFalse();
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task DispatchAsync_AfterShutdown_ReturnsWindowClosedWithoutExecuting(CancellationToken ct) {
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        await window.Features.Lifecycle.CloseAsync(ct);
        bool callbackRan = false;

        InfiniFrameDispatchResult result = await window.DispatchAsync(() => callbackRan = true, cancellationToken: ct);

        await Assert.That(result).IsEqualTo(InfiniFrameDispatchResult.WindowClosed);
        await Assert.That(callbackRan).IsFalse();
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_DirectAssignment(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        int callbackThreadId = -1;

        // Act
        window.Features.Invoke.Invoke(() => {
            callbackThreadId = Environment.CurrentManagedThreadId;
        });

        // Assert
        await Assert.That(callbackThreadId).IsEqualTo(window.ManagedThreadId);
    }

    [Test]
    [NotInParallelInfiniTests]
    public async Task AtWindowStage_ExtensionAssignment(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        int callbackThreadId = -1;

        // Act
        IInfiniFrameWindow returnedWindow = window.Invoke(() => {
            callbackThreadId = Environment.CurrentManagedThreadId;
        });

        // Assert
        await Assert.That(callbackThreadId).IsEqualTo(window.ManagedThreadId);
        await Assert.That(returnedWindow).IsSameReferenceAs(window);
    }
}
