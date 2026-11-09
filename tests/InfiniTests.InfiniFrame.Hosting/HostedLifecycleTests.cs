// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Window.Events;
using InfiniFrame.Window.Hosting;

namespace InfiniTests.InfiniFrame.Hosting;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
// Facade lifecycle and callback roots. Contract tests only; no browser.
[NotInParallel]
public sealed class HostedLifecycleTests {
    [Test]
    [Timeout(30_000)]
    public async Task Ready_OnlyAfterProviderReportsCreated_AndWindowCreatedRunsOnDispatcher(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync(new ScriptedHostProvider { AutoCreated = false });
        int createdThread = 0;
        (InfiniFrameHostedWindow window, ScriptedWindowHostCore host) =
            await harness.CreateAsync(builder => builder.RegisterWindowCreatedHandler(_ => createdThread = Environment.CurrentManagedThreadId));

        await Assert.That(window.LifecycleState).IsEqualTo(InfiniFrameWindowLifecycleState.Creating);
        Task ready = window.WaitForReadyAsync(ct).AsTask();
        await Task.Delay(100, ct);
        await Assert.That(ready.IsCompleted).IsFalse();

        await host.CreatedAsync();
        await Waits.WithinAsync(ready, "ready after Created()");
        await harness.DrainDispatcherAsync();
        await Assert.That(window.LifecycleState).IsEqualTo(InfiniFrameWindowLifecycleState.Ready);
        await Assert.That(createdThread).IsEqualTo(window.ManagedThreadId);
        await Assert.That(createdThread).IsNotEqualTo(harness.Host.LoopThreadId);
    }

    [Test]
    [Timeout(30_000)]
    public async Task Close_CompletesOnlyOnClosedNotification_AndTearsDownOnce(CancellationToken ct) {
        int baseline = InfiniFrameEvents.ActiveNativeCallbackRoots;
        await using HostedHarness harness = await HostedHarness.StartAsync(new ScriptedHostProvider { AutoClose = false });
        int closedCallbacks = 0;
        (InfiniFrameHostedWindow window, ScriptedWindowHostCore host) =
            await harness.CreateAsync(builder => builder.RegisterWindowClosedHandler(_ => Interlocked.Increment(ref closedCallbacks)));
        await window.WaitForReadyAsync(ct);
        await Assert.That(InfiniFrameEvents.ActiveNativeCallbackRoots).IsEqualTo(baseline + 1);

        Task close = window.CloseAsync(ct).AsTask();
        await Waits.UntilAsync(() => host.RequestCloseCalls == 1, "RequestCloseAsync delivered");
        await Task.Delay(100, ct);
        await Assert.That(close.IsCompleted).IsFalse();
        await Assert.That(window.LifecycleState).IsEqualTo(InfiniFrameWindowLifecycleState.CloseRequested);

        await host.CloseNativelyAsync();
        await Waits.WithinAsync(close, "close completes after Closed()");
        await window.WaitForTeardownAsync(ct);
        await Assert.That(closedCallbacks).IsEqualTo(1);
        await Assert.That(host.DisposeCalls).IsEqualTo(1);
        await Assert.That(InfiniFrameEvents.ActiveNativeCallbackRoots).IsEqualTo(baseline);

        await window.DisposeAsync();
        await window.DisposeAsync();
        await Assert.That(window.LifecycleState).IsEqualTo(InfiniFrameWindowLifecycleState.Disposed);
        await Assert.That(host.DisposeCalls).IsEqualTo(1);
        await Assert.That(host.RequestCloseCalls).IsEqualTo(1);
    }

    [Test]
    [Timeout(30_000)]
    public async Task DuplicateAndLateNotifications_AreIgnoredWithoutDoubleDisposal(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        int closedCallbacks = 0;
        int messages = 0;
        (InfiniFrameHostedWindow window, ScriptedWindowHostCore host) = await harness.CreateAttachedAsync(1, builder => builder
            .RegisterWindowClosedHandler(_ => Interlocked.Increment(ref closedCallbacks))
            .RegisterWebMessageReceivedHandler((_, _) => Interlocked.Increment(ref messages)));

        await host.CloseNativelyAsync();
        await host.CloseNativelyAsync();
        await host.FaultAsync(new InvalidOperationException("late fault"));
        await host.PostAsync(1, "late message");
        await host.CreatedAsync();
        await host.AttachAsync(2);
        await window.WaitForTeardownAsync(ct);
        await harness.DrainDispatcherAsync();

        await Assert.That(closedCallbacks).IsEqualTo(1);
        await Assert.That(messages).IsEqualTo(0);
        await Assert.That(host.DisposeCalls).IsEqualTo(1);
        await Assert.That(host.RequestCloseCalls).IsEqualTo(0);
        await Assert.That(window.Fault).IsNull();
        await Assert.That(window.CurrentDocument).IsEqualTo(0UL);
    }

