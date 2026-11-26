// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics;
using InfiniFrame.BlazorWebView;
using InfiniFrame.BlazorWebView.Hosted;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace InfiniTests.InfiniFrame.Hosting;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
// The retirement timeout branch: these tests shorten the coordinator's bounds and read its counters.
// A retired page's component disposal is suspended across the retirement bound; the coordinator's fallback releases the
// page scope; then the original disposal is resumed and completes, faults or is cancelled. Shutdown is also exercised in
// that interval. The supported teardown policy under test: after the bound, the page's remaining disposal work is not
// cancelled, and no longer awaited; its scope is released once, so disposal code that resumes later meets already-disposed
// scoped services; how that disposal ends is counted and logged; the current page is not affected. At shutdown the
// application does not wait for the coordinator (unchanged: the window-closed handler starts the manager's disposal without
// awaiting it), so Run() returns, and the application's singletons are disposed, while a retirement can still be running.
[NotInParallel]
public sealed class HostedRetirementTimeoutTests {
    private const string AlreadyDisposed = SuspendingDisposalProbe.AlreadyDisposed;

    [Test]
    [Timeout(60_000)]
    [Arguments(ResumedDisposal.Completes)]
    [Arguments(ResumedDisposal.Faults)]
    [Arguments(ResumedDisposal.IsCancelled)]
    public async Task SuspendedDisposal_PastTheBound_ReleasesTheScopeOnce_AndItsLateEndIsCounted(ResumedDisposal end, CancellationToken ct) {
        Run run = await StartAsync(ct, retirement: TimeSpan.FromMilliseconds(400));
        BlazorDocumentScenario s = run.Scenario;
        run.Gate.End = end;
        s.Answer("probe.value", static document => $"\"value-from-document-{document}\"");
        (IsolationProbe page1, IsolationProbe page2) = await run.ReloadAsync();
        TouchableScopeService page1Service = run.Gate.Services[0];

        // The bound passes: the fallback releases page 1's scope while that page's disposal is still suspended.
        await Waits.UntilAsync(() => run.Diagnostics is { DisposeTimeouts: 1, PageScopeFallbacks: 1 }, "the bound passed and the fallback ran", 10_000);
        await Waits.UntilAsync(() => page1Service.DisposeCalls == 1 && page1.Scope.DisposeCalls == 1, "the fallback disposed page 1's scoped services", 5_000);
        await Assert.That(run.Gate.Resumed).IsFalse();

        // Meanwhile the current page is unaffected: it renders, and its JS call is answered in its own document.
        int mark = s.Mark, batches = s.RenderBatches(2);
        Task<string>? value = null;
        await page2.OnDispatcher(() => {
            page2.Rerender("page-2-during-page-1-disposal");
            value = HostedDocumentIsolationTests.Call(() => page2.JS.InvokeAsync<string>("probe.value"));
        });
        await Waits.UntilAsync(() => s.RenderBatches(2) > batches && value!.IsCompleted, "page 2 rendered and its JS call was answered");
        await Assert.That(value!.Result).IsEqualTo("value-from-document-2");
        await Assert.That(s.SentSince(mark).All(static m => m.Document == 2)).IsTrue();

        // The suspended disposal resumes after its scope was released, and completes, faults or is cancelled.
        int beforeResume = s.Mark;
        long refusedBeforeResume = run.Diagnostics.RefusedOutbound;
        run.Gate.Release();
        await Waits.UntilAsync(() => run.Gate.Resumed && run.Diagnostics is var d && d.LateDisposeCompletions + d.LateDisposeFaults + d.LateDisposeCancellations == 1,
            "the resumed disposal's end was counted", 10_000);
        await Task.Delay(300, ct);// a second count, if there were one, would show up here
        await page2.OnDispatcher(static () => {});// whatever the resumed disposal queued on the dispatcher has run
        HostedDocumentDiagnostics after = run.Diagnostics;
        (string Category, LogLevel Level, string Message, Exception? Exception)[] lateFaultWarnings =
            [.. run.Logs.Entries.Where(static e => e.Level == LogLevel.Warning && e.Message.Contains("faulted after its bound", StringComparison.Ordinal))];
        int lateCancellationWarnings =
            run.Logs.Entries.Count(static e => e.Level == LogLevel.Warning && e.Message.Contains("was cancelled after its bound", StringComparison.Ordinal));
        Console.WriteLine(
            $"TIMEOUT-BRANCH (resumed disposal {end}): it found its scoped service {run.Gate.ScopedObservation} " +
            $"and an application singleton {run.Gate.SingletonObservation}; " +
            $"late-fault warnings: {lateFaultWarnings.Length} ({string.Join(", ", lateFaultWarnings.Select(static w => w.Exception?.GetType().Name ?? "no exception"))}); " +
            $"sent after the resume: {BlazorDocumentScenario.Describe(s.SentSince(beforeResume))}; warnings: {run.Logs.DescribeWarnings()}; {after}");
        await Assert.That(run.Gate.ScopedObservation).IsEqualTo(AlreadyDisposed);// the documented policy: not valid any more
        await Assert.That(run.Gate.SingletonObservation).IsEqualTo(SuspendingDisposalProbe.Usable);// the application still runs
        await Assert.That(after.DisposeTimeouts).IsEqualTo(1);
        await Assert.That(after.ManagersDisposed).IsEqualTo(0);// reported as timed out, not as disposed within the bound
        await Assert.That(after.DisposeFailures).IsEqualTo(0);// a late end is not reported as a failure within the bound
        await Assert.That(after.LateDisposeCompletions).IsEqualTo(end == ResumedDisposal.Completes ? 1 : 0);
        await Assert.That(after.LateDisposeFaults).IsEqualTo(end == ResumedDisposal.Faults ? 1 : 0);
        await Assert.That(after.LateDisposeCancellations).IsEqualTo(end == ResumedDisposal.IsCancelled ? 1 : 0);
        await Assert.That(lateFaultWarnings.Length).IsEqualTo(end == ResumedDisposal.Faults ? 1 : 0);
        await Assert.That(lateCancellationWarnings).IsEqualTo(end == ResumedDisposal.IsCancelled ? 1 : 0);
        if (end == ResumedDisposal.Faults)
            await Assert.That(lateFaultWarnings[0].Exception is InvalidOperationException { Message: "the resumed disposal failed" }).IsTrue();
        await Assert.That(page1Service.DisposeCalls).IsEqualTo(1);// at most once: the framework's own later scope disposal was a no-op
        await Assert.That(page1.Scope.DisposeCalls).IsEqualTo(1);
        await Assert.That(after.PageScopeFallbacks).IsEqualTo(1);
        await Assert.That(page2.Scope.DisposeCalls).IsEqualTo(0);
        // Nothing at all was sent after the resume (page 2 is idle): a failed disposal's error notice for the retired
        // document was refused (counted), not sent to any document.
        await Assert.That(s.SentSince(beforeResume).Length).IsEqualTo(0);
        await Assert.That(after.RefusedOutbound - refusedBeforeResume).IsEqualTo(end == ResumedDisposal.Completes ? 0L : 1L);

        await s.CloseAsync(ct);
        await Waits.UntilAsync(() => run.Diagnostics.ManagersDisposed == 1, "page 2's manager disposed within its bound at close", 10_000);
        HostedDocumentDiagnostics closed = run.Diagnostics;
        await Assert.That(closed.DisposeTimeouts).IsEqualTo(1);
        await Assert.That(closed.ShutdownTimeouts).IsEqualTo(0);
        await Assert.That(closed.LiveManagers).IsEqualTo(0);
        await Assert.That(run.Gate.Services[1].DisposeCalls).IsEqualTo(1);
    }

