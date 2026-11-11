// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Application;
using InfiniFrame.Hosting;
using InfiniFrame.Window.Hosting;

namespace InfiniTests.InfiniFrame.Hosting;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
// Lifecycle, threading and dispatch edge cases of the hosted window facade: synchronous close and waits, cancellation
// callbacks, synchronization contexts, sends, bounded close and disposal, late window hosts, readiness, close
// requests, dispatch budgets and resource bodies. HostedBlazorTests covers that Blazor output follows its page.
// Contract tests only; no browser.
[NotInParallel]
public sealed class HostedEdgeCaseTests {
    // ---- synchronous close and waits on the dispatcher thread --------------------------------------------------------
    [Test]
    [Timeout(30_000)]
    public async Task CloseFromAWindowCreatingHandler_DoesNotDeadlock_AndClosesOnceCreated(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        int closedCallbacks = 0;
        InfiniFrameHostedWindow window = harness.Build(builder => builder
            .RegisterWindowCreatingHandler(static creating => creating.Close())
            .RegisterWindowClosedHandler(_ => Interlocked.Increment(ref closedCallbacks)));

        await Waits.WithinAsync(harness.Runtime.CreateWindowsAsync([window], ct), "creation with Close() called from WindowCreating");
        await Waits.WithinAsync(window.WaitForTeardownAsync(ct).AsTask(), "teardown after the deferred close");
        ScriptedWindowHostCore host = harness.Host.Windows[0];
        await Assert.That(host.RequestCloseCalls).IsEqualTo(1);
        await Assert.That(host.DisposeCalls).IsEqualTo(1);
        await Assert.That(closedCallbacks).IsEqualTo(1);
    }

    [Test]
    [Timeout(30_000)]
    public async Task WaitForClose_OnTheDispatcherThread_IsRefusedInsteadOfDeadlocking(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        (InfiniFrameHostedWindow window, _) = await harness.CreateAsync();
        await window.WaitForReadyAsync(ct);

        Exception? refused = null;
        Task<InfiniFrameDispatchResult> onDispatcher = window.DispatchAsync(() => {
            try { window.WaitForClose(); }
            catch (InvalidOperationException exception) { refused = exception; }
        }, cancellationToken: ct).AsTask();
        await Waits.WithinAsync(onDispatcher, "WaitForClose on the dispatcher thread");
        await Assert.That(refused).IsNotNull();
        await Assert.That(refused!.Message).Contains("WaitForCloseAsync");

        // Off the dispatcher it still blocks until the window has closed.
        Task blocking = Task.Run(window.WaitForClose, ct);
        await Task.Delay(50, ct);
        await Assert.That(blocking.IsCompleted).IsFalse();
        await window.CloseAsync(ct);
        await Waits.WithinAsync(blocking, "WaitForClose after the close");
    }

    // ---- cancellation callbacks never run on, or throw into, provider threads ----------------------------------------
    [Test]
    [Timeout(30_000)]
    public async Task CancellationCallbacks_RunOffTheProviderThread_AndTheirFailuresDoNotEscapeDocumentAttached(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        (InfiniFrameHostedWindow window, ScriptedWindowHostCore host) = await harness.CreateAttachedAsync(1);
        var inFlight = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        int callbackThread = 0;
        host.SendOverride = (_, _, token) => {
            // Provider callbacks on the send token: one records its thread, one throws when the document retires.
            token.Register(static () => throw new InvalidOperationException("provider cancellation callback failed"));
            token.Register(() => callbackThread = Environment.CurrentManagedThreadId);
            inFlight.TrySetResult();
            return Task.Delay(Timeout.Infinite, token);
        };

        Task send = window.SendWebMessageAsync("in flight for document 1", ct).AsTask();
        await inFlight.Task.WaitAsync(TimeSpan.FromSeconds(5), ct);
        await Waits.WithinAsync(host.AttachAsync(2), "DocumentAttached returns normally to the provider");
        InfiniFrameHostDocumentRetiredException retired = await Expect.ThrowsAsync<InfiniFrameHostDocumentRetiredException>(() => send);

        await Assert.That(retired.Document.Value).IsEqualTo(1UL);
        await Assert.That(callbackThread).IsNotEqualTo(0);
        await Assert.That(callbackThread).IsNotEqualTo(harness.Host.LoopThreadId);
        await Assert.That(window.CurrentDocument).IsEqualTo(2UL);
        await Assert.That(window.Fault).IsNull();
        await Assert.That(window.LifecycleState).IsEqualTo(InfiniFrameWindowLifecycleState.Ready);
    }