    [Test]
    [Timeout(30_000)]
    public async Task ProviderFault_EndsWindowOnce_RequestsNativeClose_AndDisposesHostOnce(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync(new ScriptedHostProvider { AutoClose = false });
        int closedCallbacks = 0;
        (InfiniFrameHostedWindow window, ScriptedWindowHostCore host) =
            await harness.CreateAttachedAsync(1, builder => builder.RegisterWindowClosedHandler(_ => Interlocked.Increment(ref closedCallbacks)));

        var fault = new InvalidOperationException("renderer lost");
        await host.FaultAsync(fault);
        await window.WaitForTeardownAsync(ct);
        await host.CloseNativelyAsync();
        await harness.DrainDispatcherAsync();

        await Assert.That(window.Fault).IsSameReferenceAs(fault);
        await Assert.That(closedCallbacks).IsEqualTo(1);
        await Assert.That(host.RequestCloseCalls).IsEqualTo(1);
        await Assert.That(host.DisposeCalls).IsEqualTo(1);
        await Assert.That(window.IsClosedOrClosing()).IsTrue();
    }

    [Test]
    [Timeout(30_000)]
    public async Task FaultBeforeCreated_FailsReadyWithTheFault(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync(new ScriptedHostProvider { AutoCreated = false });
        (InfiniFrameHostedWindow window, ScriptedWindowHostCore host) = await harness.CreateAsync();
        Task ready = window.WaitForReadyAsync(ct).AsTask();

        await host.FaultAsync(new TimeoutException("native window never appeared"));
        TimeoutException exception = await Expect.ThrowsAsync<TimeoutException>(() => ready);
        await Assert.That(exception.Message).Contains("never appeared");
    }

    [Test]
    [Timeout(30_000)]
    public async Task ApplicationClose_CanBeCancelledByClosingHandler(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        bool allow = false;
        (InfiniFrameHostedWindow window, ScriptedWindowHostCore host) = await harness.CreateAsync(builder =>
            builder.RegisterWindowClosingHandler((_, _) => allow ? WindowClosingResult.Close : WindowClosingResult.Cancel));
        await window.WaitForReadyAsync(ct);

        await Assert.ThrowsAsync<InfiniFrameCloseRejectedException>(async () => await window.CloseAsync(ct));
        await Assert.That(window.LifecycleState).IsEqualTo(InfiniFrameWindowLifecycleState.Ready);
        await Assert.That(host.RequestCloseCalls).IsEqualTo(0);

        allow = true;
        await Waits.WithinAsync(window.CloseAsync(ct).AsTask(), "second close");
        await Assert.That(host.RequestCloseCalls).IsEqualTo(1);
        await Assert.That(window.LifecycleState >= InfiniFrameWindowLifecycleState.NativeClosed).IsTrue();
    }

    [Test]
    [Timeout(30_000)]
    public async Task NativeCloseRequest_RunsClosingHandlersOnDispatcher_AndCanBeCancelled(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        bool allow = false;
        int handlerThread = 0;
        (InfiniFrameHostedWindow window, ScriptedWindowHostCore host) = await harness.CreateAsync(builder =>
            builder.RegisterWindowClosingHandler((_, _) => {
                handlerThread = Environment.CurrentManagedThreadId;
                return allow ? WindowClosingResult.Close : WindowClosingResult.Cancel;
            }));
        await window.WaitForReadyAsync(ct);

        bool proceed = await host.Events.CloseRequestedAsync(ct);
        await Assert.That(proceed).IsFalse();
        await Assert.That(window.LifecycleState).IsEqualTo(InfiniFrameWindowLifecycleState.Ready);
        await Assert.That(handlerThread).IsEqualTo(window.ManagedThreadId);

        allow = true;
        proceed = await host.Events.CloseRequestedAsync(ct);
        await Assert.That(proceed).IsTrue();
        await Assert.That(window.LifecycleState).IsEqualTo(InfiniFrameWindowLifecycleState.CloseRequested);
        await host.CloseNativelyAsync();
        await window.WaitForTeardownAsync(ct);
        await Assert.That(host.RequestCloseCalls).IsEqualTo(0);
    }

    [Test]
    [Timeout(30_000)]
    public async Task NativeCloseRequest_DuplicateCannotBypassPendingVeto(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        int closingCalls = 0;
        (InfiniFrameHostedWindow window, ScriptedWindowHostCore host) = await harness.CreateAsync(builder =>
            builder.RegisterWindowClosingHandler((_, _) => {
                Interlocked.Increment(ref closingCalls);
                return WindowClosingResult.Cancel;
            }));
        await window.WaitForReadyAsync(ct);
        using var release = new ManualResetEventSlim();
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        Task<InfiniFrameDispatchResult> blocker = window.DispatchAsync(() => {
            entered.TrySetResult();
            release.Wait(ct);
        }, cancellationToken: ct).AsTask();
        Task<bool> first;
        Task<bool> duplicate;
        try {
            await entered.Task.WaitAsync(ct);
            first = host.Events.CloseRequestedAsync(ct).AsTask();
            duplicate = host.Events.CloseRequestedAsync(ct).AsTask();
        }
        finally {
            release.Set();
        }

        await blocker;
        await Assert.That(await first).IsFalse();
        await Assert.That(await duplicate).IsFalse();
        await Assert.That(closingCalls).IsEqualTo(1);
        await Assert.That(window.LifecycleState).IsEqualTo(InfiniFrameWindowLifecycleState.Ready);
    }