    [Test]
    [Timeout(60_000)]
    public async Task AComponentsOwnTimeoutException_WithinTheBound_IsReportedAsAFailure_NotAsTheBound(CancellationToken ct) {
        Run run = await StartAsync(ct, retirement: TimeSpan.FromSeconds(5));
        BlazorDocumentScenario s = run.Scenario;
        run.Gate.FailAtOnceWithTimeout = true;// page 1's component disposal throws its own TimeoutException at once
        (IsolationProbe page1, _) = await run.ReloadAsync();
        await Waits.UntilAsync(() => run.Diagnostics.PageScopeFallbacks == 1 && page1.Scope.DisposeCalls == 1, "page 1's scope released by the fallback", 10_000);
        await Task.Delay(300, ct);
        HostedDocumentDiagnostics d = run.Diagnostics;
        string warnings = run.Logs.DescribeWarnings();
        Console.WriteLine($"TIMEOUT-BRANCH a component's own TimeoutException within the 5 s bound: {d}; warnings: {warnings}");
        await Assert.That(d.DisposeFailures).IsEqualTo(1);// it failed within the bound
        await Assert.That(d.DisposeTimeouts).IsEqualTo(0);// the bound did not pass
        await Assert.That(d.LateDisposeFaults).IsEqualTo(0);// and it is not a late end
        await Assert.That(d.LateDisposeCompletions).IsEqualTo(0);
        await Assert.That(d.LateDisposeCancellations).IsEqualTo(0);
        await Assert.That(d.ManagersDisposed).IsEqualTo(0);
        await Assert.That(warnings.Contains("still disposing after", StringComparison.Ordinal)).IsFalse();
        await Assert.That(run.Logs.Entries.Any(static e => e.Level == LogLevel.Warning
            && e.Message.Contains("Disposing the Blazor page of retired hosted document 1 failed", StringComparison.Ordinal)
            && e.Exception is TimeoutException { Message: SuspendingDisposalProbe.OwnTimeout })).IsTrue();
        await s.CloseAsync(ct);
    }

