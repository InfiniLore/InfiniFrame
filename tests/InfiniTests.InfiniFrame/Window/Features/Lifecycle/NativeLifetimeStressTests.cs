// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.NativeBridge.Managed;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[NotInParallelInfiniTests]
public class NativeLifetimeStressTests {
    // Keep the stress deterministic across Linux runners with very different CPU counts.
    private const int ConcurrentFeatureCallerCount = 4;

    [Test]
    [DefaultInfiniTestsTimeout(20_000)]
    public async Task FeatureCallsRacingClose_DoNotReachFreedNativeInstance(CancellationToken ct) {
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        using var stop = CancellationTokenSource.CreateLinkedTokenSource(ct);
        int completedCalls = 0;

        Task[] callers = Enumerable.Range(0, ConcurrentFeatureCallerCount)
            .Select(workerIndex => Task.Run(() => {
                _ = workerIndex;
                while (!stop.IsCancellationRequested) {
                    try {
                        _ = window.Features.State.IsFocused;
                        Interlocked.Increment(ref completedCalls);
                    }
                    catch (ObjectDisposedException) {
                        return;
                    }
                }
            }, stop.Token)).ToArray();

        await Task.Delay(50, ct);
        window.Close();
        window.WaitForClose();
        stop.Cancel();

        try {
            await Task.WhenAll(callers);
        }
        catch (OperationCanceledException) when (stop.IsCancellationRequested) {}

        await Assert.That(completedCalls).IsGreaterThanOrEqualTo(0);
        await Assert.That(window.Features.Lifecycle.State).IsEqualTo(InfiniFrameWindowLifecycleState.NativeClosed);
        await Assert.That(() => window.Features.State.IsFocused).Throws<ObjectDisposedException>();
    }

    [Test]
    [DefaultInfiniTestsTimeout(20_000)]
    public async Task ConcurrentCloseRequests_ProduceSingleDeterministicShutdown(CancellationToken ct) {
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        Task[] closeRequests = Enumerable.Range(0, 16)
            .Select(_ => Task.Run(window.Close, ct))
            .ToArray();

        await Task.WhenAll(closeRequests);
        window.WaitForClose();

        await Assert.That(window.Features.Lifecycle.State).IsEqualTo(InfiniFrameWindowLifecycleState.NativeClosed);
        ((IDisposable)window).Dispose();
        await Assert.That(window.Features.Lifecycle.State).IsEqualTo(InfiniFrameWindowLifecycleState.Disposed);
    }
}
