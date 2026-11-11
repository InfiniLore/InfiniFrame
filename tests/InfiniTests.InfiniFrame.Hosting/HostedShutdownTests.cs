// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Threading.Tasks.Sources;
using InfiniFrame.Hosting;
using InfiniFrame.Window.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InfiniTests.InfiniFrame.Hosting;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[NotInParallel]
public sealed class HostedShutdownTests {
    [Test]
    [Timeout(15_000)]
    public async Task OrderlyShutdown_CompletesBeforeExactlyOneDisposal(CancellationToken ct) {
        var host = new ShutdownHost();
        await using var run = await ShutdownRun.StartAsync(host);
        Task disposing = run.Runtime.DisposeAsync().AsTask();
        await host.ShutdownStarted.Task.WaitAsync(ct);
        await Assert.That(host.DisposeCalls).IsEqualTo(0);
        host.Shutdown.Complete();
        await host.DisposeStarted.Task.WaitAsync(ct);
        await Assert.That(host.Shutdown.Consumptions).IsEqualTo(1);
        host.Disposal.Complete();
        await disposing.WaitAsync(ct);
        await run.Runtime.DisposeAsync();
        await Assert.That(host.ShutdownCalls).IsEqualTo(1);
        await Assert.That(host.DisposeCalls).IsEqualTo(1);
        await Assert.That(host.Disposal.Consumptions).IsEqualTo(1);
        await Assert.That(run.Logs.Entries.Length).IsEqualTo(0);
    }

    [Test]
    [Timeout(15_000)]
    public async Task ShutdownTimeout_CancelsTheProvidersToken(CancellationToken ct) {
        var host = new ShutdownHost();
        host.OnShutdown = token => token.Register(() => host.Shutdown.Complete());
        host.OnDispose = () => host.Disposal.Complete();
        await using var run = await ShutdownRun.StartAsync(host, TimeSpan.FromMilliseconds(100));
        await run.Runtime.DisposeAsync().AsTask().WaitAsync(TimeSpan.FromSeconds(3), ct);
        await Assert.That(host.ShutdownToken.CanBeCanceled).IsTrue();
        await Assert.That(host.ShutdownToken.IsCancellationRequested).IsTrue();
        await Waits.UntilAsync(() => host.Shutdown.Consumptions == 1, "cooperative shutdown completed");
        await Assert.That(host.DisposeCalls).IsEqualTo(1);
    }

    [Test]
    [Timeout(15_000)]
    [Arguments(false)]
    [Arguments(true)]
    public async Task ShutdownIgnoringCancellation_CanFinishAfterDisposalStarts_AndItsLateFaultIsLogged(bool fault, CancellationToken ct) {
        var host = new ShutdownHost();
        host.OnDispose = () => host.Disposal.Complete();
        await using var run = await ShutdownRun.StartAsync(host, TimeSpan.FromMilliseconds(100));
        Task disposing = run.Runtime.DisposeAsync().AsTask();
        try {
            await host.DisposeStarted.Task.WaitAsync(TimeSpan.FromSeconds(3), ct);
            await disposing.WaitAsync(TimeSpan.FromSeconds(3), ct);
            await Assert.That(host.Shutdown.IsCompleted).IsFalse();
            // A late shutdown may still use its cancelled token after host disposal has returned.
            _ = host.ShutdownToken.WaitHandle;
            if (fault) host.Shutdown.Fail(new InvalidOperationException("late shutdown failure"));
            else host.Shutdown.Complete();
            await Waits.UntilAsync(() => host.Shutdown.Consumptions == 1, "late shutdown ValueTask consumed once");
            if (fault) await Waits.UntilAsync(() => HasWarning(run, "late shutdown failure"), "late shutdown fault logged", 1_000);
            await Assert.That(HasWarning(run, "late shutdown failure")).IsEqualTo(fault);
            await Assert.That(host.DisposeCalls).IsEqualTo(1);
            await Assert.That(host.Disposal.Consumptions).IsEqualTo(1);
        }
        finally {
            if (!host.Shutdown.IsCompleted) host.Shutdown.Complete();
            await disposing.WaitAsync(ct);
        }
    }

