// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;
using System.Text;
using InfiniFrame.NativeBridge;
using InfiniFrame.NativeBridge.Handles;
using InfiniFrame.NativeBridge.Parameters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Runtime implementation managing the native InfiniFrame application lifecycle including platform registration,
///     message loop execution, and window collection tracking.
///     Access the singleton via <see cref="Instance"/> after calling <see cref="AddInfiniFrame()"/>.
/// </summary>
public sealed class InfiniFrameApplication(
    ILogger<InfiniFrameApplication> logger
) : IInfiniFrameApplication, IDisposable, IAsyncDisposable {
    private NativeApplicationHandle? _handle;
    private ApplicationConfiguration? _configuration;
    private int _disposed;
    private readonly List<(string? Id, Action<IInfiniFrameWindowBuilder> Configure)> _windowRegistrations = new();
    private readonly List<(string? Id, IInfiniFrameWindowBuilder Builder)> _directBuilders = new();
    private readonly Dictionary<string, IInfiniFrameWindow> _builtWindows = new();
    private bool _built;
    private Action? _onBeforeRun;

    /// <summary>
    ///     Gets the current application instance. Only available after <see cref="AddInfiniFrame"/> has been called.
    /// </summary>
    public static InfiniFrameApplication? Instance { get; internal set; }

    /// <inheritdoc />
    public Guid Id { get; } = Guid.NewGuid();

    /// <inheritdoc />
    public IntPtr ApplicationHandle => _handle?.DangerousGetHandle() ?? IntPtr.Zero;

    /// <inheritdoc />
    public bool IsShutdownRequested { get; private set; }

    /// <inheritdoc />
    public IReadOnlyList<IInfiniFrameWindow> Windows => _builtWindows.Values.ToList().AsReadOnly();

    // -----------------------------------------------------------------------------------------------------------------
    // Static factory
    // -----------------------------------------------------------------------------------------------------------------

    /// <summary>
    ///     Creates and optionally initializes a new InfiniFrame application.
    /// </summary>
    /// <param name="configure">Optional callback to configure application-level settings.</param>
    /// <returns>The application instance for fluent chaining with <see cref="WithWindow"/>.</returns>
    public static InfiniFrameApplication Initialize(Action<ApplicationConfiguration>? configure = null) {
        var logger = NullLogger<InfiniFrameApplication>.Instance;
        var app = new InfiniFrameApplication(logger);
        if (configure is not null) {
            var config = new ApplicationConfiguration();
            configure(config);
            app.Initialize(config);
        }
        return app;
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Fluent window registration
    // -----------------------------------------------------------------------------------------------------------------

    /// <inheritdoc />
    public InfiniFrameApplication WithWindow(Action<IInfiniFrameWindowBuilder> configure) {
        RegisterWindow(configure);
        return this;
    }

    /// <summary>
    ///     Registers a window with a unique string identifier.
    ///     The window is lazily built on the first Run() or RunAsync() call.
    /// </summary>
    /// <param name="id">A unique string identifier for the window.</param>
    /// <param name="configure">A callback to configure the window builder.</param>
    /// <returns>The application instance for chaining.</returns>
    public InfiniFrameApplication WithWindow(string id, Action<IInfiniFrameWindowBuilder> configure) {
        RegisterWindow(id, configure);
        return this;
    }

    /// <summary>
    ///     Registers a window with a unique string identifier using an existing builder.
    ///     The window is lazily built on the first Run() or RunAsync() call.
    /// </summary>
    /// <param name="id">A unique string identifier for the window.</param>
    /// <param name="builder">The window builder to use.</param>
    /// <returns>The application instance for chaining.</returns>
    public InfiniFrameApplication WithWindow(string id, IInfiniFrameWindowBuilder builder) {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ObjectDisposedException.ThrowIf(_disposed != 0, this);
        if (_built) throw new InvalidOperationException("Cannot register windows after Run() has been called.");
        _directBuilders.Add((id, builder));
        return this;
    }

    /// <summary>
    ///     Registers a window using an existing builder with an auto-generated GUID identifier.
    /// </summary>
    /// <param name="builder">The window builder to use.</param>
    /// <returns>The application instance for chaining.</returns>
    public InfiniFrameApplication WithWindow(IInfiniFrameWindowBuilder builder) {
        ArgumentNullException.ThrowIfNull(builder);
        ObjectDisposedException.ThrowIf(_disposed != 0, this);
        if (_built) throw new InvalidOperationException("Cannot register windows after Run() has been called.");
        _directBuilders.Add((null, builder));
        return this;
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Window management
    // -----------------------------------------------------------------------------------------------------------------

    /// <inheritdoc />
    public void RegisterWindow(string id, Action<IInfiniFrameWindowBuilder> configure) {
        ArgumentNullException.ThrowIfNull(configure);
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ObjectDisposedException.ThrowIf(_disposed != 0, this);
        if (_built) throw new InvalidOperationException("Cannot register windows after Run() has been called.");
        _windowRegistrations.Add((id, configure));
    }

    /// <inheritdoc />
    public void RegisterWindow(Action<IInfiniFrameWindowBuilder> configure) {
        ArgumentNullException.ThrowIfNull(configure);
        ObjectDisposedException.ThrowIf(_disposed != 0, this);
        if (_built) throw new InvalidOperationException("Cannot register windows after Run() has been called.");
        _windowRegistrations.Add((null, configure));
    }

    /// <inheritdoc />
    public IInfiniFrameWindow GetWindow(string id) {
        if (!_built) throw new InvalidOperationException("Windows have not been built yet. Call Run() or RunAsync() first.");
        return _builtWindows.TryGetValue(id, out IInfiniFrameWindow? window)
            ? window
            : throw new KeyNotFoundException($"Window with id '{id}' was not found.");
    }

    /// <inheritdoc />
    public IInfiniFrameWindow? TryGetWindow(string id) {
        if (!_built) return null;
        return _builtWindows.TryGetValue(id, out IInfiniFrameWindow? window) ? window : null;
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------

    /// <inheritdoc />
    public void Initialize(ApplicationConfiguration config) {
        ObjectDisposedException.ThrowIf(_disposed != 0, this);
        ArgumentNullException.ThrowIfNull(config);

        _configuration = config;

        var parameters = new ApplicationInitParameters {
            StructSize = Marshal.SizeOf<ApplicationInitParameters>()
        };

        // Marshal string parameters
        IntPtr appUserModelIdPtr = IntPtr.Zero;
        IntPtr notificationRegIdPtr = IntPtr.Zero;
        IntPtr webView2RuntimePathPtr = IntPtr.Zero;

        try {
            if (config.WindowsAppUserModelId is not null) {
                appUserModelIdPtr = MarshalStringUtf8(config.WindowsAppUserModelId);
                parameters.WindowsAppUserModelId = appUserModelIdPtr;
            }

            if (config.NotificationRegistrationId is not null) {
                notificationRegIdPtr = MarshalStringUtf8(config.NotificationRegistrationId);
                parameters.NotificationRegistrationId = notificationRegIdPtr;
            }

            if (config.WebView2RuntimePath is not null) {
                webView2RuntimePathPtr = MarshalStringUtf8(config.WebView2RuntimePath);
                parameters.WebView2RuntimePath = webView2RuntimePathPtr;
            }

            IntPtr unmanagedPtr = Marshal.AllocHGlobal(Marshal.SizeOf<ApplicationInitParameters>());
            try {
                Marshal.StructureToPtr(parameters, unmanagedPtr, false);

                InfiniFrameNativeInteropStatus status =
                    InfiniFrameNative.ApplicationConstructor(unmanagedPtr, out IntPtr handle);
                if (status != InfiniFrameNativeInteropStatus.Success) {
                    int lastError = Marshal.GetLastPInvokeError();
                    string nativeMessage = InfiniFrameNative.GetLastErrorMessage() ?? "No native error message provided.";
                    throw new InfiniFrameNativeInteropException(
                        $"Application constructor failed with status {status}. Error #{lastError}. {nativeMessage}");
                }

                ArgumentOutOfRangeException.ThrowIfZero(handle);

                _handle = new NativeApplicationHandle(new IntPtr(handle));

                logger.LogInformation("Native application initialized successfully.");
            }
            finally {
                Marshal.FreeHGlobal(unmanagedPtr);
            }

            // Platform-specific registration
            if (OperatingSystem.IsWindows()) {
                if (config.HInstance == IntPtr.Zero)
                    throw new InvalidOperationException("HInstance is required on Windows.");

                InfiniFrameNativeInteropStatus regStatus =
                    InfiniFrameNative.ApplicationRegisterWin32(_handle.DangerousGetHandle(), config.HInstance);
                if (regStatus != InfiniFrameNativeInteropStatus.Success) {
                    int lastError = Marshal.GetLastPInvokeError();
                    string nativeMessage = InfiniFrameNative.GetLastErrorMessage() ?? "No native error message provided.";
                    throw new InfiniFrameNativeInteropException(
                        $"Win32 registration failed with status {regStatus}. Error #{lastError}. {nativeMessage}");
                }

                logger.LogDebug("Win32 platform registration completed.");
            }
            else if (OperatingSystem.IsMacOS()) {
                InfiniFrameNativeInteropStatus regStatus =
                    InfiniFrameNative.ApplicationRegisterMac(_handle.DangerousGetHandle());
                if (regStatus != InfiniFrameNativeInteropStatus.Success) {
                    int lastError = Marshal.GetLastPInvokeError();
                    string nativeMessage = InfiniFrameNative.GetLastErrorMessage() ?? "No native error message provided.";
                    throw new InfiniFrameNativeInteropException(
                        $"macOS registration failed with status {regStatus}. Error #{lastError}. {nativeMessage}");
                }

                logger.LogDebug("macOS platform registration completed.");
            }
            else if (OperatingSystem.IsLinux()) {
                logger.LogDebug("Linux GTK initialization handled natively.");
            }
            else {
                throw new PlatformNotSupportedException();
            }
        }
        finally {
            if (appUserModelIdPtr != IntPtr.Zero) Marshal.FreeHGlobal(appUserModelIdPtr);
            if (notificationRegIdPtr != IntPtr.Zero) Marshal.FreeHGlobal(notificationRegIdPtr);
            if (webView2RuntimePathPtr != IntPtr.Zero) Marshal.FreeHGlobal(webView2RuntimePathPtr);
        }

        Instance = this;
    }

    /// <inheritdoc />
    public void Run() {
        ObjectDisposedException.ThrowIf(_disposed != 0, this);
        if (_handle is null)
            throw new InvalidOperationException("Application has not been initialized. Call Initialize() first.");

        _onBeforeRun?.Invoke();
        BuildAllWindows();

        logger.LogDebug("Starting application message loop.");

        InfiniFrameNativeInteropStatus status =
            InfiniFrameNative.ApplicationRun(_handle.DangerousGetHandle());
        if (status != InfiniFrameNativeInteropStatus.Success) {
            int lastError = Marshal.GetLastPInvokeError();
            string nativeMessage = InfiniFrameNative.GetLastErrorMessage() ?? "No native error message provided.";
            throw new InfiniFrameNativeInteropException(
                $"Application run failed with status {status}. Error #{lastError}. {nativeMessage}");
        }

        logger.LogDebug("Application message loop exited.");
    }

    /// <inheritdoc />
    public async Task RunAsync(CancellationToken ct = default) {
        ObjectDisposedException.ThrowIf(_disposed != 0, this);
        if (_handle is null)
            throw new InvalidOperationException("Application has not been initialized. Call Initialize() first.");

        _onBeforeRun?.Invoke();
        BuildAllWindows();

        logger.LogDebug("Starting application message loop (async).");

        await using var registration = ct.Register(() => Shutdown());

        await Task.Run(() => {
            InfiniFrameNativeInteropStatus status =
                InfiniFrameNative.ApplicationRun(_handle.DangerousGetHandle());
            if (status != InfiniFrameNativeInteropStatus.Success) {
                int lastError = Marshal.GetLastPInvokeError();
                string nativeMessage = InfiniFrameNative.GetLastErrorMessage() ?? "No native error message provided.";
                throw new InfiniFrameNativeInteropException(
                    $"Application run failed with status {status}. Error #{lastError}. {nativeMessage}");
            }
        }, ct).ConfigureAwait(false);

        logger.LogDebug("Application message loop exited (async).");
    }

    /// <inheritdoc />
    public void Shutdown() {
        if (_disposed != 0 || _handle is null) return;

        IsShutdownRequested = true;

        logger.LogDebug("Signaling application shutdown.");

        InfiniFrameNativeInteropStatus status =
            InfiniFrameNative.ApplicationShutdown(_handle.DangerousGetHandle());
        if (status != InfiniFrameNativeInteropStatus.Success) {
            int lastError = Marshal.GetLastPInvokeError();
            string nativeMessage = InfiniFrameNative.GetLastErrorMessage() ?? "No native error message provided.";
            logger.LogWarning(
                "Application shutdown signal returned status {Status}. Error #{LastError}. {Message}",
                status, lastError, nativeMessage);
        }
    }

    /// <inheritdoc />
    public void CloseAll() {
        ObjectDisposedException.ThrowIf(_disposed != 0, this);

        logger.LogDebug("Closing all {WindowCount} windows.", _builtWindows.Count);

        foreach (var kvp in _builtWindows) {
            var window = kvp.Value;
            try {
                window.Features.Lifecycle.Close();
            }
            catch (Exception ex) {
                logger.LogWarning(ex, "Failed to close window {WindowId}.", window.Id);
            }
        }
    }

    /// <inheritdoc />
    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync() {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;

        foreach (var window in _builtWindows.Values) {
            try {
                if (window is IAsyncDisposable asyncDisposable)
                    await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                else if (window is IDisposable disposable)
                    disposable.Dispose();
            }
            catch (Exception ex) {
                logger.LogWarning(ex, "Failed to dispose window during application shutdown.");
            }
        }
        _builtWindows.Clear();

        _handle?.Dispose();
        _handle = null;
        if (Instance == this) Instance = null;
        logger.LogDebug("Application disposed (async).");
    }

    private void Dispose(bool disposing) {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;

        if (disposing) {
            foreach (var window in _builtWindows.Values) {
                try {
                    if (window is IDisposable disposable)
                        disposable.Dispose();
                }
                catch (Exception ex) {
                    logger.LogWarning(ex, "Failed to dispose window during application shutdown.");
                }
            }
            _builtWindows.Clear();

            _handle?.Dispose();
            _handle = null;
            if (Instance == this) Instance = null;
            logger.LogDebug("Application disposed.");
        }
    }

    private void BuildAllWindows() {
        if (_built) return;
        _built = true;

        foreach (var (id, configure) in _windowRegistrations) {
            string windowId = id ?? Guid.NewGuid().ToString();
            var builder = new InfiniFrameWindowBuilder();
            configure(builder);
            IInfiniFrameWindow window = builder.Build();
            _builtWindows[windowId] = window;
        }

        foreach (var (id, builder) in _directBuilders) {
            string windowId = id ?? Guid.NewGuid().ToString();
            IInfiniFrameWindow window = builder.Build();
            _builtWindows[windowId] = window;
        }

        _windowRegistrations.Clear();
        _directBuilders.Clear();
    }

    internal void SetOnBeforeRun(Action action) {
        _onBeforeRun = action;
    }

    private static IntPtr MarshalStringUtf8(string? value) {
        if (value is null) return IntPtr.Zero;
        byte[] utf8 = Encoding.UTF8.GetBytes(value + '\0');
        IntPtr ptr = Marshal.AllocHGlobal(utf8.Length);
        Marshal.Copy(utf8, 0, ptr, utf8.Length);
        return ptr;
    }
}
