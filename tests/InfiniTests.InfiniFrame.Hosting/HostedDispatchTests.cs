// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;
using InfiniFrame;
using InfiniFrame.Window.Hosting;

namespace InfiniTests.InfiniFrame.Hosting;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
// Dispatch: intended managed context, callbacks after closure, no same-context synchronous-wait deadlock.
[NotInParallel]
public sealed class HostedDispatchTests {
    [Test]
    [Timeout(30_000)]
    public async Task ProviderEvents_RunOnTheCoreDispatcher_NeverOnTheProviderLoop(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        int messageThread = 0;
        int closedThread = 0;
        (InfiniFrameHostedWindow window, ScriptedWindowHostCore host) = await harness.CreateAttachedAsync(1, builder => builder
            .RegisterWebMessageReceivedHandler((_, _) => messageThread = Environment.CurrentManagedThreadId)
            .RegisterWindowClosedHandler(_ => closedThread = Environment.CurrentManagedThreadId));

        await host.PostAsync(1, "__bwv:hello");
        await harness.DrainDispatcherAsync();
        await host.CloseNativelyAsync();
        await window.WaitForTeardownAsync(ct);

        await Assert.That(messageThread).IsEqualTo(window.ManagedThreadId);
        await Assert.That(closedThread).IsEqualTo(window.ManagedThreadId);
        await Assert.That(messageThread).IsNotEqualTo(harness.Host.LoopThreadId);
    }

    [Test]
    [Timeout(30_000)]
    public async Task SynchronousFacadeCalls_FromTheDispatcherThread_DoNotDeadlock(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        var outcome = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
        (InfiniFrameHostedWindow window, ScriptedWindowHostCore host) = await harness.CreateAttachedAsync(1, builder =>
            builder.RegisterWebMessageReceivedHandler((sender, _) => {
                try {
                    // Each call blocks this dispatcher thread on a provider task that completes on the provider loop.
                    sender.Features.Size.SetSize(500, 400);
                    sender.Features.Decorations.SetTitle("from-handler");
                    sender.SendWebMessage("reply");
                    outcome.TrySetResult($"{sender.Features.Size.Size.Width}:{sender.Features.Decorations.Title}");
                }
                catch (Exception exception) {
                    outcome.TrySetException(exception);
                }
            }));

        await host.PostAsync(1, "__bwv:go");
        await Waits.WithinAsync(outcome.Task, "synchronous provider calls from a dispatcher callback");
        await Assert.That(outcome.Task.Result).IsEqualTo("500:from-handler");
        await Assert.That(host.Sent.Count).IsEqualTo(1);
        await Assert.That(((ScriptedWindowHost)host).Bounds.Size).IsEqualTo(new Size(500, 400));
    }

    [Test]
    [Timeout(30_000)]
    public async Task InvokeAndDispatch_ReportCompletionCancellationTimeoutAndClosure(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        (InfiniFrameHostedWindow window, ScriptedWindowHostCore host) = await harness.CreateAttachedAsync(1);

        int ranOn = 0;
        await Assert.That(window.Invoke(() => ranOn = Environment.CurrentManagedThreadId)).IsEqualTo(InfiniFrameDispatchResult.Completed);
        await Assert.That(ranOn).IsEqualTo(window.ManagedThreadId);

        InfiniFrameDispatchResult nested = InfiniFrameDispatchResult.Failed;
        await window.DispatchAsync(() => nested = window.Invoke(() => {}), cancellationToken: ct);
        await Assert.That(nested).IsEqualTo(InfiniFrameDispatchResult.Completed);

        using var cancelled = new CancellationTokenSource();
        await cancelled.CancelAsync();
        await Assert.That(await window.DispatchAsync(() => {}, cancellationToken: cancelled.Token)).IsEqualTo(InfiniFrameDispatchResult.Cancelled);

        using var release = new ManualResetEventSlim();
        Task<InfiniFrameDispatchResult> blocker = window.DispatchAsync(() => release.Wait(ct), TimeSpan.FromSeconds(20), ct).AsTask();
        await Assert.That(await window.DispatchAsync(() => {}, TimeSpan.FromMilliseconds(100), ct)).IsEqualTo(InfiniFrameDispatchResult.TimedOut);
        release.Set();
        await blocker;

        await host.CloseNativelyAsync();
        await window.WaitForTeardownAsync(ct);
        bool ranAfterClose = false;
        await Assert.That(window.Invoke(() => ranAfterClose = true)).IsEqualTo(InfiniFrameDispatchResult.WindowClosed);
        await Assert.That(await window.DispatchAsync(() => ranAfterClose = true, cancellationToken: ct)).IsEqualTo(InfiniFrameDispatchResult.WindowClosed);
        await Assert.That(ranAfterClose).IsFalse();
    }