    [Test]
    [Timeout(15_000)]
    public async Task DisposalTimeout_ObservesAndLogsTheLateDisposalFault(CancellationToken ct) {
        var host = new ShutdownHost();
        host.Shutdown.Complete();
        await using var run = await ShutdownRun.StartAsync(host, TimeSpan.FromMilliseconds(100));
        try {
            await run.Runtime.DisposeAsync().AsTask().WaitAsync(TimeSpan.FromSeconds(3), ct);
            await Assert.That(host.Disposal.IsCompleted).IsFalse();
            host.Disposal.Fail(new InvalidOperationException("late disposal failure"));
            await Waits.UntilAsync(() => HasWarning(run, "late disposal failure"), "late disposal fault logged", 1_000);
            await Assert.That(host.Disposal.Consumptions).IsEqualTo(1);
            await Assert.That(host.DisposeCalls).IsEqualTo(1);
        }
        finally {
            if (!host.Disposal.IsCompleted) host.Disposal.Complete();
        }
    }

    [Test]
    [Timeout(15_000)]
    public async Task BlockingCancellationCallback_DoesNotBlockDisposal_AndKeepsTheSourceAlive(CancellationToken ct) {
        using var releaseCallback = new ManualResetEventSlim();
        var callbackStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var callbackFinished = new TaskCompletionSource<Exception?>(TaskCreationOptions.RunContinuationsAsynchronously);
        var host = new ShutdownHost();
        host.OnShutdown = token => token.Register(() => {
            callbackStarted.TrySetResult();
            releaseCallback.Wait(ct);
            try {
                // Shutdown has already completed by this point. The CTS must survive until this callback exits too.
                _ = token.WaitHandle;
                callbackFinished.TrySetResult(null);
            }
            catch (Exception exception) { callbackFinished.TrySetResult(exception); }
            throw new InvalidOperationException("late cancellation callback failure");
        });
        host.OnDispose = () => { host.Shutdown.Complete(); host.Disposal.Complete(); };
        await using var run = await ShutdownRun.StartAsync(host, TimeSpan.FromMilliseconds(100));
        try {
            await run.Runtime.DisposeAsync().AsTask().WaitAsync(TimeSpan.FromSeconds(3), ct);
            await callbackStarted.Task.WaitAsync(TimeSpan.FromSeconds(1), ct);
            await Assert.That(host.DisposeCalls).IsEqualTo(1);
            await Waits.UntilAsync(() => host.Shutdown.Consumptions == 1, "shutdown completed with a callback still blocked");
        }
        finally { releaseCallback.Set(); }
        await Assert.That(await callbackFinished.Task.WaitAsync(ct)).IsNull();
        await Waits.UntilAsync(() => HasWarning(run, "late cancellation callback failure"), "late cancellation fault logged", 1_000);
    }

    [Test]
    [Timeout(15_000)]
    public async Task ProvidersOwnTimeoutException_IsAnOrdinaryFailure(CancellationToken ct) {
        var host = new ShutdownHost();
        host.Shutdown.Fail(new TimeoutException("provider shutdown failure"));
        host.Disposal.Fail(new TimeoutException("provider disposal failure"));
        await using var run = await ShutdownRun.StartAsync(host);
        await run.Runtime.DisposeAsync().AsTask().WaitAsync(ct);
        await Assert.That(HasWarning(run, "provider shutdown failure")).IsTrue();
        await Assert.That(HasWarning(run, "provider disposal failure")).IsTrue();
        await Assert.That(host.ShutdownToken.IsCancellationRequested).IsFalse();
        await Assert.That(run.Logs.Entries.Length).IsEqualTo(2);
    }

    private static bool HasWarning(ShutdownRun run, string message) => run.Logs.Entries.Any(e =>
        e.Level == LogLevel.Warning && e.Exception?.GetBaseException().Message == message);