    [Test]
    [Timeout(60_000)]
    [Arguments(false)]
    [Arguments(true)]
    public async Task ShutdownWhileTheDisposalIsSuspended_DoesNotWaitForIt_AndEachScopeIsReleasedOnce(bool afterTheBound, CancellationToken ct) {
        TimeSpan retirement = afterTheBound ? TimeSpan.FromMilliseconds(400) : TimeSpan.FromSeconds(2);
        TimeSpan shutdown = TimeSpan.FromSeconds(4);
        Run run = await StartAsync(ct, retirement, shutdown);
        BlazorDocumentScenario s = run.Scenario;
        (IsolationProbe page1, IsolationProbe page2) = await run.ReloadAsync();
        if (afterTheBound) {
            // Past the bound: page 1's retirement already ended (its scope released) while its disposal is still suspended.
            await Waits.UntilAsync(() => page1.Scope.DisposeCalls == 1, "page 1's scope released at its bound", 10_000);
        }
        HostedDocumentDiagnostics atClose = run.Diagnostics;
        await Assert.That(atClose.DisposeTimeouts).IsEqualTo(afterTheBound ? 1 : 0);
        await Assert.That(run.Gate.Resumed).IsFalse();

        var close = Stopwatch.StartNew();
        await s.CloseAsync(ct);// returns once Run() returned
        long runReturnedMs = close.ElapsedMilliseconds;
        HostedDocumentDiagnostics atRunEnd = run.Diagnostics;
        int page1ScopeAtRunEnd = page1.Scope.DisposeCalls, singletonAtRunEnd = run.Singleton.DisposeCalls;

        await Waits.UntilAsync(() => page1.Scope.DisposeCalls == 1 && page2.Scope.DisposeCalls == 1, "both page scopes released", 10_000);
        await Waits.UntilAsync(() => run.Diagnostics.ManagersDisposed == 1, "page 2's manager disposed", 10_000);
        // Wait past the coordinator's own shutdown bound: had its wait not ended by then, ShutdownTimeouts would be 1.
        TimeSpan pastTheShutdownBound = shutdown + TimeSpan.FromMilliseconds(500) - close.Elapsed;
        if (pastTheShutdownBound > TimeSpan.Zero) await Task.Delay(pastTheShutdownBound, ct);
        HostedDocumentDiagnostics closed = run.Diagnostics;
        Console.WriteLine(
            $"TIMEOUT-BRANCH shutdown {(afterTheBound ? "after" : "before")} the {retirement.TotalMilliseconds} ms bound, while the disposal was suspended: " +
            $"Run() returned {runReturnedMs} ms after the close request; at that moment page 1's scope disposals: {page1ScopeAtRunEnd}, " +
            $"application singleton disposals: {singletonAtRunEnd}, {atRunEnd}; {close.ElapsedMilliseconds} ms after the close request {closed}; " +
            $"warnings: {run.Logs.DescribeWarnings()}");
        // Run() does not wait for the coordinator (unchanged behaviour): by the time it returns, the application's root
        // provider has disposed its singletons, and page 1's retirement may still be running.
        await Assert.That(singletonAtRunEnd).IsEqualTo(1);
        if (!afterTheBound) {
            await Assert.That(atRunEnd.DisposeTimeouts).IsEqualTo(0);// still inside its 2 s bound when Run() returned
            await Assert.That(page1ScopeAtRunEnd).IsEqualTo(0);
        }
        // The coordinator's own shutdown wait (in the background) ended within its 4 s bound; each scope released once.
        await Assert.That(closed.ShutdownTimeouts).IsEqualTo(0);
        await Assert.That(closed.DisposeTimeouts).IsEqualTo(1);
        await Assert.That(closed.PageScopeFallbacks).IsEqualTo(1);
        await Assert.That(closed.ManagersDisposed).IsEqualTo(1);// page 2 only; page 1 stays reported as timed out
        await Assert.That(closed.LiveManagers).IsEqualTo(0);
        await Assert.That(run.Gate.Resumed).IsFalse();

        // The suspended disposal resumes after the application shut down. Its end is still counted.
        run.Gate.Release();
        await Waits.UntilAsync(() => run.Diagnostics.LateDisposeCompletions == 1, "the late completion was counted after shutdown", 10_000);
        Console.WriteLine(
            $"TIMEOUT-BRANCH resumed after shutdown: scoped service {run.Gate.ScopedObservation}, application singleton {run.Gate.SingletonObservation}; {run.Diagnostics}");
        await Assert.That(run.Gate.ScopedObservation).IsEqualTo(AlreadyDisposed);
        await Assert.That(run.Gate.SingletonObservation).IsEqualTo(AlreadyDisposed);
        await Assert.That(run.Gate.Services[0].DisposeCalls).IsEqualTo(1);
        await Assert.That(run.Gate.Services[1].DisposeCalls).IsEqualTo(1);
        await Assert.That(page1.Scope.DisposeCalls).IsEqualTo(1);
        await Assert.That(run.Diagnostics.LateDisposeFaults).IsEqualTo(0);
        await Assert.That(run.Diagnostics.LateDisposeCancellations).IsEqualTo(0);
    }