    [Test]
    [Timeout(30_000)]
    public async Task ThrowingCancellationCallback_DuringTheTerminalTransition_StillTearsTheWindowDown(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        int closedCallbacks = 0;
        (InfiniFrameHostedWindow window, ScriptedWindowHostCore host) = await harness.CreateAttachedAsync(1, builder =>
            builder.RegisterWindowClosedHandler(_ => Interlocked.Increment(ref closedCallbacks)));
        var inFlight = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        host.SendOverride = (_, _, token) => {
            token.Register(static () => throw new InvalidOperationException("provider cancellation callback failed"));
            inFlight.TrySetResult();
            return Task.Delay(Timeout.Infinite, token);
        };

        Task send = window.SendWebMessageAsync("in flight when the window closes", ct).AsTask();
        await inFlight.Task.WaitAsync(TimeSpan.FromSeconds(5), ct);
        await Waits.WithinAsync(host.CloseNativelyAsync(), "Closed() returns normally to the provider");
        await Waits.WithinAsync(window.WaitForTeardownAsync(ct).AsTask(), "teardown despite the throwing callback");
        await Assert.ThrowsAsync<InfiniFrameHostDocumentRetiredException>(() => send);
        await Assert.That(closedCallbacks).IsEqualTo(1);
        await Assert.That(host.DisposeCalls).IsEqualTo(1);
    }

    // ---- provider calls never run under the caller's SynchronizationContext -----------------------------------------
    [Test]
    [Timeout(30_000)]
    public async Task ProviderAwaits_DoNotResumeOnTheCallersSynchronizationContext(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        (InfiniFrameHostedWindow window, ScriptedWindowHostCore host) = await harness.CreateAttachedAsync(1);
        var scripted = (ScriptedWindowHost)host;
        scripted.CapturesContext = true;
        host.SendOverride = async (document, message, _) => {
            await Task.Yield();// No ConfigureAwait(false): resumes on a captured context if one is current.
            host.Sent.Enqueue(new SentMessage(document.Value, message, Environment.CurrentManagedThreadId));
        };
        var stalled = new StalledSynchronizationContext();

        // Like Blazor code calling the facade: the dispatcher thread runs under a context that cannot run posts now.
        Task<InfiniFrameDispatchResult> call = window.DispatchAsync(() => {
            SynchronizationContext? previous = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(stalled);
            try {
                window.SetTitle("set under a context");
                window.SendWebMessage("sent under a context");
            }
            finally {
                SynchronizationContext.SetSynchronizationContext(previous);
            }
        }, cancellationToken: ct).AsTask();

        await Waits.WithinAsync(call, "synchronous provider calls made under a SynchronizationContext", 5_000);
        await Assert.That(scripted.Title).IsEqualTo("set under a context");
        await Assert.That(host.Sent.Single().Message).IsEqualTo("sent under a context");
        await Assert.That(stalled.Posts).IsEqualTo(0);
    }