    [Test]
    [Timeout(30_000)]
    public async Task NativeCloseRequest_CancellationSuppressesQueuedHandlers_AndAllowsRetry(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        int closingCalls = 0;
        (InfiniFrameHostedWindow window, ScriptedWindowHostCore host) = await harness.CreateAsync(builder =>
            builder.RegisterWindowClosingHandler((_, _) => {
                Interlocked.Increment(ref closingCalls);
                return WindowClosingResult.Cancel;
            }));
        await window.WaitForReadyAsync(ct);
        using var release = new ManualResetEventSlim();
        using var cancelled = new CancellationTokenSource();
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        Task<InfiniFrameDispatchResult> blocker = window.DispatchAsync(() => {
            entered.TrySetResult();
            release.Wait(ct);
        }, cancellationToken: ct).AsTask();
        try {
            await entered.Task.WaitAsync(ct);
            Task<bool> first = host.Events.CloseRequestedAsync(cancelled.Token).AsTask();
            await cancelled.CancelAsync();
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await first);
            await Assert.That(window.LifecycleState).IsEqualTo(InfiniFrameWindowLifecycleState.Ready);
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await host.Events.CloseRequestedAsync(cancelled.Token));
            await Assert.That(window.LifecycleState).IsEqualTo(InfiniFrameWindowLifecycleState.Ready);
        }
        finally {
            release.Set();
        }
        await blocker;
        await harness.DrainDispatcherAsync();
        await Assert.That(closingCalls).IsEqualTo(0);

        await Assert.That(await host.Events.CloseRequestedAsync(ct)).IsFalse();
        await Assert.That(closingCalls).IsEqualTo(1);
        await Assert.That(window.LifecycleState).IsEqualTo(InfiniFrameWindowLifecycleState.Ready);
    }

    [Test]
    [Timeout(30_000)]
    public async Task DisposeOnDispatcherThread_DoesNotDeadlock_AndCompletesAfterTeardown(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        (InfiniFrameHostedWindow window, ScriptedWindowHostCore host) = await harness.CreateAsync();
        await window.WaitForReadyAsync(ct);

        Task<InfiniFrameDispatchResult> dispose = window.DispatchAsync(window.Dispose, cancellationToken: ct).AsTask();
        await Waits.WithinAsync(dispose, "Dispose() called on the dispatcher thread");
        await Assert.That(dispose.Result).IsEqualTo(InfiniFrameDispatchResult.Completed);
        await Waits.UntilAsync(() => window.LifecycleState == InfiniFrameWindowLifecycleState.Disposed, "deferred disposal");
        await Assert.That(host.DisposeCalls).IsEqualTo(1);
    }

    [Test]
    [Timeout(30_000)]
    public async Task DisposeAsync_FromAnotherThread_ForceClosesWithoutClosingHandlers(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        (InfiniFrameHostedWindow window, ScriptedWindowHostCore host) = await harness.CreateAsync(builder =>
            builder.RegisterWindowClosingHandler((_, _) => WindowClosingResult.Cancel));
        await window.WaitForReadyAsync(ct);

        await Waits.WithinAsync(Task.Run(async () => await window.DisposeAsync(), ct), "DisposeAsync");
        await Assert.That(window.LifecycleState).IsEqualTo(InfiniFrameWindowLifecycleState.Disposed);
        await Assert.That(host.RequestCloseCalls).IsEqualTo(1);
        await Assert.That(host.DisposeCalls).IsEqualTo(1);
    }

    [Test]
    [Timeout(30_000)]
    public async Task CreationFailure_RollsBackWithoutClosedCallbacks(CancellationToken ct) {
        var provider = new ScriptedHostProvider { FailCreateWindow = _ => new NotSupportedException("the provider cannot create this window") };
        await using HostedHarness harness = await HostedHarness.StartAsync(provider);
        int closedCallbacks = 0;
        InfiniFrameHostedWindow window = harness.Build(builder => builder.RegisterWindowClosedHandler(_ => Interlocked.Increment(ref closedCallbacks)));

        await Assert.ThrowsAsync<NotSupportedException>(() => harness.Runtime.CreateWindowsAsync([window], ct));
        await harness.DrainDispatcherAsync();
        await Assert.That(window.LifecycleState).IsEqualTo(InfiniFrameWindowLifecycleState.Disposed);
        await Assert.That(closedCallbacks).IsEqualTo(0);
        await Assert.ThrowsAsync<NotSupportedException>(async () => await window.WaitForReadyAsync(ct));
    }
}
