// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using InfiniFrame.NativeBridge;
using InfiniFrame.NativeBridge.Handles;
using InfiniFrame.Utilities;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Application-level owner for lazily built InfiniFrame windows.
/// </summary>
public sealed class InfiniFrameApplication : IInfiniFrameApplication {
    private readonly ILogger<InfiniFrameApplication> logger;
    private readonly NativeApplicationHandle _nativeHandle;
    private readonly object _gate = new();
    private readonly List<(
        string? Id,
        Action<IInfiniFrameWindowBuilder>? Configure,
        InfiniFrameWindowBuilder? Builder,
        IServiceProvider? Provider
    )> _registrations = [];
    private readonly List<Action<InfiniFrameWindowBuilder>> _windowConventions = [];
    private readonly List<Func<Task>> _shutdownActions = [];
    private readonly List<Func<Task>> _startupActions = [];
    private readonly Dictionary<string, IInfiniFrameWindow> _windows = [];
    private IServiceProvider? _serviceProvider;
    private readonly TaskCompletionSource _shutdownSignal = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private int _disposed;
    private bool _built;
    private int _shutdownRequested;

    private InfiniFrameApplication(ILogger<InfiniFrameApplication> logger, ApplicationConfiguration configuration) {
        this.logger = logger;
        InfiniFrameNativeInteropStatus status = InfiniFrameNative.ApplicationConstructor(out IntPtr handle);
        if (status != InfiniFrameNativeInteropStatus.Success)
            throw new InfiniFrameNativeInteropException(InfiniFrameNative.GetLastErrorMessage() ?? "Could not create native application.");

        _nativeHandle = new NativeApplicationHandle(handle);
        InfiniFrameNativeInteropStatus configureStatus = InfiniFrameNative.ApplicationConfigure(
            _nativeHandle.DangerousGetHandle(),
            configuration.WebView2RuntimePath,
            configuration.NotificationRegistrationId,
            configuration.AppUserModelId,
            configuration.DefaultNotificationIcon
        );
        if (configureStatus != InfiniFrameNativeInteropStatus.Success) {
            _nativeHandle.Dispose();
            throw new InfiniFrameNativeInteropException(
                InfiniFrameNative.GetLastErrorMessage() ?? "Could not configure native application.");
        }
    }

    /// <summary>Creates an application without requiring a dependency-injection container.</summary>
    public static InfiniFrameApplication Initialize()
        => new(NullLogger<InfiniFrameApplication>.Instance, new ApplicationConfiguration());

    internal static InfiniFrameApplication Initialize(ApplicationConfiguration configuration)
        => new(NullLogger<InfiniFrameApplication>.Instance, configuration);

    public static InfiniFrameApplicationBuilder CreateBuilder(string[]? args = null)
        => new(args);

    /// <summary>Creates an application using the supplied logger.</summary>
    public static InfiniFrameApplication Initialize(ILogger<InfiniFrameApplication> logger) {
        ArgumentNullException.ThrowIfNull(logger);
        return new InfiniFrameApplication(logger, new ApplicationConfiguration());
    }

    public Guid Id { get; } = Guid.NewGuid();
    public IntPtr ApplicationHandle => _nativeHandle.DangerousGetHandle();
    public bool IsShutdownRequested => Volatile.Read(ref _shutdownRequested) != 0;
    public event Action<IInfiniFrameWindow>? WindowCreated;
    public event Action<IInfiniFrameWindow>? WindowDestroyed;

    /// <inheritdoc />
    public void RegisterWindow(Action<IInfiniFrameWindowBuilder> configure) {
        ArgumentNullException.ThrowIfNull(configure);
        RegisterWindowCore(null, configure, null);
    }

    /// <inheritdoc />
    public void RegisterWindow(string id, Action<IInfiniFrameWindowBuilder> configure) {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(configure);
        RegisterWindowCore(id, configure, null);
    }

    /// <summary>Registers an unnamed window and returns this application for fluent configuration.</summary>
    public InfiniFrameApplication WithWindow(Action<IInfiniFrameWindowBuilder> configure) {
        RegisterWindow(configure);
        return this;
    }

    /// <summary>Registers a named window and returns this application for fluent configuration.</summary>
    public InfiniFrameApplication WithWindow(string id, Action<IInfiniFrameWindowBuilder> configure) {
        RegisterWindow(id, configure);
        return this;
    }

    /// <inheritdoc />
    public IInfiniFrameWindow GetWindow(string id) {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        lock (_gate) {
            EnsureBuilt();
            return _windows.TryGetValue(id, out IInfiniFrameWindow? window)
                ? window
                : throw new KeyNotFoundException($"Window with id '{id}' was not found.");
        }
    }