    // ---- synchronous SendWebMessage stays fire-and-forget ----------------------------------------------------------
    [Test]
    [Timeout(30_000)]
    public async Task SynchronousSend_BeforeAnyDocumentAndDuringAReload_NeitherThrowsNorBlocks(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        (InfiniFrameHostedWindow window, ScriptedWindowHostCore host) = await harness.CreateAsync();
        await window.WaitForReadyAsync(ct);

        await Waits.WithinAsync(Task.Run(() => window.SendWebMessage("before any document"), ct), "send before any document", 2_000);
        await host.AttachAsync(1);
        await host.RetireAsync(1);// Reload: document 1 is gone and its replacement has not attached yet.
        await Waits.WithinAsync(Task.Run(() => window.SendWebMessage("during the reload"), ct), "send during the reload", 2_000);
        Task<InfiniFrameDispatchResult> fromHandler =
            window.DispatchAsync(() => window.SendWebMessage("during the reload, from the dispatcher"), cancellationToken: ct).AsTask();
        await Waits.WithinAsync(fromHandler, "send on the dispatcher thread during the reload", 2_000);
        await host.AttachAsync(2);
        await Waits.WithinAsync(Task.Run(() => window.SendWebMessage("after the reload"), ct), "send after the reload", 2_000);

        SentMessage[] sent = [.. host.Sent];
        await Assert.That(fromHandler.Result).IsEqualTo(InfiniFrameDispatchResult.Completed);
        await Assert.That(sent.Length).IsEqualTo(1);
        await Assert.That(sent[0].Document).IsEqualTo(2UL);
        await Assert.That(sent[0].Message).IsEqualTo("after the reload");
        await Assert.That(window.Fault).IsNull();
    }

    // ---- every provider wait during close and disposal is bounded --------------------------------------------------
    [Test]
    [Timeout(30_000)]
    public async Task ProviderThatNeverAcceptsClose_DisposalIsBounded_AndStillReleasesTheWindowHost(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync(closeTimeout: TimeSpan.FromSeconds(1));
        (InfiniFrameHostedWindow window, ScriptedWindowHostCore host) = await harness.CreateAsync();
        await window.WaitForReadyAsync(ct);
        host.HangRequestClose = true;

        await Waits.WithinAsync(window.DisposeAsync().AsTask(), "disposal while the provider never accepts the close", 10_000);
        await Assert.That(window.LifecycleState).IsEqualTo(InfiniFrameWindowLifecycleState.Disposed);
        await Assert.That(window.Fault).IsTypeOf<InfiniFrameHostOperationException>();
        await Assert.That(host.DisposeCalls).IsEqualTo(1);
    }

    // ---- a window host returned after the window ended is closed and released --------------------------------------
    [Test]
    [Timeout(30_000)]
    public async Task WindowHostReturnedAfterTeardownGaveUp_IsClosedAndDisposed(CancellationToken ct) {
        var gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        await using HostedHarness harness = await HostedHarness.StartAsync(new ScriptedHostProvider { CreateWindowGate = _ => gate.Task },
            closeTimeout: TimeSpan.FromSeconds(1));
        InfiniFrameHostedWindow window = harness.Build();
        Task creation = harness.Runtime.CreateWindowsAsync([window], ct);
        await Waits.UntilAsync(() => harness.Host.CreateWindowCalls == 1, "CreateWindowAsync entered");

        // Disposal gives up waiting for the slow provider and finishes the window.
        await Waits.WithinAsync(window.DisposeAsync().AsTask(), "disposal during a slow creation", 10_000);
        await Assert.That(window.LifecycleState).IsEqualTo(InfiniFrameWindowLifecycleState.Disposed);

        // The provider then returns its native window: it must not leak.
        gate.SetResult();
        await Waits.WithinAsync(creation, "creation returns after the window ended");
        await Waits.UntilAsync(() => harness.Host.Windows.Count == 1 && harness.Host.Windows[0].DisposeCalls == 1, "late window host disposed");
        await Assert.That(harness.Host.Windows[0].RequestCloseCalls).IsEqualTo(1);
    }