    [Test]
    [Timeout(60_000)]
    public async Task TheCoordinatorsShutdownWait_StopsAtItsBound_AndTheRetirementStillEndsOnce(CancellationToken ct) {
        Run run = await StartAsync(ct, retirement: TimeSpan.FromSeconds(2), shutdown: TimeSpan.FromMilliseconds(300));
        BlazorDocumentScenario s = run.Scenario;
        (IsolationProbe page1, _) = await run.ReloadAsync();

        await s.CloseAsync(ct);
        // The coordinator's shutdown wait (300 ms here) stops while page 1's retirement is still inside its 2 s bound.
        await Waits.UntilAsync(() => run.Diagnostics.ShutdownTimeouts == 1, "the coordinator's shutdown wait reached its bound", 10_000);
        HostedDocumentDiagnostics atShutdownBound = run.Diagnostics;
        int page1ScopeAtShutdownBound = page1.Scope.DisposeCalls;
        await Assert.That(atShutdownBound.DisposeTimeouts).IsEqualTo(0);
        await Assert.That(page1ScopeAtShutdownBound).IsEqualTo(0);// the retirement, no longer awaited by shutdown, is still running

        // That retirement still reaches its own bound later, releases page 1's scope once, and its late end is counted.
        await Waits.UntilAsync(() => page1.Scope.DisposeCalls == 1, "the retirement released page 1's scope after shutdown", 10_000);
        run.Gate.Release();
        await Waits.UntilAsync(() => run.Diagnostics.LateDisposeCompletions == 1, "the late completion was counted", 10_000);
        HostedDocumentDiagnostics end = run.Diagnostics;
        Console.WriteLine(
            $"TIMEOUT-BRANCH coordinator shutdown bound (300 ms) shorter than the retirement bound (2000 ms): at the shutdown bound {atShutdownBound}; " +
            $"at the end {end}; warnings: {run.Logs.DescribeWarnings()}");
        await Assert.That(end.DisposeTimeouts).IsEqualTo(1);
        await Assert.That(end.PageScopeFallbacks).IsEqualTo(1);
        await Assert.That(end.ShutdownTimeouts).IsEqualTo(1);
        await Assert.That(run.Gate.ScopedObservation).IsEqualTo(AlreadyDisposed);
        await Assert.That(run.Gate.Services[0].DisposeCalls).IsEqualTo(1);
        await Assert.That(page1.Scope.DisposeCalls).IsEqualTo(1);
    }