    [Test]
    [Arguments(false)]
    [Arguments(true)]
    [Timeout(30_000)]
    public async Task ApplicationDispatchQueuedBeforeClosed_IsSuppressedWhenShutdownWins(bool synchronous, CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        (InfiniFrameHostedWindow window, ScriptedWindowHostCore host) = await harness.CreateAttachedAsync(1);
        using var release = new ManualResetEventSlim();
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        Task<InfiniFrameDispatchResult> blocker = window.DispatchAsync(() => {
            entered.TrySetResult();
            release.Wait(ct);
        }, cancellationToken: ct).AsTask();
        bool ran = false;
        Task<InfiniFrameDispatchResult> queued;
        try {
            await entered.Task.WaitAsync(ct);
            queued = synchronous
                ? Task.Run(() => window.Invoke(() => ran = true), ct)
                : window.DispatchAsync(() => ran = true, cancellationToken: ct).AsTask();
            await Waits.UntilAsync(() => harness.Runtime.Dispatcher.QueuedApplicationItems == 2, "application callback queued behind blocker");
            host.Events.Closed();
        }
        finally {
            release.Set();
        }

        await Assert.That(await blocker).IsEqualTo(InfiniFrameDispatchResult.Completed);
        await Assert.That(await queued).IsEqualTo(InfiniFrameDispatchResult.WindowClosed);
        await Assert.That(ran).IsFalse();
        await window.WaitForTeardownAsync(ct);
    }

    [Test]
    [Timeout(60_000)]
    public async Task DispatcherBudgetExhaustion_FaultsTheWindow_InsteadOfDroppingMessages(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        int delivered = 0;
        (InfiniFrameHostedWindow window, ScriptedWindowHostCore host) = await harness.CreateAttachedAsync(1, builder =>
            builder.RegisterWebMessageReceivedHandler((_, _) => Interlocked.Increment(ref delivered)));

        using var release = new ManualResetEventSlim();
        Task<InfiniFrameDispatchResult> blocker = window.DispatchAsync(() => release.Wait(ct), TimeSpan.FromSeconds(50), ct).AsTask();
        for (int index = 0; index <= InfiniFrameHostedDispatcher.DefaultItemCapacity && window.Fault is null; index++)
            await host.PostAsync(1, $"__bwv:m{index}");
        release.Set();
        await blocker;
        await window.WaitForTeardownAsync(ct);

        await Assert.That(window.Fault).IsNotNull();
        await Assert.That(window.Fault!.Message).Contains("budget");
        await Assert.That(delivered).IsEqualTo(0);
        await Assert.That(host.RequestCloseCalls).IsEqualTo(1);
    }

    [Test]
    [Timeout(30_000)]
    public async Task CloseCalledOnTheDispatcherThread_DeliversTheRequestWithoutDeadlock(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        (InfiniFrameHostedWindow window, ScriptedWindowHostCore host) = await harness.CreateAttachedAsync(1);

        await Waits.WithinAsync(window.DispatchAsync(window.Close, cancellationToken: ct).AsTask(), "Close() on the dispatcher");
        await window.WaitForTeardownAsync(ct);
        await Assert.That(host.RequestCloseCalls).IsEqualTo(1);
    }
}
