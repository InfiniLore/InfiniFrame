// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge;
using InfiniFrame.NativeBridge.Handles;
using FluentValidation;
using InfiniFrame.NativeBridge.Parameters.Application;
using InfiniFrame.Utilities;
using InfiniFrame.Window.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace InfiniFrame.Application;
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
    private readonly List<Func<Task>> _shutdownActions = [];
    private readonly List<Func<Task>> _startupActions = [];
    private readonly Dictionary<string, IInfiniFrameWindow> _windows = [];
    private IServiceProvider? _serviceProvider;
    private int _disposed;
    private int _runState;
    private bool _built;
    private int _shutdownRequested;

    private InfiniFrameApplication(ILogger<InfiniFrameApplication> logger, ApplicationConfiguration configuration) {
        this.logger = logger;
        InfiniFrameNativeApplicationParameters parameters = configuration.ToNativeParameters();
        new InfiniFrameNativeApplicationParametersValidator().ValidateAndThrow(parameters);

        InfiniFrameNativeInteropStatus status = InfiniFrameNative.ApplicationConstructor(out IntPtr handle);
        if (status != InfiniFrameNativeInteropStatus.Success)
            throw new InfiniFrameNativeInteropException(InfiniFrameNative.GetLastErrorMessage() ?? "Could not create native application.");

        _nativeHandle = new NativeApplicationHandle(handle);
        InfiniFrameNativeInteropStatus configureStatus = InfiniFrameNative.ApplicationConfigure(
            _nativeHandle.DangerousGetHandle(),
            in parameters
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
        BeginRun();
        try {
            EnsureWindowsStaThread();
            RegisterNativeApplication();
            if (IsShutdownRequested) return;
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
        BeginRun();
        await using CancellationTokenRegistration registration = ct.Register(Shutdown);
        Task? uiTask = null;
        try {
            ct.ThrowIfCancellationRequested();
            await StartRegisteredComponentsAsync().ConfigureAwait(false);
            ct.ThrowIfCancellationRequested();
            var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var uiThread = new Thread(() => {
                try {
                    if (IsShutdownRequested) {
                        completion.TrySetResult();
                        return;
                    }

                    RegisterNativeApplication();
                    if (IsShutdownRequested) {
                        completion.TrySetResult();
                        return;
                    }

                    BuildAllWindows();
                    if (IsShutdownRequested) {
                        CloseAll();
                        completion.TrySetResult();
                        return;
                    }

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
            uiTask = completion.Task;
            await uiTask.ConfigureAwait(false);
        }
        finally {
            try {
                if (uiTask is not null)
                    await uiTask.ConfigureAwait(false);
            }
            finally {
                await DisposeAsync().ConfigureAwait(false);
            }
        }
    }

    /// <inheritdoc />
    public void Shutdown() {
        if (Volatile.Read(ref _disposed) != 0) return;
        if (Interlocked.Exchange(ref _shutdownRequested, 1) != 0) return;
        CloseAll();
        if (!OperatingSystem.IsWindows())
            InfiniFrameNative.ApplicationShutdown(_nativeHandle.DangerousGetHandle());
    }

    /// <inheritdoc />
    public void CloseAll() {
        foreach (IInfiniFrameWindow window in Windows.ToArray()) {
            try {
                window.Close();
            }
            catch (Exception exception) when (exception is ObjectDisposedException or InvalidOperationException) {
                logger.LogDebug(exception, "Window was already unavailable during application shutdown.");
            }
        }
    }

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
        }

        Exception? windowDisposalFailure = null;
        foreach (IInfiniFrameWindow window in windows) {
            try {
                (window as IDisposable)?.Dispose();
                if (window.LifecycleState != InfiniFrameWindowLifecycleState.Disposed)
                    throw new InvalidOperationException("A window did not complete native disposal.");
                WindowDestroyed?.Invoke(window);
            }
            catch (Exception ex) when (ex is not OutOfMemoryException and not StackOverflowException) {
                logger.LogWarning(ex, "Failed to dispose an application window.");
                windowDisposalFailure ??= ex;
            }
        }
        if (windowDisposalFailure is not null) {
            Volatile.Write(ref _disposed, 0);
            throw new InvalidOperationException("The application could not dispose all windows.", windowDisposalFailure);
        }
        lock (_gate) {
            _windows.Clear();
            _registrations.Clear();
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
        }

        Exception? windowDisposalFailure = null;
        foreach (IInfiniFrameWindow window in windows) {
            try {
                if (window is IAsyncDisposable asyncDisposable)
                    await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                else (window as IDisposable)?.Dispose();
                if (window.LifecycleState != InfiniFrameWindowLifecycleState.Disposed)
                    throw new InvalidOperationException("A window did not complete native disposal.");
                WindowDestroyed?.Invoke(window);
            }
            catch (Exception ex) when (ex is not OutOfMemoryException and not StackOverflowException) {
                logger.LogWarning(ex, "Failed to asynchronously dispose an application window.");
                windowDisposalFailure ??= ex;
            }
        }
        if (windowDisposalFailure is not null) {
            Volatile.Write(ref _disposed, 0);
            throw new InvalidOperationException("The application could not dispose all windows.", windowDisposalFailure);
        }
        lock (_gate) {
            _windows.Clear();
            _registrations.Clear();
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

    private void BeginRun() {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
        if (Interlocked.CompareExchange(ref _runState, 1, 0) != 0)
            throw new InvalidOperationException("The InfiniFrame application can only be run once.");
    }

    internal void ValidateWindowIntegrationTargets(IReadOnlyList<string>? ids, string integrationName) {
        lock (_gate) {
            GetIntegrationTargets(ids, integrationName);
        }
    }

    internal void ApplyWindowIntegration(
        IReadOnlyList<string>? ids,
        string integrationName,
        Action<IInfiniFrameWindowBuilder> configure
    ) {
        ArgumentNullException.ThrowIfNull(configure);
        lock (_gate) {
            foreach (int index in GetIntegrationTargets(ids, integrationName)) {
                (string? id, Action<IInfiniFrameWindowBuilder>? existing, InfiniFrameWindowBuilder? builder, IServiceProvider? provider) = _registrations[index];
                _registrations[index] = (id, target => {
                    existing?.Invoke(target);
                    configure(target);
                }, builder, provider);
            }
        }
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
            if (_built || Volatile.Read(ref _runState) != 0)
                throw new InvalidOperationException("Cannot register windows after the application run has started.");
            if (id is not null && _registrations.Any(registration => registration.Id == id))
                throw new ArgumentException($"A window with id '{id}' is already registered.", nameof(id));
            _registrations.Add((id, configure, builder, provider));
        }
    }

    private IReadOnlyList<int> GetIntegrationTargets(IReadOnlyList<string>? ids, string integrationName) {
        if (ids is null || ids.Count == 0) {
            if (_registrations.Count == 0)
                throw new InvalidOperationException(
                    $"Cannot bind {integrationName}: no windows are registered. Register a window before configuring the integration.");
            if (_registrations.Count != 1)
                throw new InvalidOperationException(
                    $"Cannot bind {integrationName}: {_registrations.Count} windows are registered. Specify explicit window IDs.");
            return [0];
        }

        if (ids.Any(string.IsNullOrWhiteSpace))
            throw new ArgumentException($"{integrationName} window IDs must not be null or whitespace.", nameof(ids));
        if (ids.Count != ids.Distinct(StringComparer.Ordinal).Count())
            throw new ArgumentException($"{integrationName} window IDs must not contain duplicates.", nameof(ids));

        var targets = new List<int>(ids.Count);
        foreach (string id in ids) {
            int index = _registrations.FindIndex(registration => string.Equals(registration.Id, id, StringComparison.Ordinal));
            if (index < 0)
                throw new InvalidOperationException(
                    $"Cannot bind {integrationName}: no registered window has ID '{id}'.");
            targets.Add(index);
        }
        return targets;
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
        if (OperatingSystem.IsWindows()) {
            InfiniFrameNativeInteropStatus status = InfiniFrameNative.ApplicationRun(_nativeHandle.DangerousGetHandle());
            if (status != InfiniFrameNativeInteropStatus.Success)
                throw new InfiniFrameNativeInteropException(
                    InfiniFrameNative.GetLastErrorMessage() ?? "Could not run the native application.");
            return;
        }

        foreach (IInfiniFrameWindow window in Windows.ToArray()) window.WaitForClose();
    }

    private static void EnsureWindowsStaThread() {
        if (OperatingSystem.IsWindows() && Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
            throw new InvalidOperationException("InfiniFrameApplication.Run() must be called from a Windows STA thread.");
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