    // ---- ready only once Created() was reported and the window host exists -----------------------------------------
    [Test]
    [Timeout(30_000)]
    public async Task ReadyAndWindowCreated_WaitForTheWindowHost_WhenCreatedPrecedesTheCreateWindowResult(CancellationToken ct) {
        var createdReported = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var returnHost = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        await using HostedHarness harness = await HostedHarness.StartAsync(new ScriptedHostProvider {
            AfterCreatedGate = _ => {
                createdReported.TrySetResult();
                return returnHost.Task;
            }
        });
        int createdHandlerRuns = 0;
        Exception? handlerFailure = null;
        InfiniFrameHostedWindow window = harness.Build(builder => builder.RegisterWindowCreatedHandler(created => {
            Interlocked.Increment(ref createdHandlerRuns);
            try { created.SetTitle("set by WindowCreated"); }
            catch (Exception exception) { handlerFailure = exception; }
        }));

        Task creation = harness.Runtime.CreateWindowsAsync([window], ct);
        await createdReported.Task.WaitAsync(TimeSpan.FromSeconds(5), ct);
        await Task.Delay(100, ct);
        await harness.DrainDispatcherAsync();
        await Assert.That(window.WaitForReadyAsync(ct).AsTask().IsCompleted).IsFalse();
        await Assert.That(createdHandlerRuns).IsEqualTo(0);

        returnHost.SetResult();
        await Waits.WithinAsync(creation, "creation completes");
        await window.WaitForReadyAsync(ct);
        await harness.DrainDispatcherAsync();
        await Assert.That(createdHandlerRuns).IsEqualTo(1);
        await Assert.That(handlerFailure).IsNull();
        await Assert.That(((ScriptedWindowHost)harness.Host.Windows[0]).Title).IsEqualTo("set by WindowCreated");
    }

    // ---- no window is left stuck in CloseRequested ---------------------------------------------------------------
    [Test]
    [Timeout(30_000)]
    public async Task ThrowingClosingHandler_DoesNotCancelClosing_OnEitherClosePath(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        static WindowClosingResult Throwing(IInfiniFrameWindow _, EventArgs? __) => throw new InvalidOperationException("closing handler failed");
        (InfiniFrameHostedWindow applicationClosed, ScriptedWindowHostCore first) =
            await harness.CreateAsync(builder => builder.RegisterWindowClosingHandler(Throwing));
        (InfiniFrameHostedWindow nativelyClosed, ScriptedWindowHostCore second) =
            await harness.CreateAsync(builder => builder.RegisterWindowClosingHandler(Throwing));
        await applicationClosed.WaitForReadyAsync(ct);
        await nativelyClosed.WaitForReadyAsync(ct);

        await Waits.WithinAsync(applicationClosed.CloseAsync(ct).AsTask(), "application close with a failing Closing handler");
        await Assert.That(first.RequestCloseCalls).IsEqualTo(1);
        await Assert.That(applicationClosed.LifecycleState >= InfiniFrameWindowLifecycleState.NativeClosed).IsTrue();

        bool proceed = await second.Events.CloseRequestedAsync(ct);
        await Assert.That(proceed).IsTrue();
        await Assert.That(nativelyClosed.LifecycleState).IsEqualTo(InfiniFrameWindowLifecycleState.CloseRequested);
    }

