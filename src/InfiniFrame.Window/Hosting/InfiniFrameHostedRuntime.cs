// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Hosting;
using InfiniFrame.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace InfiniFrame.Window.Hosting;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Core-owned runtime for one application that selected an external host provider. It validates the provider,
///     owns the provider's <see cref="IInfiniFrameHost" /> and the managed dispatcher, and drives start, window creation,
///     run completion and shutdown. It never builds a second service container: it borrows the application's.
/// </summary>
internal sealed class InfiniFrameHostedRuntime : IAsyncDisposable {
    private static readonly TimeSpan ShutdownTimeout = TimeSpan.FromSeconds(30);
    private readonly TaskCompletionSource _shutdownRequested = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private InfiniFrameHostedDispatcher? _dispatcher;
    private ILoggerFactory _loggerFactory = NullLoggerFactory.Instance;
    private ILogger _logger = NullLogger.Instance;
    private int _started;
    private int _disposed;

    internal InfiniFrameHostedRuntime(IInfiniFrameHostProvider provider, Guid applicationId) {
        DisplayName = Validate(provider);
        Capabilities = provider.Capabilities;
        Host = provider.CreateHost(new InfiniFrameHostCreateContext { ApplicationId = applicationId })
            ?? throw new InvalidOperationException($"The host provider '{DisplayName}' returned no host.");
    }

    /// <summary>
    ///     Checks the provider's contract version without side effects, so an incompatible provider is rejected before the
    ///     process is latched to an external host. Returns the provider's diagnostic name.
    /// </summary>
    internal static string Validate(IInfiniFrameHostProvider provider) {
        ArgumentNullException.ThrowIfNull(provider);
        string name = string.IsNullOrWhiteSpace(provider.DisplayName) ? provider.GetType().FullName ?? "unnamed" : provider.DisplayName;
        Version version = provider.ContractVersion
            ?? throw new NotSupportedException($"The host provider '{name}' does not report a host contract version.");
        if (version.Major != InfiniFrameHostContract.MajorVersion || version.Minor > InfiniFrameHostContract.MinorVersion)
            throw new NotSupportedException(
                $"The host provider '{name}' targets host contract {version.Major}.{version.Minor}; " +
                $"this InfiniFrame build implements {InfiniFrameHostContract.MajorVersion}.{InfiniFrameHostContract.MinorVersion}.");
        return name;
    }

    internal string DisplayName { get; }

    /// <summary>
    ///     Bound for each provider wait while a window closes, tears down or is disposed (default 30 seconds). Tests
    ///     shorten it; no public setting exists.
    /// </summary>
    internal TimeSpan CloseTimeout { get; init; } = TimeSpan.FromSeconds(30);

    internal InfiniFrameHostCapabilities Capabilities { get; }
    internal IInfiniFrameHost Host { get; }
    internal ILoggerFactory LoggerFactory => _loggerFactory;
    internal bool IsStarted => Volatile.Read(ref _started) != 0;

    internal InfiniFrameHostedDispatcher Dispatcher =>
        _dispatcher ?? throw new InvalidOperationException("The host runtime has not been started.");

    /// <summary>Creates the managed dispatcher and starts the provider's host. Called once, from the run.</summary>
    internal async Task StartAsync(IServiceProvider services, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(services);
        if (Interlocked.Exchange(ref _started, 1) != 0) throw new InvalidOperationException("The host runtime has already been started.");
        _loggerFactory = services.GetService<ILoggerFactory>() ?? NullLoggerFactory.Instance;
        _logger = _loggerFactory.CreateLogger<InfiniFrameHostedRuntime>();
        _dispatcher = new InfiniFrameHostedDispatcher(_loggerFactory.CreateLogger<InfiniFrameHostedDispatcher>());
        _logger.LogDebug("Starting external host provider {Provider} (capabilities {Capabilities})", DisplayName, Capabilities);
        var context = new InfiniFrameHostStartContext { Services = services, LoggerFactory = _loggerFactory };
        await InfiniFrameProviderCall.Start(() => Host.StartAsync(context, cancellationToken)).ConfigureAwait(false);
        await InfiniFrameProviderCall.ToThreadPool();
    }

    /// <summary>Requests the native windows in registration order. The first failure stops creation and is rethrown.</summary>
    internal async Task CreateWindowsAsync(IReadOnlyList<InfiniFrameHostedWindow> windows, CancellationToken cancellationToken) {
        foreach (InfiniFrameHostedWindow window in windows)
            await window.StartAsync(cancellationToken).ConfigureAwait(false);
    }

    internal void RequestShutdown() => _shutdownRequested.TrySetResult();

    /// <summary>
    ///     Waits until every window has torn down, shutdown is requested, or the host completes. A host that completes
    ///     first ends every remaining window; its fault, if any, is returned so the run can rethrow it.
    /// </summary>
    internal async Task<Exception?> WaitForRunCompletionAsync(IReadOnlyList<InfiniFrameHostedWindow> windows) {
        Task allTornDown = Task.WhenAll(windows.Select(static window => window.WaitForTeardownAsync().AsTask()));
        await Task.WhenAny(allTornDown, Host.Completion, _shutdownRequested.Task).ConfigureAwait(false);
        // Host.Completion may be completed on a provider thread: never continue the run (and its disposal) there.
        await InfiniFrameProviderCall.ToThreadPool();
        if (!Host.Completion.IsCompleted) return null;

        Exception? fault = Host.Completion.IsFaulted
            ? Host.Completion.Exception!.GetBaseException()
            : Host.Completion.IsCanceled ? new OperationCanceledException("The host completed as cancelled.") : null;
        if (fault is not null) _logger.LogError(fault, "External host provider {Provider} failed", DisplayName);
        foreach (InfiniFrameHostedWindow window in windows) {
            if (window.LifecycleState >= InfiniFrameWindowLifecycleState.NativeClosed) continue;
            window.FaultWindow(new InfiniFrameHostOperationException($"The host provider '{DisplayName}' stopped while the window was open.", fault));
        }
        return fault;
    }

    /// <summary>Shuts the host down (if started) and disposes it exactly once, then stops the dispatcher.</summary>
    public async ValueTask DisposeAsync() {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;
        try {
            if (IsStarted) {
                try {
                    await InfiniFrameProviderCall.Start(() => Host.ShutdownAsync(CancellationToken.None)).AsTask().WaitAsync(ShutdownTimeout).ConfigureAwait(false);
                    await InfiniFrameProviderCall.ToThreadPool();
                }
                catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) {
                    _logger.LogWarning(exception, "External host provider {Provider} failed to shut down", DisplayName);
                }
            }
            try {
                await InfiniFrameProviderCall.Start(Host.DisposeAsync).AsTask().WaitAsync(ShutdownTimeout).ConfigureAwait(false);
                await InfiniFrameProviderCall.ToThreadPool();
            }
            catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) {
                _logger.LogWarning(exception, "External host provider {Provider} failed to dispose", DisplayName);
            }
        }
        finally {
            _dispatcher?.Dispose();
        }
    }
}
