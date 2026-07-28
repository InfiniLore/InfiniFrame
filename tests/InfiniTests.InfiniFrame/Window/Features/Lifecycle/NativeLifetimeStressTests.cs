// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.NativeBridge;

namespace InfiniTests.InfiniFrame.Window.Features.Lifecycle;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[NotInParallelInfiniTests]
public class NativeLifetimeStressTests {
    // Keep the stress deterministic across Linux runners with very different CPU counts.
    private const int ConcurrentFeatureCallerCount = 4;

    [Test]
    [OnlyRunOnMacOs]
    [NotInParallelInfiniTests]
    [DefaultInfiniTestsTimeout(20_000)]
    public Task RepeatedCloseAndRecreate_ReusesMacWebKitHost(CancellationToken ct) {
        // macOS keeps the complete AppKit/WebKit host alive across logical sessions.  Besides
        // catching the original display-link crash, this asserts that Close/WaitForClose expose
        // a completed logical session before the next compatible lease is constructed.
        const int iterations = 12;
        IntPtr? firstNativeWindow = null;

        for (int i = 0; i < iterations; i++) {
            ct.ThrowIfCancellationRequested();

            using var windowUtility = InfiniFrameTestWindow.Create(ct);
            IInfiniFrameWindow window = windowUtility.Window;
            IntPtr nativeWindow = window.WindowHandle;
            if (firstNativeWindow is null) firstNativeWindow = nativeWindow;
            else if (OperatingSystem.IsMacOS() && nativeWindow != firstNativeWindow.Value)
                throw new InvalidOperationException("A compatible macOS session did not reuse its pooled NSWindow/WKWebView host.");
            window.Close();
            window.WaitForClose();
        }

        return Task.CompletedTask;
    }

    [Test]
    [OnlyRunOnMacOs]
    [NotInParallelInfiniTests]
    [DefaultInfiniTestsTimeout(30_000)]
    public Task Pool_RemainsBounded_WhenMoreCompatibleSessionsClose(CancellationToken ct) {
        const int hostPoolLimit = 8;
        for (int i = 0; i < hostPoolLimit + 4; ++i) {
            using var windowUtility = InfiniFrameTestWindow.Create(builder: builder =>
                builder.Features.Decorations.SetChromeless(i % 2 == 0), ct);
            windowUtility.Window.Close();
            windowUtility.Window.WaitForClose();
        }

        if (InfiniFrameNativeTesting.MacPooledHostCount() > hostPoolLimit)
            throw new InvalidOperationException("The macOS host pool exceeded its configured bound.");
        return Task.CompletedTask;
    }

    [Test]
    [OnlyRunOnMacOs]
    [NotInParallelInfiniTests]
    public Task IncompatibleConstructionSettings_DoNotReuseHost(CancellationToken ct) {
        IntPtr titled;
        using (var first = InfiniFrameTestWindow.Create(ct)) {
            titled = first.Window.WindowHandle;
            first.Window.Close();
            first.Window.WaitForClose();
        }
        using var borderless = InfiniFrameTestWindow.Create(builder: builder =>
            builder.Features.Decorations.SetChromeless(true), ct);
        if (borderless.Window.WindowHandle == titled)
            throw new InvalidOperationException("A chromeless session reused an incompatible titled macOS host.");
        return Task.CompletedTask;
    }

    [Test]
    [DefaultInfiniTestsTimeout(20_000)]
    public async Task FeatureCallsRacingClose_DoNotReachFreedNativeInstance(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;
        using var stop = CancellationTokenSource.CreateLinkedTokenSource(ct);
        int completedCalls = 0;

        Task[] callers = Enumerable.Range(0, ConcurrentFeatureCallerCount)
            .Select(workerIndex => Task.Run(action: () => {
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

        // Act
        await Task.Delay(50, ct);
        window.Close();
        window.WaitForClose();
        stop.Cancel();

        try {
            await Task.WhenAll(callers);
        }
        catch (OperationCanceledException) when (stop.IsCancellationRequested) {}

        // Assert
        await Assert.That(completedCalls).IsGreaterThanOrEqualTo(0);
        await Assert.That(window.Features.Lifecycle.State).IsEqualTo(InfiniFrameWindowLifecycleState.NativeClosed);
        await Assert.That(() => window.Features.State.IsFocused).Throws<ObjectDisposedException>();
    }

    [Test]
    [DefaultInfiniTestsTimeout(20_000)]
    public async Task ConcurrentCloseRequests_ProduceSingleDeterministicShutdown(CancellationToken ct) {
        // Arrange
        using var windowUtility = InfiniFrameTestWindow.Create(ct);
        IInfiniFrameWindow window = windowUtility.Window;

        Task[] closeRequests = Enumerable.Range(0, 16)
            .Select(_ => Task.Run(window.Close, ct))
            .ToArray();

        // Act
        await Task.WhenAll(closeRequests);
        window.WaitForClose();

        // Assert
        await Assert.That(window.Features.Lifecycle.State).IsEqualTo(InfiniFrameWindowLifecycleState.NativeClosed);

        // Act
        ((IDisposable)window).Dispose();

        // Assert
        await Assert.That(window.Features.Lifecycle.State).IsEqualTo(InfiniFrameWindowLifecycleState.Disposed);
    }
}