    [Test]
    [Timeout(30_000)]
    public async Task ProviderAbandonedCloseRequest_LeavesTheWindowOpenAndReady(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        using var handlerEntered = new ManualResetEventSlim();
        using var releaseHandler = new ManualResetEventSlim();
        (InfiniFrameHostedWindow window, ScriptedWindowHostCore host) = await harness.CreateAsync(builder =>
            builder.RegisterWindowClosingHandler((_, _) => {
                handlerEntered.Set();
                releaseHandler.Wait(TimeSpan.FromSeconds(10));
                return WindowClosingResult.Close;
            }));
        await window.WaitForReadyAsync(ct);

        using var providerGivesUp = new CancellationTokenSource();
        Task<bool> request = host.Events.CloseRequestedAsync(providerGivesUp.Token).AsTask();
        await Waits.UntilAsync(() => handlerEntered.IsSet, "Closing handler running");
        await providerGivesUp.CancelAsync();
        Exception? outcome = null;
        try { await request; }
        catch (Exception exception) { outcome = exception; }
        await Assert.That(outcome is OperationCanceledException).IsTrue();
        await Assert.That(window.LifecycleState).IsEqualTo(InfiniFrameWindowLifecycleState.Ready);

        releaseHandler.Set();
        await harness.DrainDispatcherAsync();
        await Assert.That(window.LifecycleState).IsEqualTo(InfiniFrameWindowLifecycleState.Ready);
        await Assert.That(host.RequestCloseCalls).IsEqualTo(0);

        // The window stays fully usable: a later close works normally.
        await Waits.WithinAsync(window.CloseAsync(ct).AsTask(), "a later close");
        await Assert.That(host.RequestCloseCalls).IsEqualTo(1);
    }

    [Test]
    [Timeout(30_000)]
    public async Task CloseCommittedDuringCreation_IsDelivered_EvenWhenTheCallerStopsWaiting(CancellationToken ct) {
        var gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        await using HostedHarness harness = await HostedHarness.StartAsync(new ScriptedHostProvider { CreateWindowGate = _ => gate.Task });
        int closedCallbacks = 0;
        InfiniFrameHostedWindow window = harness.Build(builder => builder.RegisterWindowClosedHandler(_ => Interlocked.Increment(ref closedCallbacks)));
        Task creation = harness.Runtime.CreateWindowsAsync([window], ct);
        await Waits.UntilAsync(() => harness.Host.CreateWindowCalls == 1, "CreateWindowAsync entered");

        using var callerGivesUp = new CancellationTokenSource();
        Task close = window.CloseAsync(callerGivesUp.Token).AsTask();
        await Task.Delay(50, ct);
        await Assert.That(close.IsCompleted).IsFalse();
        await Assert.That(window.LifecycleState).IsEqualTo(InfiniFrameWindowLifecycleState.CloseRequested);
        await callerGivesUp.CancelAsync();
        Exception? outcome = null;
        try { await close; }
        catch (Exception exception) { outcome = exception; }
        await Assert.That(outcome is OperationCanceledException).IsTrue();

        gate.SetResult();
        await Waits.WithinAsync(creation, "creation completes");
        await Waits.WithinAsync(window.WaitForTeardownAsync(ct).AsTask(), "the committed close completes after creation");
        ScriptedWindowHostCore host = harness.Host.Windows[0];
        await Assert.That(host.RequestCloseCalls).IsEqualTo(1);
        await Assert.That(host.DisposeCalls).IsEqualTo(1);
        await Assert.That(closedCallbacks).IsEqualTo(1);
    }

    [Test]
    [Timeout(30_000)]
    public async Task CreatedDuringACloseThatIsThenCancelled_ReturnsTheWindowToReady(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync(new ScriptedHostProvider { AutoCreated = false });
        using var handlerEntered = new ManualResetEventSlim();
        using var releaseHandler = new ManualResetEventSlim();
        (InfiniFrameHostedWindow window, ScriptedWindowHostCore host) = await harness.CreateAsync(builder =>
            builder.RegisterWindowClosingHandler((_, _) => {
                handlerEntered.Set();
                releaseHandler.Wait(TimeSpan.FromSeconds(10));
                return WindowClosingResult.Cancel;
            }));
        await Assert.That(window.LifecycleState).IsEqualTo(InfiniFrameWindowLifecycleState.Creating);

        Task close = window.CloseAsync(ct).AsTask();
        await Waits.UntilAsync(() => handlerEntered.IsSet, "Closing handler running");
        await host.CreatedAsync();// Created() arrives while the close is being decided.
        releaseHandler.Set();
        await Assert.ThrowsAsync<InfiniFrameCloseRejectedException>(() => close);
        await window.WaitForReadyAsync(ct);
        await harness.DrainDispatcherAsync();
        await Assert.That(window.LifecycleState).IsEqualTo(InfiniFrameWindowLifecycleState.Ready);
    }