    /// <inheritdoc />
    public IInfiniFrameWindow? TryGetWindow(string id) {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        lock (_gate) return _built && _windows.TryGetValue(id, out IInfiniFrameWindow? window) ? window : null;
    }

    /// <inheritdoc />
    public IReadOnlyList<IInfiniFrameWindow> Windows {
        get {
            lock (_gate) return _windows.Values.ToArray();
        }
    }

    /// <inheritdoc />
    public void Run() {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
        try {
            RegisterNativeApplication();
            StartRegisteredComponents();
            BuildAllWindows();
            RunNativeLoop();
        }
        finally {
            Dispose();
        }
    }

    /// <inheritdoc />
    public async Task RunAsync(CancellationToken ct = default) {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
        using CancellationTokenRegistration registration = ct.Register(Shutdown);
        try {
            await StartRegisteredComponentsAsync().ConfigureAwait(false);
            var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var uiThread = new Thread(() => {
                try {
                    RegisterNativeApplication();
                    BuildAllWindows();
                    RunNativeLoop();
                    completion.TrySetResult();
                }
                catch (Exception exception) when (ExceptionsUtility.IsNonFatalException(exception)) {
                    completion.TrySetException(exception);
                }
            }) {
                IsBackground = true,
                Name = "InfiniFrame Application UI Thread"
            };

            if (OperatingSystem.IsWindows())
                uiThread.SetApartmentState(ApartmentState.STA);
            uiThread.Start();
            await Task.WhenAny(completion.Task, _shutdownSignal.Task).ConfigureAwait(false);
            if (!completion.Task.IsCompleted) {
                try {
                    await completion.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                }
                catch (TimeoutException) {
                    logger.LogWarning("Native application loop did not exit after shutdown request.");
                }
            }
        }
        finally {
            await DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public void Shutdown() {
        if (Volatile.Read(ref _disposed) != 0) return;
        Volatile.Write(ref _shutdownRequested, 1);
        _shutdownSignal.TrySetResult();
        InfiniFrameNative.ApplicationShutdown(_nativeHandle.DangerousGetHandle());
    }

    /// <inheritdoc />
    public void CloseAll() => Shutdown();

    private void RegisterNativeApplication() {
        InfiniFrameNativeInteropStatus status = InfiniFrameNative.ApplicationRegister(_nativeHandle.DangerousGetHandle());
        if (status != InfiniFrameNativeInteropStatus.Success)
            throw new InfiniFrameNativeInteropException(
                InfiniFrameNative.GetLastErrorMessage() ?? "Could not register native application.");
    }

    /// <inheritdoc />
    public void Dispose() {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;

        IInfiniFrameWindow[] windows;
        lock (_gate) {
            windows = _windows.Values.ToArray();
            _windows.Clear();
            _registrations.Clear();
        }

        foreach (IInfiniFrameWindow window in windows) {
            try {
                (window as IDisposable)?.Dispose();
                WindowDestroyed?.Invoke(window);
            }
            catch (Exception ex) when (ex is not OutOfMemoryException and not StackOverflowException) {
                logger.LogWarning(ex, "Failed to dispose an application window.");
            }
        }
        StopRegisteredComponents();
        _nativeHandle.Dispose();
        if (_serviceProvider is IAsyncDisposable asyncServiceProvider)
            asyncServiceProvider.DisposeAsync().AsTask().GetAwaiter().GetResult();
        else (_serviceProvider as IDisposable)?.Dispose();
        _serviceProvider = null;
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync() {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;

        IInfiniFrameWindow[] windows;
        lock (_gate) {
            windows = _windows.Values.ToArray();
            _windows.Clear();
            _registrations.Clear();
        }

        foreach (IInfiniFrameWindow window in windows) {
            try {
                if (window is IAsyncDisposable asyncDisposable)
                    await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                else (window as IDisposable)?.Dispose();
                WindowDestroyed?.Invoke(window);
            }
            catch (Exception ex) when (ex is not OutOfMemoryException and not StackOverflowException) {
                logger.LogWarning(ex, "Failed to asynchronously dispose an application window.");
            }
        }
        await StopRegisteredComponentsAsync().ConfigureAwait(false);
        _nativeHandle.Dispose();
        if (_serviceProvider is IAsyncDisposable asyncServiceProvider)
            await asyncServiceProvider.DisposeAsync().ConfigureAwait(false);
        else (_serviceProvider as IDisposable)?.Dispose();
        _serviceProvider = null;
    }

    internal void RegisterWindowBuilder(string id, InfiniFrameWindowBuilder builder, IServiceProvider? provider = null) {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(builder);
        RegisterWindowCore(id, null, builder, provider);
    }

    internal void RegisterWindowConvention(Action<InfiniFrameWindowBuilder> configure) {
        ArgumentNullException.ThrowIfNull(configure);
        lock (_gate) _windowConventions.Add(configure);
    }

    internal void RegisterShutdownAction(Func<Task> action) {
        ArgumentNullException.ThrowIfNull(action);
        lock (_gate) _shutdownActions.Add(action);
    }

    internal void RegisterStartupAction(Func<Task> action) {
        ArgumentNullException.ThrowIfNull(action);
        lock (_gate) _startupActions.Add(action);
    }

    private void RegisterWindowCore(
        string? id,
        Action<IInfiniFrameWindowBuilder>? configure,
        InfiniFrameWindowBuilder? builder,
        IServiceProvider? provider = null
    ) {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
        lock (_gate) {
            if (_built) throw new InvalidOperationException("Cannot register windows after the application has run.");
            if (id is not null && _registrations.Any(registration => registration.Id == id))
                throw new ArgumentException($"A window with id '{id}' is already registered.", nameof(id));
            _registrations.Add((id, configure, builder, provider));
        }
    }

    private void BuildAllWindows() {
        (string Id, IInfiniFrameWindow Window)[] built;
        lock (_gate) {
            ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
            if (_built) return;

            var windows = new List<(string Id, IInfiniFrameWindow Window)>();
            try {
                foreach ((string? id, Action<IInfiniFrameWindowBuilder>? configure, InfiniFrameWindowBuilder? registeredBuilder, IServiceProvider? provider) in _registrations) {
                    InfiniFrameWindowBuilder builder = registeredBuilder ?? new InfiniFrameWindowBuilder();
                    foreach (Action<InfiniFrameWindowBuilder> convention in _windowConventions)
                        convention(builder);
                    configure?.Invoke(builder);
                    builder.SetApplicationHandle(_nativeHandle.DangerousGetHandle());
                    string windowId = id ?? Guid.NewGuid().ToString("N");
                    windows.Add((windowId, builder.Build(provider ?? _serviceProvider)));
                }

                foreach ((string id, IInfiniFrameWindow window) in windows) {
                    _windows.Add(id, window);
                }
                _registrations.Clear();
                _built = true;
                built = [.. windows];
            }
            catch {
                foreach ((_, IInfiniFrameWindow window) in windows) (window as IDisposable)?.Dispose();
                throw;
            }
        }

        foreach ((_, IInfiniFrameWindow window) in built)
            WindowCreated?.Invoke(window);
    }

    private void EnsureBuilt()
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
        if (!_built) throw new InvalidOperationException("Windows have not been built yet. Call Run() or RunAsync() first.");
    }

    private void RunNativeLoop() {
        foreach (IInfiniFrameWindow window in Windows.ToArray()) window.WaitForClose();
    }

    internal void AttachServiceProvider(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;
    internal IServiceProvider RootServiceProvider => _serviceProvider
        ?? throw new InvalidOperationException("The application service provider has not been initialized.");

    private void StopRegisteredComponents() {
        Func<Task>[] actions;
        lock (_gate) {
            actions = _shutdownActions.ToArray();
            _shutdownActions.Clear();
        }

        foreach (Func<Task> action in actions) {
            try { action().GetAwaiter().GetResult(); }
            catch (Exception ex) when (ex is not OutOfMemoryException and not StackOverflowException) {
                logger.LogWarning(ex, "Failed to stop an application component.");
            }
        }
    }

    private void StartRegisteredComponents() {
        Func<Task>[] actions;
        lock (_gate) actions = _startupActions.ToArray();
        foreach (Func<Task> action in actions) action().GetAwaiter().GetResult();
    }

    private async Task StartRegisteredComponentsAsync() {
        Func<Task>[] actions;
        lock (_gate) actions = _startupActions.ToArray();
        foreach (Func<Task> action in actions) await action().ConfigureAwait(false);
    }

    private async Task StopRegisteredComponentsAsync() {
        Func<Task>[] actions;
        lock (_gate) {
            actions = _shutdownActions.ToArray();
            _shutdownActions.Clear();
        }

        foreach (Func<Task> action in actions) {
            try { await action().ConfigureAwait(false); }
            catch (Exception ex) when (ex is not OutOfMemoryException and not StackOverflowException) {
                logger.LogWarning(ex, "Failed to stop an application component.");
            }
        }
    }
}