    [Test]
    [Timeout(60_000)]
    public async Task TheBounds_KeepTheirProductionDefaults_AndRefuseNonPositiveValues(CancellationToken ct) {
        var probe = await BlazorDocumentScenario.StartAsync(ct);
        HostedDocumentManagers hosted = ((InfiniFrameWebViewManager)probe.Window.ServiceProvider!.GetRequiredService<IInfiniFrameWebViewManager>()).HostedDocuments;
        await Assert.That(hosted.RetirementTimeout).IsEqualTo(TimeSpan.FromSeconds(10));
        await Assert.That(hosted.ShutdownTimeout).IsEqualTo(TimeSpan.FromSeconds(15));
        await Assert.That(HostedDocumentIsolationTests.Capture(() => hosted.RetirementTimeout = TimeSpan.Zero)).IsEqualTo("threw ArgumentOutOfRangeException");
        await Assert.That(HostedDocumentIsolationTests.Capture(() => hosted.ShutdownTimeout = TimeSpan.FromMilliseconds(-1))).IsEqualTo("threw ArgumentOutOfRangeException");
        await Assert.That(hosted.RetirementTimeout).IsEqualTo(TimeSpan.FromSeconds(10));
        await Assert.That(hosted.ShutdownTimeout).IsEqualTo(TimeSpan.FromSeconds(15));
        await probe.CloseAsync(ct);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------------------------------------------------
    private static async Task<Run> StartAsync(CancellationToken ct, TimeSpan retirement, TimeSpan? shutdown = null) {
        var logs = new CapturedLogs();
        BlazorDocumentScenario s = await BlazorDocumentScenario.StartAsync(ct,
            static blazor => blazor.RootComponents.Add<SuspendingDisposalProbe>("#suspending-probe"),
            services => services
                .AddSingleton<DisposalGate>()
                .AddSingleton<ApplicationSingletonProbe>()
                .AddScoped<TouchableScopeService>()
                .AddLogging(logging => logging.AddProvider(logs)));
        IServiceProvider services = s.Window.ServiceProvider!;
        var manager = (InfiniFrameWebViewManager)services.GetRequiredService<IInfiniFrameWebViewManager>();
        HostedDocumentManagers hosted = manager.HostedDocuments;
        hosted.RetirementTimeout = retirement;
        if (shutdown is {} bound) hosted.ShutdownTimeout = bound;
        return new Run(s, manager, services.GetRequiredService<DisposalGate>(), services.GetRequiredService<ApplicationSingletonProbe>(), logs);
    }

    private sealed record Run(
        BlazorDocumentScenario Scenario,
        InfiniFrameWebViewManager Manager,
        DisposalGate Gate,
        ApplicationSingletonProbe Singleton,
        CapturedLogs Logs
    ) {
        internal HostedDocumentDiagnostics Diagnostics => Manager.GetHostedDocumentDiagnostics();

        /// <summary>Loads page 1, replaces it with page 2, and waits until page 1's component disposal started.</summary>
        internal async Task<(IsolationProbe Page1, IsolationProbe Page2)> ReloadAsync() {
            IsolationProbe page1 = await Scenario.LoadPageAsync(1);
            await Waits.UntilAsync(() => Gate.Services.Length == 1, "page 1's suspending root initialized");
            await Scenario.Host.RetireAsync(1);
            IsolationProbe page2 = await Scenario.LoadPageAsync(2);
            await Waits.UntilAsync(() => Gate.Services.Length == 2, "page 2's suspending root initialized");
            await Gate.Started.WaitAsync(TimeSpan.FromSeconds(10));
            return (page1, page2);
        }
    }
}

/// <summary>How the suspended disposal ends once it resumes.</summary>
public enum ResumedDisposal {
    Completes,
    Faults,
    IsCancelled
}

/// <summary>Holds the first page's component disposal across the retirement bound, then lets it resume (test-owned).</summary>
public sealed class DisposalGate {
    private readonly TaskCompletionSource _started = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly TaskCompletionSource _released = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly List<TouchableScopeService> _services = [];
    private int _suspensions = 1;
    private string? _scopedObservation;
    private string? _singletonObservation;
    private int _resumed;

    public ResumedDisposal End { get; set; }

    /// <summary>The first disposal throws its own <see cref="TimeoutException" /> at once instead of being suspended.</summary>
    public bool FailAtOnceWithTimeout { get; set; }

    public Task Started => _started.Task;
    public Task Released => _released.Task;
    public bool Resumed => Volatile.Read(ref _resumed) != 0;

    /// <summary>What the resumed disposal found when it used its page's scoped service.</summary>
    public string? ScopedObservation => Volatile.Read(ref _scopedObservation);

    /// <summary>What the resumed disposal found when it used an application singleton.</summary>
    public string? SingletonObservation => Volatile.Read(ref _singletonObservation);