    // ---- application dispatches and provider data have separate budgets --------------------------------------------
    [Test]
    [Timeout(60_000)]
    public async Task ApplicationDispatchBurst_DoesNotFaultTheWindowThroughTheProviderLane(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        int delivered = 0;
        (InfiniFrameHostedWindow window, ScriptedWindowHostCore host) = await harness.CreateAttachedAsync(1, builder =>
            builder.RegisterWebMessageReceivedHandler((_, _) => Interlocked.Increment(ref delivered)));

        using var release = new ManualResetEventSlim();
        Task<InfiniFrameDispatchResult> blocker = window.DispatchAsync(() => release.Wait(ct), TimeSpan.FromSeconds(50), ct).AsTask();
        int burst = InfiniFrameHostedDispatcher.DefaultItemCapacity + 500;
        var dispatches = new List<Task<InfiniFrameDispatchResult>>(burst);
        for (int index = 0; index < burst; index++) dispatches.Add(window.DispatchAsync(static () => {}, TimeSpan.FromSeconds(50), ct).AsTask());
        await host.PostAsync(1, "__bwv:after the application burst");
        release.Set();
        await blocker;
        InfiniFrameDispatchResult[] results = await Task.WhenAll(dispatches);
        await harness.DrainDispatcherAsync();

        await Assert.That(results.All(static result => result == InfiniFrameDispatchResult.Completed)).IsTrue();
        await Assert.That(delivered).IsEqualTo(1);
        await Assert.That(window.Fault).IsNull();
    }

    [Test]
    [Timeout(60_000)]
    public async Task ProviderBurst_DoesNotMakeApplicationDispatchFail(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        int delivered = 0;
        (InfiniFrameHostedWindow window, ScriptedWindowHostCore host) = await harness.CreateAttachedAsync(1, builder =>
            builder.RegisterWebMessageReceivedHandler((_, _) => Interlocked.Increment(ref delivered)));

        using var release = new ManualResetEventSlim();
        Task<InfiniFrameDispatchResult> blocker = window.DispatchAsync(() => release.Wait(ct), TimeSpan.FromSeconds(50), ct).AsTask();
        int burst = InfiniFrameHostedDispatcher.DefaultItemCapacity - 1;
        for (int index = 0; index < burst; index++) await host.PostAsync(1, $"__bwv:m{index}");
        Task<InfiniFrameDispatchResult> dispatch = window.DispatchAsync(static () => {}, TimeSpan.FromSeconds(50), ct).AsTask();
        release.Set();
        await blocker;

        await Assert.That(await dispatch).IsEqualTo(InfiniFrameDispatchResult.Completed);
        await harness.DrainDispatcherAsync();
        await Assert.That(delivered).IsEqualTo(burst);
        await Assert.That(window.Fault).IsNull();
    }

    // ---- miscellaneous ---------------------------------------------------------------------------------------------
    [Test]
    [Timeout(30_000)]
    public async Task BodyWhoseLengthCannotBeRead_IsServedWithoutALength(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync();
        int handlerThread = 0;
        (_, ScriptedWindowHostCore host) = await harness.CreateAttachedAsync(1, builder => builder.RegisterCustomSchemeHandler("probe", (_, _) => {
            handlerThread = Environment.CurrentManagedThreadId;
            return (new LengthFailingStream(), "text/plain");
        }));

        await using InfiniFrameHostResourceResponse response = await host.Events.GetResourceAsync(
            new InfiniFrameHostResourceRequest { Method = "GET", Url = new Uri("probe://localhost/a.txt") }, ct);
        await Assert.That(response.StatusCode).IsEqualTo(200);
        await Assert.That(response.ContentLength).IsNull();
        await Assert.That(response.Body).IsNotNull();
        // Documented: handlers run on a thread-pool thread, never on the provider's calling thread.
        await Assert.That(handlerThread).IsNotEqualTo(harness.Host.LoopThreadId);
    }

