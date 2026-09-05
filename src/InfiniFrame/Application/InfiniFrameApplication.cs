// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using InfiniFrame.NativeBridge;
using InfiniFrame.NativeBridge.Handles;

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
    private readonly List<(string? Id, Action<IInfiniFrameWindowBuilder> Configure)> _registrations = [];
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
        RegisterWindowCore(null, configure);
    }

    /// <inheritdoc />
    public void RegisterWindow(string id, Action<IInfiniFrameWindowBuilder> configure) {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(configure);
        RegisterWindowCore(id, configure);
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
        BuildAllWindows();
        RunNativeLoop();
    }

    /// <inheritdoc />
    public IInfiniFrameApplication WithWebView2RuntimePath(string path) {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        EnsureConfigurationMutable();
        _webView2RuntimePath = Path.GetFullPath(path);
        ConfigureNativeApplication();
        return this;
    }

    /// <inheritdoc />
    public IInfiniFrameApplication WithNotificationRegistrationId(string id) {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        EnsureConfigurationMutable();
        _notificationRegistrationId = id;
        ConfigureNativeApplication();
        return this;
    }

    /// <inheritdoc />
    public IInfiniFrameApplication WithAppUserModelId(string id) {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        EnsureConfigurationMutable();
        _appUserModelId = id;
        ConfigureNativeApplication();
        return this;
    }

    /// <inheritdoc />
    public IInfiniFrameApplication WithDefaultNotificationIcon(string path) {
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
        await Task.Run(
            () => {
                BuildAllWindows();
                RunNativeLoop();
            },
            CancellationToken.None
        ).ConfigureAwait(false);
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

    private void RegisterWindowCore(string? id, Action<IInfiniFrameWindowBuilder> configure) {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
        lock (_gate) {
            if (_built) throw new InvalidOperationException("Cannot register windows after the application has run.");
            if (id is not null && _registrations.Any(registration => registration.Id == id))
                throw new ArgumentException($"A window with id '{id}' is already registered.", nameof(id));
            _registrations.Add((id, configure));
        }
    }

    private void BuildAllWindows() {
        lock (_gate) {
            ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
            if (_built) return;

            var built = new List<(string Id, IInfiniFrameWindow Window)>();
            try {
                foreach ((string? id, Action<IInfiniFrameWindowBuilder> configure) in _registrations) {
                    var builder = InfiniFrameWindowBuilder.Create();
                    configure(builder);
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
}