    private sealed class ShutdownRun(InfiniFrameHostedRuntime runtime, ServiceProvider services, CapturedLogs logs) : IAsyncDisposable {
        internal InfiniFrameHostedRuntime Runtime => runtime;
        internal CapturedLogs Logs => logs;

        internal static async Task<ShutdownRun> StartAsync(ShutdownHost host, TimeSpan? timeout = null) {
            var logs = new CapturedLogs();
            ServiceProvider services = new ServiceCollection().AddLogging(builder => builder.AddProvider(logs)).BuildServiceProvider();
            var runtime = new InfiniFrameHostedRuntime(new ShutdownProvider(host), Guid.NewGuid()) {
                ShutdownTimeout = timeout ?? TimeSpan.FromSeconds(5)
            };
            await runtime.StartAsync(services, CancellationToken.None);
            return new ShutdownRun(runtime, services, logs);
        }

        public async ValueTask DisposeAsync() {
            await runtime.DisposeAsync();
            await services.DisposeAsync();
        }
    }

    private sealed class ShutdownProvider(ShutdownHost host) : IInfiniFrameHostProvider {
        public string DisplayName => "shutdown test host";
        public Version ContractVersion => new(InfiniFrameHostContract.MajorVersion, InfiniFrameHostContract.MinorVersion);
        public InfiniFrameHostCapabilities Capabilities => default;
        public IInfiniFrameHost CreateHost(InfiniFrameHostCreateContext context) => host;
    }

    private sealed class ShutdownHost : IInfiniFrameHost {
        internal SingleConsumptionOperation Shutdown { get; } = new();
        internal SingleConsumptionOperation Disposal { get; } = new();
        internal TaskCompletionSource ShutdownStarted { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        internal TaskCompletionSource DisposeStarted { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        internal Action<CancellationToken>? OnShutdown { get; set; }
        internal Action? OnDispose { get; set; }
        internal CancellationToken ShutdownToken { get; private set; }
        internal int ShutdownCalls { get; private set; }
        internal int DisposeCalls { get; private set; }
        public Task Completion => Task.CompletedTask;
        public ValueTask StartAsync(InfiniFrameHostStartContext context, CancellationToken cancellationToken) => ValueTask.CompletedTask;
        public ValueTask<IInfiniFrameWindowHost> CreateWindowAsync(InfiniFrameHostWindowRequest request,
            IInfiniFrameWindowHostEvents events, CancellationToken cancellationToken) => throw new NotSupportedException();

        public ValueTask ShutdownAsync(CancellationToken cancellationToken) {
            ShutdownCalls++;
            ShutdownToken = cancellationToken;
            OnShutdown?.Invoke(cancellationToken);
            ShutdownStarted.TrySetResult();
            return Shutdown.Task;
        }

        public ValueTask DisposeAsync() {
            DisposeCalls++;
            OnDispose?.Invoke();
            DisposeStarted.TrySetResult();
            return Disposal.Task;
        }
    }

    // A provider is allowed to return a ValueTask backed by a single-consumption source, rather than a reusable Task.
    private sealed class SingleConsumptionOperation : IValueTaskSource {
        private ManualResetValueTaskSourceCore<bool> _source = new() { RunContinuationsAsynchronously = true };
        private int _consumptions;
        internal int Consumptions => Volatile.Read(ref _consumptions);
        internal bool IsCompleted => _source.GetStatus(_source.Version) != ValueTaskSourceStatus.Pending;
        internal ValueTask Task => new(this, _source.Version);
        internal void Complete() => _source.SetResult(true);
        internal void Fail(Exception exception) => _source.SetException(exception);
        public ValueTaskSourceStatus GetStatus(short token) => _source.GetStatus(token);
        public void OnCompleted(Action<object?> continuation, object? state, short token, ValueTaskSourceOnCompletedFlags flags) =>
            _source.OnCompleted(continuation, state, token, flags);
        public void GetResult(short token) {
            if (Interlocked.Increment(ref _consumptions) != 1) throw new InvalidOperationException("Provider ValueTask consumed more than once.");
            _source.GetResult(token);
        }
    }
}
