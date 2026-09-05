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
    private readonly List<(string? Id, Action<IInfiniFrameWindowBuilder>? Configure, InfiniFrameWindowBuilder? Builder)> _registrations = [];
    private readonly List<Func<Task>> _shutdownActions = [];
    private readonly Dictionary<string, IInfiniFrameWindow> _windows = [];
    private int _disposed;
    private bool _built;
    private string? _webView2RuntimePath;
    private string? _notificationRegistrationId;
    private string? _appUserModelId;
    private string? _defaultNotificationIcon;

    private InfiniFrameApplication(ILogger<InfiniFrameApplication> logger) {
        this.logger = logger;
        InfiniFrameNativeInteropStatus status = InfiniFrameNative.ApplicationConstructor(out IntPtr handle);
        if (status != InfiniFrameNativeInteropStatus.Success)
            throw new InfiniFrameNativeInteropException(InfiniFrameNative.GetLastErrorMessage() ?? "Could not create native application.");

        _nativeHandle = new NativeApplicationHandle(handle);
        status = InfiniFrameNative.ApplicationRegister(_nativeHandle.DangerousGetHandle());
        if (status != InfiniFrameNativeInteropStatus.Success) {
            _nativeHandle.Dispose();
            throw new InfiniFrameNativeInteropException(InfiniFrameNative.GetLastErrorMessage() ?? "Could not register native application.");
        }
    }

    /// <summary>Creates an application without requiring a dependency-injection container.</summary>
    public static InfiniFrameApplication Initialize()
        => new(NullLogger<InfiniFrameApplication>.Instance);

    /// <summary>Creates an application using the supplied logger.</summary>
    public static InfiniFrameApplication Initialize(ILogger<InfiniFrameApplication> logger) {
        ArgumentNullException.ThrowIfNull(logger);
        return new InfiniFrameApplication(logger);
    }

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
            BuildAllWindows();
            RunNativeLoop();
        }
        finally {
            StopRegisteredComponents();
        }
    }

    /// <inheritdoc />
    public InfiniFrameApplication WithWebView2RuntimePath(string path) {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        EnsureConfigurationMutable();
        _webView2RuntimePath = Path.GetFullPath(path);
        ConfigureNativeApplication();
        return this;
    }

    /// <inheritdoc />
    public InfiniFrameApplication WithNotificationRegistrationId(string id) {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        EnsureConfigurationMutable();
        _notificationRegistrationId = id;
        ConfigureNativeApplication();
        return this;
    }

    /// <inheritdoc />
    public InfiniFrameApplication WithAppUserModelId(string id) {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        EnsureConfigurationMutable();
        _appUserModelId = id;
        ConfigureNativeApplication();
        return this;
    }

    /// <inheritdoc />
    public InfiniFrameApplication WithDefaultNotificationIcon(string path) {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        EnsureConfigurationMutable();
        _defaultNotificationIcon = Path.GetFullPath(path);
        ConfigureNativeApplication();
        return this;
    }

    /// <inheritdoc />
    public async Task RunAsync(CancellationToken ct = default) {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
        using CancellationTokenRegistration registration = ct.Register(Shutdown);
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var uiThread = new Thread(() => {
            try {
                InfiniFrameNativeInteropStatus registrationStatus = InfiniFrameNative.ApplicationRegister(
                    _nativeHandle.DangerousGetHandle());
                if (registrationStatus != InfiniFrameNativeInteropStatus.Success)
                    throw new InfiniFrameNativeInteropException(
                        InfiniFrameNative.GetLastErrorMessage() ?? "Could not prepare the native application UI thread.");

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
        try {
            await completion.Task.ConfigureAwait(false);
        }
        finally {
            StopRegisteredComponents();
        }
    }

    /// <inheritdoc />
    public void Shutdown() {
        InfiniFrameNative.ApplicationShutdown(_nativeHandle.DangerousGetHandle());
        IInfiniFrameWindow[] windows = Windows.ToArray();
        foreach (IInfiniFrameWindow window in windows) {
            try {
                window.Close();
            }
            catch (Exception ex) when (ex is ObjectDisposedException or InvalidOperationException) {
                logger.LogDebug(ex, "Window was already unavailable during application shutdown.");
            }
        }
    }

    /// <inheritdoc />
    public void Dispose() {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;

        StopRegisteredComponents();

        IInfiniFrameWindow[] windows;
        lock (_gate) {
            windows = _windows.Values.ToArray();
            _windows.Clear();
            _registrations.Clear();
        }

        foreach (IInfiniFrameWindow window in windows) {
            try { (window as IDisposable)?.Dispose(); }
            catch (Exception ex) when (ex is not OutOfMemoryException and not StackOverflowException) {
                logger.LogWarning(ex, "Failed to dispose an application window.");
            }
        }
        _nativeHandle.Dispose();
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync() {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;

        await StopRegisteredComponentsAsync().ConfigureAwait(false);

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
            }
            catch (Exception ex) when (ex is not OutOfMemoryException and not StackOverflowException) {
                logger.LogWarning(ex, "Failed to asynchronously dispose an application window.");
            }
        }
        _nativeHandle.Dispose();
    }

    internal void RegisterWindowBuilder(string id, InfiniFrameWindowBuilder builder) {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(builder);
        RegisterWindowCore(id, null, builder);
    }

    internal void RegisterShutdownAction(Func<Task> action) {
        ArgumentNullException.ThrowIfNull(action);
        lock (_gate) _shutdownActions.Add(action);
    }

    private void RegisterWindowCore(
        string? id,
        Action<IInfiniFrameWindowBuilder>? configure,
        InfiniFrameWindowBuilder? builder
    ) {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
        lock (_gate) {
            if (_built) throw new InvalidOperationException("Cannot register windows after the application has run.");
            if (id is not null && _registrations.Any(registration => registration.Id == id))
                throw new ArgumentException($"A window with id '{id}' is already registered.", nameof(id));
            _registrations.Add((id, configure, builder));
        }
    }

    private void BuildAllWindows() {
        lock (_gate) {
            ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
            if (_built) return;

            var built = new List<(string Id, IInfiniFrameWindow Window)>();
            try {
                foreach ((string? id, Action<IInfiniFrameWindowBuilder>? configure, InfiniFrameWindowBuilder? registeredBuilder) in _registrations) {
                    InfiniFrameWindowBuilder builder = registeredBuilder ?? new InfiniFrameWindowBuilder();
                    configure?.Invoke(builder);
                    string windowId = id ?? Guid.NewGuid().ToString("N");
                    built.Add((windowId, builder.Build()));
                }

                foreach ((string id, IInfiniFrameWindow window) in built) _windows.Add(id, window);
                _registrations.Clear();
                _built = true;
            }
            catch {
                foreach ((_, IInfiniFrameWindow window) in built) (window as IDisposable)?.Dispose();
                throw;
            }
        }
    }

    private void EnsureBuilt()
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
        if (!_built) throw new InvalidOperationException("Windows have not been built yet. Call Run() or RunAsync() first.");
    }

    private void EnsureConfigurationMutable() {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
        lock (_gate) {
            if (_built) throw new InvalidOperationException("Application configuration cannot change after the application has run.");
        }
    }

    private void ConfigureNativeApplication() {
        InfiniFrameNativeInteropStatus status = InfiniFrameNative.ApplicationConfigure(
            _nativeHandle.DangerousGetHandle(),
            _webView2RuntimePath,
            _notificationRegistrationId,
            _appUserModelId,
            _defaultNotificationIcon
        );
        if (status != InfiniFrameNativeInteropStatus.Success)
            throw new InfiniFrameNativeInteropException(InfiniFrameNative.GetLastErrorMessage() ?? "Could not configure native application.");
    }

    private void RunNativeLoop() {
        if (!OperatingSystem.IsWindows()) {
            foreach (IInfiniFrameWindow window in Windows.ToArray()) window.WaitForClose();
            return;
        }

        InfiniFrameNativeInteropStatus status = InfiniFrameNative.ApplicationRun(_nativeHandle.DangerousGetHandle());
        if (status != InfiniFrameNativeInteropStatus.Success)
            throw new InfiniFrameNativeInteropException(InfiniFrameNative.GetLastErrorMessage() ?? "Native application loop failed.");
    }

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