    /// <summary>Each page's scoped service, in page order.</summary>
    public TouchableScopeService[] Services {
        get {
            lock (_services) return [.. _services];
        }
    }

    public void Register(TouchableScopeService service) {
        lock (_services) _services.Add(service);
    }

    /// <summary>True only for the first disposal: later pages dispose normally.</summary>
    public bool TryTakeSuspension() => Interlocked.Decrement(ref _suspensions) >= 0;

    public void MarkStarted() => _started.TrySetResult();
    public void Release() => _released.TrySetResult();

    public void MarkResumed(string scoped, string singleton) {
        Volatile.Write(ref _scopedObservation, scoped);
        Volatile.Write(ref _singletonObservation, singleton);
        Volatile.Write(ref _resumed, 1);
    }
}

/// <summary>A scoped service that, like real ones, refuses use after its scope disposed it.</summary>
public sealed class TouchableScopeService : IDisposable {
    private int _disposeCalls;
    public int DisposeCalls => Volatile.Read(ref _disposeCalls);
    public void Touch() => ObjectDisposedException.ThrowIf(DisposeCalls > 0, this);
    public void Dispose() => Interlocked.Increment(ref _disposeCalls);
}

/// <summary>An application singleton that refuses use after the application's root provider disposed it.</summary>
public sealed class ApplicationSingletonProbe : IDisposable {
    private int _disposeCalls;
    public int DisposeCalls => Volatile.Read(ref _disposeCalls);
    public void Touch() => ObjectDisposedException.ThrowIf(DisposeCalls > 0, this);
    public void Dispose() => Interlocked.Increment(ref _disposeCalls);
}

/// <summary>A second root whose disposal waits on the gate and then uses its page's scoped service and a singleton.</summary>
public sealed class SuspendingDisposalProbe : ComponentBase, IAsyncDisposable {
    internal const string Usable = "usable";
    internal const string AlreadyDisposed = "already disposed (ObjectDisposedException)";
    internal const string OwnTimeout = "the component's own operation timed out";

    [Inject] public DisposalGate Gate { get; set; } = default!;
    [Inject] public TouchableScopeService Service { get; set; } = default!;
    [Inject] public ApplicationSingletonProbe Singleton { get; set; } = default!;

    protected override void OnInitialized() => Gate.Register(Service);
    protected override void BuildRenderTree(RenderTreeBuilder builder) => builder.AddMarkupContent(0, "<p>suspending probe</p>");

    public async ValueTask DisposeAsync() {
        if (!Gate.TryTakeSuspension()) return;
        Gate.MarkStarted();
        if (Gate.FailAtOnceWithTimeout) throw new TimeoutException(OwnTimeout);
        await Gate.Released;// suspended across the retirement bound
        Gate.MarkResumed(Use(Service.Touch), Use(Singleton.Touch));
        switch (Gate.End) {
            case ResumedDisposal.Faults:
                throw new InvalidOperationException("the resumed disposal failed");
            case ResumedDisposal.IsCancelled:
                // As a JS call in a disposal path ends when its caller's token fires.
                throw new TaskCanceledException("the resumed disposal's JS call was cancelled by its token");
        }
    }

    private static string Use(Action touch) {
        try {
            touch();
            return Usable;
        }
        catch (ObjectDisposedException) {
            return AlreadyDisposed;
        }
    }
}

/// <summary>Captures the application's log entries (Information and above, the default minimum level).</summary>
internal sealed class CapturedLogs : ILoggerProvider {
    private readonly List<(string Category, LogLevel Level, string Message, Exception? Exception)> _entries = [];

    public (string Category, LogLevel Level, string Message, Exception? Exception)[] Entries {
        get {
            lock (_entries) return [.. _entries];
        }
    }

    /// <summary>The Warning-or-higher entries, briefly (for the test's record).</summary>
    public string DescribeWarnings() {
        string[] warnings = [.. Entries.Where(static e => e.Level >= LogLevel.Warning).Select(static e => $"[{e.Level}: {e.Message}{(e.Exception is {} x ? $" ({x.GetType().Name})" : "")}]")];
        return warnings.Length == 0 ? "(none)" : string.Join(" ", warnings);
    }

    public ILogger CreateLogger(string categoryName) => new Logger(this, categoryName);
    public void Dispose() {}

    private sealed class Logger(CapturedLogs owner, string category) : ILogger {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) {
            lock (owner._entries) owner._entries.Add((category, logLevel, formatter(state, exception), exception));
        }
    }
}