    [Test]
    [Timeout(30_000)]
    public async Task InFlightSend_LeavesTheProviderBeforeItsWindowHostIsDisposed(CancellationToken ct) {
        await using HostedHarness harness = await HostedHarness.StartAsync(closeTimeout: TimeSpan.FromSeconds(10));
        (InfiniFrameHostedWindow window, ScriptedWindowHostCore host) = await harness.CreateAttachedAsync(1);
        var inFlight = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var finishSend = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        host.SendOverride = (_, _, _) => {
            inFlight.TrySetResult();
            return finishSend.Task;// A provider call that does not stop at once when its token is cancelled.
        };

        Task send = window.SendWebMessageAsync("slow to leave the provider", ct).AsTask();
        await inFlight.Task.WaitAsync(TimeSpan.FromSeconds(5), ct);
        await host.CloseNativelyAsync();
        await Task.Delay(200, ct);
        await Assert.That(host.DisposeCalls).IsEqualTo(0);

        finishSend.SetResult();
        await Waits.WithinAsync(send, "the in-flight send completes");
        await Waits.WithinAsync(window.WaitForTeardownAsync(ct).AsTask(), "teardown after the send left the provider");
        await Assert.That(host.DisposeCalls).IsEqualTo(1);
    }

    [Test]
    public async Task IncompatibleProvider_IsRejectedBeforeTheProcessIsLatched(CancellationToken ct) {
        InfiniFrameHostSelection.ResetForTests();
        var nextMajor = new ScriptedHostProvider { ContractVersion = new Version(InfiniFrameHostContract.MajorVersion + 1, 0) };
        await Assert.That(() => InfiniFrameApplication.CreateBuilder().UseHostProvider(nextMajor).WithWindow("main", HostedHarness.ConfigureDefaults).Build())
            .Throws<NotSupportedException>();
        await Assert.That(InfiniFrameHostSelection.Current).IsEqualTo(InfiniFrameHostSelection.Kind.None);
        await Assert.That(nextMajor.CreateHostCalls).IsEqualTo(0);

        var newerMinor = new ScriptedHostProvider {
            ContractVersion = new Version(InfiniFrameHostContract.MajorVersion, InfiniFrameHostContract.MinorVersion + 1)
        };
        await Assert.That(() => InfiniFrameApplication.CreateBuilder().UseHostProvider(newerMinor).Build()).Throws<NotSupportedException>();
        await Assert.That(InfiniFrameHostSelection.Current).IsEqualTo(InfiniFrameHostSelection.Kind.None);

        // The rejected provider left no latch behind: a compatible provider can still be selected.
        var compatible = new ScriptedHostProvider();
        InfiniFrameApplication application = InfiniFrameApplication.CreateBuilder()
            .UseHostProvider(compatible)
            .WithWindow("main", HostedHarness.ConfigureDefaults)
            .Build();
        await Assert.That(InfiniFrameHostSelection.Current).IsEqualTo(InfiniFrameHostSelection.Kind.Hosted);
        await application.DisposeAsync();
        InfiniFrameHostSelection.ResetForTests();
    }

    /// <summary>A context whose posts cannot run (as when its owning thread is blocked); counts attempts.</summary>
    private sealed class StalledSynchronizationContext : SynchronizationContext {
        private int _posts;
        internal int Posts => Volatile.Read(ref _posts);
        public override void Post(SendOrPostCallback d, object? state) => Interlocked.Increment(ref _posts);
        public override void Send(SendOrPostCallback d, object? state) => throw new InvalidOperationException("Send is not expected.");
        public override SynchronizationContext CreateCopy() => this;
    }

    private sealed class LengthFailingStream() : MemoryStream("abc"u8.ToArray()) {
        public override long Length => throw new IOException("the length is unavailable");
    }
}
