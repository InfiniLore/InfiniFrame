// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge;
using InfiniFrame.NativeBridge.Dialogs;
using InfiniFrame.NativeBridge.Parameters;
using InfiniFrame.StaticAssets;
using InfiniFrame.Utilities;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class InfiniFrameWindow : IInfiniFrameWindow {
    private static readonly Lazy<IntPtr> WindowType = new(NativeLibrary.GetMainProgramHandle);

    public required ILogger<IInfiniFrameWindow> Logger { get; init; }
    public required IServiceProvider? ServiceProvider { get; init; }
    public required IInfiniFrameEvents Events { get; init; }
    public required IInfiniFrameOptions Configuration { get; init; }
    public IInfiniFrameStaticAssets? StaticAssets { get; init; }

    public IInfiniFrameEventsStore EventsStore => Events.EventsStore;

    public IntPtr NativeType => WindowType.Value;

    public IntPtr InstanceHandle { get; private set; }

    public Rectangle CachedPreFullScreenBounds { get; set; } = Rectangle.Empty;
    public Rectangle CachedPreMaximizedBounds { get; set; } = Rectangle.Empty;

    private int _shutdownState;

    private bool IsClosing => Volatile.Read(ref _shutdownState) != 0;
    public bool IsClosed => Volatile.Read(ref _shutdownState) == 2;
    public bool IsClosedOrClosing => IsClosing || InstanceHandle == IntPtr.Zero;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------

    /// <summary>
    ///     Dispatches an Action to the UI thread if called from another thread.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="InfiniFrameWindow" /> instance.
    /// </returns>
    /// <param name="workItem"> The delegate encapsulating a method / action to be executed in the UI thread.</param>
    public void Invoke(Action workItem) {
        if (Environment.CurrentManagedThreadId == ManagedThreadId) {
            Run(workItem);
            return;
        }

        IntPtr handle = InstanceHandle;

        InfiniFrameNative.EnsureSucceeded(
            InfiniFrameNative.Invoke(handle, callback: () => Run(workItem)),
            nameof(InfiniFrameNative.Invoke)
        );
        return;

        static void Run(Action action) {
            Marshal.SetLastPInvokeError(0);
            action();
            InfiniFrameNative.EnsureSucceeded(
                InfiniFrameNativeInteropStatus.Success,
                "Invoke"
            );
        }
    }

    /// <summary>
    ///     Responsible for the initialization of the primary native window and remains in operation until the window is
    ///     closed.
    ///     This method is also applicable for initializing child windows, but in this case, it does not inhibit operation.
    /// </summary>
    /// <remarks>
    ///     The operation of the message loop is exclusive to the main native window only.
    /// </remarks>
    public void WaitForClose() {
        if (IsClosing || IsClosed) {
            Logger.LogDebug("Skipping WaitForClose during shutdown");
            return;
        }

        try {
            Logger.LogDebug("Starting message loop for window.");

            IntPtr handle = InstanceHandle;
            if (handle == IntPtr.Zero)
                return;

            Invoke(() => InfiniFrameNative.WaitForExit(handle));
        }
        catch (Exception ex) when (ExceptionsUtility.IsNonFatalException(ex)) {
            int lastError = OperatingSystem.IsWindows()
                ? Marshal.GetLastWin32Error()
                : 0;

            Logger.LogError(ex, "Error #{LastErrorCode} while running message loop", lastError);
            throw new ApplicationException(
                $"Native code exception. Error # {lastError}",
                ex);
        }
        finally {
            InstanceHandle = IntPtr.Zero;
            Interlocked.Exchange(ref _shutdownState, 2);
        }
    }

    public ValueTask WaitForCloseAsync(CancellationToken ct = default) {
        if (ct.IsCancellationRequested)
            return ValueTask.FromCanceled(ct);

        WaitForClose();
        return ValueTask.CompletedTask;
    }

    void IInfiniFrameWindow.MarkClosedFromNativeCallback() {
        InstanceHandle = IntPtr.Zero;
        Interlocked.Exchange(ref _shutdownState, 2);
    }

    /// <summary>
    ///     Closes the native window.
    /// </summary>
    /// <exception cref="ApplicationException">
    ///     Thrown when the window is not initialized.
    /// </exception>
    public void Close() {
        if (Interlocked.Exchange(ref _shutdownState, 1) != 0) {
            Logger.LogDebug("Skipping Close during shutdown");
            return;
        }

        Logger.LogDebug(".Close()");
        Events.OnWindowClosingRequested();

        IntPtr handle = InstanceHandle;
        if (handle == IntPtr.Zero)
            return;

        Invoke(() => {
            IntPtr h = InstanceHandle;
            if (h == IntPtr.Zero)
                return;

            InfiniFrameNative.Close(h);

            InstanceHandle = IntPtr.Zero;
            Interlocked.Exchange(ref _shutdownState, 2);
        });
    }

    public ValueTask CloseAsync(CancellationToken ct = default) {
        if (ct.IsCancellationRequested)
            return ValueTask.FromCanceled(ct);

        Close();
        return ValueTask.CompletedTask;
    }

    /// <summary>
    ///     Send a message to the native window's native browser control's JavaScript context.
    /// </summary>
    /// <remarks>
    ///     In JavaScript, messages can be received via <code>window.infiniframe.host.receiveCallback(callback)</code>.
    /// </remarks>
    /// <exception cref="ApplicationException">
    ///     Thrown when the window is not initialized.
    /// </exception>
    /// <param name="message">Message as string</param>
    public void SendWebMessage(string message) {
        if (IsClosedOrClosing)
            return;

        IntPtr handle = InstanceHandle;
        if (handle == IntPtr.Zero)
            return;

        Invoke(() => {
            if (InstanceHandle == IntPtr.Zero)
                return;

            InfiniFrameNative.SendWebMessage(handle, message);
        });
    }

    public Task SendWebMessageAsync(string message, CancellationToken ct = default)
        => ct.IsCancellationRequested
            ? Task.FromCanceled(ct)
            : Task.Run(action: () => SendWebMessage(message), ct);

    /// <summary>
    ///     Sends a native notification to the OS.
    ///     Sometimes referred to as Toast notifications.
    /// </summary>
    /// <exception cref="ApplicationException">
    ///     Thrown when the window is not initialized.
    /// </exception>
    /// <param name="title">The title of the notification</param>
    /// <param name="body">The text of the notification</param>
    public void SendNotification(string title, string body) {
        if (IsClosedOrClosing)
            return;

        IntPtr handle = InstanceHandle;
        if (handle == IntPtr.Zero)
            return;

        Invoke(() => {
            if (InstanceHandle == IntPtr.Zero)
                return;

            InfiniFrameNative.ShowNotification(handle, title, body);
        });
    }


    /// <summary>
    ///     Show an open file dialog native to the OS.
    /// </summary>
    /// <remarks>
    ///     Filter names are not used on macOS. Use async version for InfiniFrame.Blazor as the synchronous version
    ///     crashes.
    /// </remarks>
    /// <exception cref="ApplicationException">
    ///     Thrown when the window is not initialized.
    /// </exception>
    /// <param name="title">Title of the dialog</param>
    /// <param name="defaultPath">Default path. Defaults to <see cref="Environment.SpecialFolder.MyDocuments" /></param>
    /// <param name="multiSelect">Whether multiple selections are allowed</param>
    /// <param name="filters">Array of Extensions for filtering.</param>
    /// <returns>Array of file paths as strings</returns>
    public string?[] ShowOpenFile(string title = "Choose file", string? defaultPath = null, bool multiSelect = false, (string Name, string[] Extensions)[]? filters = null)
        => ShowOpenDialog(false, title, defaultPath, multiSelect, filters);

    /// <summary>
    ///     Async version is required for InfiniFrame.Blazor
    /// </summary>
    /// <remarks>
    ///     Filter names are not used on macOS. Use async version for InfiniFrame.Blazor as the synchronous version
    ///     crashes.
    /// </remarks>
    /// <exception cref="ApplicationException">
    ///     Thrown when the window is not initialized.
    /// </exception>
    /// <param name="title">Title of the dialog</param>
    /// <param name="defaultPath">Default path. Defaults to <see cref="Environment.SpecialFolder.MyDocuments" /></param>
    /// <param name="multiSelect">Whether multiple selections are allowed</param>
    /// <param name="filters">Array of Extensions for filtering.</param>
    /// <param name="ct">Cancellation token for the operation</param>
    /// <returns>Array of file paths as strings</returns>
    public Task<string?[]> ShowOpenFileAsync(string title = "Choose file", string? defaultPath = null, bool multiSelect = false, (string Name, string[] Extensions)[]? filters = null, CancellationToken ct = default)
        => RunDialogAsync(workItem: () => ShowOpenFile(title, defaultPath, multiSelect, filters), ct);

    /// <summary>
    ///     Show an open folder dialog native to the OS.
    /// </summary>
    /// <exception cref="ApplicationException">
    ///     Thrown when the window is not initialized.
    /// </exception>
    /// <param name="title">Title of the dialog</param>
    /// <param name="defaultPath">Default path. Defaults to <see cref="Environment.SpecialFolder.MyDocuments" /></param>
    /// <param name="multiSelect">Whether multiple selections are allowed</param>
    /// <returns>Array of folder paths as strings</returns>
    public string?[] ShowOpenFolder(string title = "Select folder", string? defaultPath = null, bool multiSelect = false)
        => ShowOpenDialog(true, title, defaultPath, multiSelect, null);

    /// <summary>
    ///     Async version is required for InfiniFrame.Blazor
    /// </summary>
    /// <exception cref="ApplicationException">
    ///     Thrown when the window is not initialized.
    /// </exception>
    /// <param name="title">Title of the dialog</param>
    /// <param name="defaultPath">Default path. Defaults to <see cref="Environment.SpecialFolder.MyDocuments" /></param>
    /// <param name="multiSelect">Whether multiple selections are allowed</param>
    /// <param name="ct">Cancellation token for the operation</param>
    /// <returns>Array of folder paths as strings</returns>
    public Task<string?[]> ShowOpenFolderAsync(string title = "Choose file", string? defaultPath = null, bool multiSelect = false, CancellationToken ct = default)
        => RunDialogAsync(workItem: () => ShowOpenFolder(title, defaultPath, multiSelect), ct);

    /// <summary>
    ///     Show a save folder dialog native to the OS.
    /// </summary>
    /// <remarks>
    ///     Filter names are not used on macOS.
    /// </remarks>
    /// <exception cref="ApplicationException">
    ///     Thrown when the window is not initialized.
    /// </exception>
    /// <param name="title">Title of the dialog</param>
    /// <param name="defaultPath">Default path. Defaults to <see cref="Environment.SpecialFolder.MyDocuments" /></param>
    /// <param name="filters">Array of Extensions for filtering.</param>
    /// <returns></returns>
    public string? ShowSaveFile(string title = "Save file", string? defaultPath = null, (string Name, string[] Extensions)[]? filters = null) {
        defaultPath ??= Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        filters ??= Array.Empty<(string, string[])>();

        string? result = null;
        string[] nativeFilters = GetNativeFilters(filters);

        Invoke(() => {
            InfiniFrameNative.EnsureSucceeded(
                InfiniFrameNative.ShowSaveFile(InstanceHandle, title, defaultPath, nativeFilters, filters.Length, null, out result),
                nameof(InfiniFrameNative.ShowSaveFile));
        });

        return result;
    }

    /// <summary>
    ///     Async version is required for InfiniFrame.Blazor
    /// </summary>
    /// <remarks>
    ///     Filter names are not used on macOS.
    /// </remarks>
    /// <exception cref="ApplicationException">
    ///     Thrown when the window is not initialized.
    /// </exception>
    /// <param name="title">Title of the dialog</param>
    /// <param name="defaultPath">Default path. Defaults to <see cref="Environment.SpecialFolder.MyDocuments" /></param>
    /// <param name="filters">Array of Extensions for filtering.</param>
    /// <param name="ct">Cancellation token for the operation</param>
    /// <returns></returns>
    public Task<string?> ShowSaveFileAsync(string title = "Choose file", string? defaultPath = null, (string Name, string[] Extensions)[]? filters = null, CancellationToken ct = default)
        => RunDialogAsync(workItem: () => ShowSaveFile(title, defaultPath, filters), ct);

    /// <summary>
    ///     Show a message dialog native to the OS.
    /// </summary>
    /// <exception cref="ApplicationException">
    ///     Thrown when the window is not initialized.
    /// </exception>
    /// <param name="title">Title of the dialog</param>
    /// <param name="text">Text of the dialog</param>
    /// <param name="buttons">Available interaction buttons <see cref="InfiniFrameDialogButtons" /></param>
    /// <param name="icon">Icon of the dialog <see cref="InfiniFrameDialogButtons" /></param>
    /// <returns>
    ///     <see cref="InfiniFrameDialogResult" />
    /// </returns>
    public InfiniFrameDialogResult ShowMessage(string title, string? text, InfiniFrameDialogButtons buttons = InfiniFrameDialogButtons.Ok, InfiniFrameDialogIcon icon = InfiniFrameDialogIcon.Info) {
        var result = InfiniFrameDialogResult.Cancel;
        Invoke(() => {
            InfiniFrameNative.EnsureSucceeded(
                InfiniFrameNative.ShowMessage(InstanceHandle, title, text ?? string.Empty, buttons, icon, out result),
                nameof(InfiniFrameNative.ShowMessage));
        });
        return result;
    }

    public bool TryResolveStaticAssetUri(string path, out Uri uri) {
        uri = null!;
        if (StaticAssets is null) return false;

        return StaticAssetSchemeHandler.TryResolveUri(
            StaticAssets.FileProvider,
            path,
            StaticAssets.BaseUri,
            StaticAssets.DefaultDocument,
            out uri);
    }

    public bool TryGetRemoteDebuggingEndpoint(out Uri? endpoint) {
        endpoint = null;
        if (!SupportsRemoteDebugging) {
            throw new PlatformNotSupportedException("Remote debugging is only supported on Windows and Linux in InfiniFrame.");
        }

        int? port = RemoteDebuggingPort;
        if (!port.HasValue || IsClosedOrClosing)
            return false;

        endpoint = RemoteDebuggingUtility.CreateEndpointUri(port.Value);
        return true;
    }

    private static Task<TResult> RunDialogAsync<TResult>(Func<TResult> workItem, CancellationToken ct = default) =>
        ct.IsCancellationRequested
            ? Task.FromCanceled<TResult>(ct)
            // Dialog calls are intentionally offloaded for Blazor flows where synchronous dialog invocation is unsafe.
            : Task.Run(workItem, ct);

    public void Initialize() {
        InfiniFrameNativeParameters startupParameters = Configuration.StartupParameters;
        int? remoteDebuggingPort = startupParameters.RemoteDebuggingPort is > 0
            ? startupParameters.RemoteDebuggingPort
            : null;

        try {
            if (remoteDebuggingPort.HasValue) {
                Logger.LogInformation(
                    "Remote debugging requested on loopback port {RemoteDebuggingPort}.",
                    remoteDebuggingPort.Value);

                if (OperatingSystem.IsLinux() && !startupParameters.DevToolsEnabled) {
                    Logger.LogInformation(
                        "Linux remote debugging keeps WebKit developer extras enabled while active."
                    );
                }
            }
            else {
                Logger.LogDebug("Remote debugging is disabled.");
            }

            RemoteDebuggingUtility.EnsureSupportedPlatform(remoteDebuggingPort);
            RemoteDebuggingUtility.ValidatePortAvailabilityOrThrow(remoteDebuggingPort, Logger);

            if (!InfiniFrameNativeParametersValidator.Validate(startupParameters, Logger)) {
                throw new ArgumentException("Startup Parameters Are Not Valid");
            }

            Events.OnWindowCreating();

            try {
                Invoke(() => {
                    if (OperatingSystem.IsWindows()) InfiniFrameNative.RegisterWin32(NativeType);
                    else if (OperatingSystem.IsMacOS()) InfiniFrameNative.RegisterMac();
                    else if (OperatingSystem.IsLinux()) {} // No specific implementation for Linux
                    else throw new PlatformNotSupportedException();
                });

                Invoke(() => {
                    InfiniFrameNative.EnsureSucceeded(
                        InfiniFrameNative.Constructor(in startupParameters, out IntPtr handle),
                        nameof(InfiniFrameNative.Constructor));

                    InstanceHandle = handle;
                });
            }
            catch (Exception ex) when (ExceptionsUtility.IsNonFatalException(ex)) {
                int lastError = OperatingSystem.IsWindows()
                    ? Marshal.GetLastWin32Error()
                    : 0;

                Logger.LogError(ex, "Error #{LastErrorCode} while creating native window", lastError);
                throw new ApplicationException($"Native code exception. Error #{lastError}", ex);
            }

            Events.OnWindowCreated();
        }
        finally {
            CustomSchemeNameMemory.FreeAll(startupParameters.CustomSchemeNames);
        }
    }

    /// <summary>
    ///     Show a native open dialog
    /// </summary>
    /// <param name="foldersOnly">Whether files are hidden</param>
    /// <param name="title">Title of the dialog</param>
    /// <param name="defaultPath">Default path. Defaults to <see cref="Environment.SpecialFolder.MyDocuments" /></param>
    /// <param name="multiSelect">Whether multiple selections are allowed</param>
    /// <param name="filters">Array of Extensions for filtering.</param>
    /// <returns>Array of paths</returns>
    private string?[] ShowOpenDialog(bool foldersOnly, string title, string? defaultPath, bool multiSelect, (string Name, string[] Extensions)[]? filters) {
        defaultPath ??= Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        filters ??= Array.Empty<(string, string[])>();

        string?[] results = Array.Empty<string?>();
        string[] nativeFilters = GetNativeFilters(filters, foldersOnly);

        Invoke(() => {
            InfiniFrameNativeInteropStatus status = foldersOnly
                ? InfiniFrameNative.ShowOpenFolder(InstanceHandle, title, defaultPath, multiSelect, out results)
                : InfiniFrameNative.ShowOpenFile(InstanceHandle, title, defaultPath, multiSelect, nativeFilters, nativeFilters.Length, out results);
            InfiniFrameNative.EnsureSucceeded(status, foldersOnly ? nameof(InfiniFrameNative.ShowOpenFolder) : nameof(InfiniFrameNative.ShowOpenFile));
        });

        return results;
    }

    /// <summary>
    ///     Returns an array of strings for native filters
    /// </summary>
    /// <param name="filters"></param>
    /// <param name="empty"></param>
    /// <returns>String array of filters</returns>
    private static string[] GetNativeFilters((string Name, string[] Extensions)[] filters, bool empty = false) {
        string[] nativeFilters = Array.Empty<string>();
        if (!empty && filters is { Length: > 0 }) {
            nativeFilters = OperatingSystem.IsMacOS()
                ? filters.SelectMany(t => t.Extensions.Select(s => s == "*" ? s : s.TrimStart('*', '.'))).ToArray()
                : filters.Select(t => $"{t.Name}|{t.Extensions.Select(s => s.StartsWith('.') ? $"*{s}" : !s.StartsWith("*.") ? $"*.{s}" : s).Aggregate((e1, e2) => $"{e1};{e2}")}").ToArray();
        }

        return nativeFilters;
    }

    #region PROPERTIES
    /// <summary>
    /// Gets the native window handle for the current platform.
    /// This property provides a platform-specific handle to the window, such as an HWND on Windows,
    /// or equivalent platform-specific handles on macOS and Linux.
    /// </summary>
    /// <remarks>
    /// The returned handle allows low-level, platform-specific operations on the native window.
    /// If the window is already closed or is in the process of closing, the property will return <see cref="IntPtr.Zero"/>.
    /// Platform-specific behavior is handled internally, and the property is only accessible when the native
    /// window initialization is complete.
    /// </remarks>
    /// <exception cref="PlatformNotSupportedException">
    /// Thrown when the property is accessed on an unsupported operating system.
    /// </exception>
    /// <returns>
    /// A platform-specific <see cref="IntPtr"/> representing the native window handle.
    /// If the window is closed or closing, it returns <see cref="IntPtr.Zero"/>.
    /// </returns>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public IntPtr WindowHandle {
        get {
            if (IsClosedOrClosing) return IntPtr.Zero;
            
            IntPtr handle;
            if (OperatingSystem.IsWindows()) {
                handle = InvokeUtility.InvokeAndReturn<IntPtr, InfiniFrameNativeInteropStatus>(
                    this,
                    InfiniFrameNative.GetWindowHandleWin32,
                    s => InfiniFrameNative.EnsureSucceeded(s, nameof(InfiniFrameNative.GetWindowHandleWin32))
                );
            }
            else if (OperatingSystem.IsMacOS()) {
                handle = InvokeUtility.InvokeAndReturn<IntPtr, InfiniFrameNativeInteropStatus>(
                    this,
                    InfiniFrameNative.GetWindowHandleMac,
                    s => InfiniFrameNative.EnsureSucceeded(s, nameof(InfiniFrameNative.GetWindowHandleMac))
                );
            
            }
            else if (OperatingSystem.IsLinux()) {
                handle = InvokeUtility.InvokeAndReturn<IntPtr, InfiniFrameNativeInteropStatus>(
                    this,
                    InfiniFrameNative.GetWindowHandleLinux,
                    s => InfiniFrameNative.EnsureSucceeded(s, nameof(InfiniFrameNative.GetWindowHandleLinux))
                );
            }
            else {
                throw new PlatformNotSupportedException();
            }

            return handle;
        }
    }

    /// <summary>
    ///     Gets a list of information for each monitor from the native window.
    ///     This property represents a list of Monitor objects associated with each display monitor.
    /// </summary>
    /// <remarks>
    ///     If called when the native instance of the window is not initialized, it will throw an ApplicationException.
    /// </remarks>
    /// <exception cref="ApplicationException">Thrown when the native instance of the window is not initialized.</exception>
    /// <returns>
    ///     A read-only list of Monitor objects representing information about each display monitor.
    /// </returns>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public ImmutableArray<InfiniMonitor> Monitors => InvokeUtility.InvokeAndReturn(this, MonitorsUtility.GetMonitors);

    /// <summary>
    ///     Retrieves the primary monitor information from the native window instance.
    /// </summary>
    /// <exception cref="ApplicationException"> Thrown when the window hasn't been initialized yet. </exception>
    /// <returns>
    ///     Returns a Monitor object representing the main monitor. The main monitor is the first monitor in the list of
    ///     available monitors.
    /// </returns>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public InfiniMonitor MainMonitor => InvokeUtility.InvokeAndReturn(this, MonitorsUtility.GetMonitors)[0];

    /// <summary>
    ///     Gets the dots per inch (DPI) for the primary display from the native window.
    /// </summary>
    /// <exception cref="ApplicationException">
    ///     An ApplicationException is thrown if the window hasn't been initialized yet.
    /// </exception>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public uint ScreenDpi => InvokeUtility.InvokeAndReturn<uint, InfiniFrameNativeInteropStatus>(
        this,
        InfiniFrameNative.GetScreenDpi,
        validateResult: s => InfiniFrameNative.EnsureSucceeded(s, nameof(InfiniFrameNative.GetScreenDpi)));

    /// <summary>
    ///     Gets a unique GUID to identify the native window.
    /// </summary>
    /// <remarks>
    ///     This property is not currently used by the InfiniFrame framework.
    /// </remarks>
    public Guid Id { get; } = Guid.NewGuid();

    public int ManagedThreadId { get; } = Environment.CurrentManagedThreadId;

    /// <summary>
    ///     Gets the value indicating whether the native window is chromeless.
    /// </summary>
    /// <remarks>
    ///     The user has to supply titlebar, border, dragging and resizing manually.
    /// </remarks>
    public bool Chromeless => Configuration.StartupParameters.Chromeless;

    /// <summary>
    ///     When true, the native window and browser control can be displayed with a transparent background.
    ///     HTML document's body background must have alpha-based value.
    ///     WebView2 on Windows can only be fully transparent or fully opaque.
    ///     By default, this is set to false.
    /// </summary>
    /// <exception cref="ApplicationException">
    ///     On Windows, thrown if trying to set a value after a native window is initialized.
    /// </exception>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool Transparent => OperatingSystem.IsWindows()
        ? Configuration.StartupParameters.Transparent// on windows it can only be set at startup
        : InvokeUtility.InvokeAndReturn<bool, InfiniFrameNativeInteropStatus>(this, InfiniFrameNative.GetTransparentEnabled, validateResult: s => InfiniFrameNative.EnsureSucceeded(s, nameof(InfiniFrameNative.GetTransparentEnabled)));

    /// <summary>
    ///     When true, the user can access the browser control's context menu.
    ///     By default, this is set to true.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool ContextMenuEnabled => InvokeUtility.InvokeAndReturn<bool, InfiniFrameNativeInteropStatus>(this, InfiniFrameNative.GetContextMenuEnabled, validateResult: s => InfiniFrameNative.EnsureSucceeded(s, nameof(InfiniFrameNative.GetContextMenuEnabled)));

    /// <summary>
    ///     When true, the user can access the browser control's developer tools.
    ///     By default, this is set to true.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool DevToolsEnabled => InvokeUtility.InvokeAndReturn<bool, InfiniFrameNativeInteropStatus>(this, InfiniFrameNative.GetDevToolsEnabled, validateResult: s => InfiniFrameNative.EnsureSucceeded(s, nameof(InfiniFrameNative.GetDevToolsEnabled)));

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool MediaAutoplayEnabled => InvokeUtility.InvokeAndReturn<bool, InfiniFrameNativeInteropStatus>(this, InfiniFrameNative.GetMediaAutoplayEnabled, validateResult: s => InfiniFrameNative.EnsureSucceeded(s, nameof(InfiniFrameNative.GetMediaAutoplayEnabled)));

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public string? UserAgent => InvokeUtility.InvokeAndReturn<string?, InfiniFrameNativeInteropStatus>(
        this,
        InfiniFrameNative.GetUserAgent,
        validateResult: s => InfiniFrameNative.EnsureSucceeded(s, nameof(InfiniFrameNative.GetUserAgent)));

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool FileSystemAccessEnabled => InvokeUtility.InvokeAndReturn<bool, InfiniFrameNativeInteropStatus>(this, InfiniFrameNative.GetFileSystemAccessEnabled, validateResult: s => InfiniFrameNative.EnsureSucceeded(s, nameof(InfiniFrameNative.GetFileSystemAccessEnabled)));

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool WebSecurityEnabled => InvokeUtility.InvokeAndReturn<bool, InfiniFrameNativeInteropStatus>(this, InfiniFrameNative.GetWebSecurityEnabled, validateResult: s => InfiniFrameNative.EnsureSucceeded(s, nameof(InfiniFrameNative.GetWebSecurityEnabled)));

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool JavascriptClipboardAccessEnabled => InvokeUtility.InvokeAndReturn<bool, InfiniFrameNativeInteropStatus>(this, InfiniFrameNative.GetJavascriptClipboardAccessEnabled, validateResult: s => InfiniFrameNative.EnsureSucceeded(s, nameof(InfiniFrameNative.GetJavascriptClipboardAccessEnabled)));

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool MediaStreamEnabled => InvokeUtility.InvokeAndReturn<bool, InfiniFrameNativeInteropStatus>(this, InfiniFrameNative.GetMediaStreamEnabled, validateResult: s => InfiniFrameNative.EnsureSucceeded(s, nameof(InfiniFrameNative.GetMediaStreamEnabled)));

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool SmoothScrollingEnabled => InvokeUtility.InvokeAndReturn<bool, InfiniFrameNativeInteropStatus>(this, InfiniFrameNative.GetSmoothScrollingEnabled, validateResult: s => InfiniFrameNative.EnsureSucceeded(s, nameof(InfiniFrameNative.GetSmoothScrollingEnabled)));

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IgnoreCertificateErrorsEnabled => InvokeUtility.InvokeAndReturn<bool, InfiniFrameNativeInteropStatus>(this, InfiniFrameNative.GetIgnoreCertificateErrorsEnabled, validateResult: s => InfiniFrameNative.EnsureSucceeded(s, nameof(InfiniFrameNative.GetIgnoreCertificateErrorsEnabled)));

    [SupportedOSPlatform("windows")]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool NotificationsEnabled => InvokeUtility.InvokeAndReturn<bool, InfiniFrameNativeInteropStatus>(this, InfiniFrameNative.GetNotificationsEnabled, validateResult: s => InfiniFrameNative.EnsureSucceeded(s, nameof(InfiniFrameNative.GetNotificationsEnabled)));

    /// <summary>
    ///     This property returns or sets the fullscreen status of the window.
    ///     When set to true, the native window will cover the entire screen, similar to kiosk mode.
    ///     By default, this is set to false.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool FullScreen => InvokeUtility.InvokeAndReturn<bool, InfiniFrameNativeInteropStatus>(this, InfiniFrameNative.GetFullScreen, validateResult: s => InfiniFrameNative.EnsureSucceeded(s, nameof(InfiniFrameNative.GetFullScreen)));

    /// <summary>
    ///     Gets whether the native browser control grants all requests for access to local resources
    ///     such as the user's camera and microphone. By default, this is set to true.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool GrantBrowserPermissions => InvokeUtility.InvokeAndReturn<bool, InfiniFrameNativeInteropStatus>(this, InfiniFrameNative.GetGrantBrowserPermissions, validateResult: s => InfiniFrameNative.EnsureSucceeded(s, nameof(InfiniFrameNative.GetGrantBrowserPermissions)));

    /// <summary>
    ///     Gets the Height property of the native window in pixels.
    ///     The default value is 0.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int Height => InvokeUtility.InvokeAndReturn(
        this,
        callback: (IntPtr handle, out int value) => InfiniFrameNative.GetSize(handle, out _, out value),
        validateResult: s => InfiniFrameNative.EnsureSucceeded(s, nameof(InfiniFrameNative.GetSize)));

    /// <summary>
    ///     Gets the icon file for the native window title bar.
    ///     The file must be located on the local machine and cannot be a URL. The default is none.
    /// </summary>
    public string IconFilePath => InvokeUtility.InvokeAndReturn<string, InfiniFrameNativeInteropStatus>(
        this,
        InfiniFrameNative.GetIconFileName,
        validateResult: s => InfiniFrameNative.EnsureSucceeded(s, nameof(InfiniFrameNative.GetIconFileName)));

    /// <summary>
    ///     Gets the native window Left (X) and Top coordinates (Y) in pixels.
    ///     Default is 0,0 that means the window will be aligned to the top-left edge of the screen.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Point Location => InvokeUtility.InvokeAndReturn(
        this,
        callback: (IntPtr handle, out Point value) => {
            InfiniFrameNativeInteropStatus status = InfiniFrameNative.GetPosition(handle, out int left, out int top);
            value = new Point(left, top);
            return status;
        },
        validateResult: s => InfiniFrameNative.EnsureSucceeded(s, nameof(InfiniFrameNative.GetPosition)));

    /// <summary>
    ///     Gets the native window Left (X) coordinate in pixels.
    ///     This represents the horizontal position of the window relative to the screen.
    ///     The default value is 0, which means the window will be aligned to the left edge of the screen.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int Left => InvokeUtility.InvokeAndReturn(
        this,
        callback: (IntPtr handle, out int value) => InfiniFrameNative.GetPosition(handle, out value, out _),
        validateResult: s => InfiniFrameNative.EnsureSucceeded(s, nameof(InfiniFrameNative.GetPosition)));

    /// <summary>
    ///     Gets whether the native window is maximized.
    ///     Default is false.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool Maximized => InvokeUtility.InvokeAndReturn<bool, InfiniFrameNativeInteropStatus>(this, InfiniFrameNative.GetMaximized, validateResult: s => InfiniFrameNative.EnsureSucceeded(s, nameof(InfiniFrameNative.GetMaximized)));

    /// <summary>
    ///     Gets whether the native window is currently within focus
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool Focused => InvokeUtility.InvokeAndReturn<bool, InfiniFrameNativeInteropStatus>(this, InfiniFrameNative.GetFocused, validateResult: s => InfiniFrameNative.EnsureSucceeded(s, nameof(InfiniFrameNative.GetFocused)));

    /// <summary>
    ///     Gets the maximum size of the native window in pixels.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Size MaxSize => InvokeUtility.InvokeAndReturn(
        this,
        callback: (IntPtr handle, out Size value) => {
            InfiniFrameNativeInteropStatus status = InfiniFrameNative.GetMaxSize(handle, out int width, out int height);
            value = new Size(width, height);
            return status;
        },
        validateResult: s => InfiniFrameNative.EnsureSucceeded(s, nameof(InfiniFrameNative.GetMaxSize)));

    /// <summary>
    ///     Gets the native window maximum height in pixels.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int MaxHeight => InvokeUtility.InvokeAndReturn(
        this,
        callback: (IntPtr handle, out int value) => InfiniFrameNative.GetMaxSize(handle, out _, out value),
        validateResult: s => InfiniFrameNative.EnsureSucceeded(s, nameof(InfiniFrameNative.GetMaxSize)));

    /// <summary>
    ///     Gets the native window maximum width in pixels.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int MaxWidth => InvokeUtility.InvokeAndReturn(
        this,
        callback: (IntPtr handle, out int value) => InfiniFrameNative.GetMaxSize(handle, out value, out _),
        validateResult: s => InfiniFrameNative.EnsureSucceeded(s, nameof(InfiniFrameNative.GetMaxSize)));

    /// <summary>
    ///     Gets whether the native window is minimized (hidden).
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool Minimized => InvokeUtility.InvokeAndReturn<bool, InfiniFrameNativeInteropStatus>(this, InfiniFrameNative.GetMinimized, validateResult: s => InfiniFrameNative.EnsureSucceeded(s, nameof(InfiniFrameNative.GetMinimized)));

    /// <summary>
    ///     Gets the minimum size of the native window in pixels.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Size MinSize => InvokeUtility.InvokeAndReturn(
        this,
        callback: (IntPtr handle, out Size value) => {
            InfiniFrameNativeInteropStatus status = InfiniFrameNative.GetMinSize(handle, out int width, out int height);
            value = new Size(width, height);
            return status;
        },
        validateResult: s => InfiniFrameNative.EnsureSucceeded(s, nameof(InfiniFrameNative.GetMinSize)));

    /// <summary>
    ///     Gets the native window minimum height in pixels.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int MinHeight => InvokeUtility.InvokeAndReturn(
        this,
        callback: (IntPtr handle, out int value) => InfiniFrameNative.GetMinSize(handle, out _, out value),
        validateResult: s => InfiniFrameNative.EnsureSucceeded(s, nameof(InfiniFrameNative.GetMinSize)));

    /// <summary>
    ///     Gets the native window minimum width in pixels.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int MinWidth => InvokeUtility.InvokeAndReturn(
        this,
        callback: (IntPtr handle, out int value) => InfiniFrameNative.GetMinSize(handle, out value, out _),
        validateResult: s => InfiniFrameNative.EnsureSucceeded(s, nameof(InfiniFrameNative.GetMinSize)));

    /// <summary>
    ///     Gets whether the user can resize the native window.
    ///     Default is true.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool Resizable => InvokeUtility.InvokeAndReturn<bool, InfiniFrameNativeInteropStatus>(this, InfiniFrameNative.GetResizable, validateResult: s => InfiniFrameNative.EnsureSucceeded(s, nameof(InfiniFrameNative.GetResizable)));

    /// <summary>
    ///     Gets the native window Size. This represents the width and the height of the window in pixels.
    ///     The default Size is 0,0.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Size Size => InvokeUtility.InvokeAndReturn(
        this,
        callback: (IntPtr handle, out Size value) => {
            InfiniFrameNativeInteropStatus status = InfiniFrameNative.GetSize(handle, out int width, out int height);
            value = new Size(width, height);
            return status;
        },
        validateResult: s => InfiniFrameNative.EnsureSucceeded(s, nameof(InfiniFrameNative.GetSize)));

    /// <summary>
    ///     Gets platform-specific initialization parameters for the native browser control on startup.
    ///     Default is none.
    ///     WINDOWS: WebView2 specific string. Space separated.
    ///     https://peter.sh/experiments/chromium-command-line-switches/
    ///     https://learn.microsoft.com/en-us/dotnet/api/microsoft.web.webview2.core.corewebview2environmentoptions.additionalbrowserarguments?view=webview2-dotnet-1.0.1938.49
    ///     viewFallbackFrom=webview2-dotnet-1.0.1901.177view%3Dwebview2-1.0.1901.177
    ///     https://www.chromium.org/developers/how-tos/run-chromium-with-flags/
    ///     LINUX: Webkit2Gtk specific string. Enter parameter names and values as JSON string.
    ///     E.g. { "set_enable_encrypted_media": true }
    ///     https://webkitgtk.org/reference/webkit2gtk/2.5.1/WebKitSettings.html
    ///     https://lazka.github.io/pgi-docs/WebKit2-4.0/classes/Settings.html
    ///     Mac: Webkit specific string. Enter parameter names and values as JSON string.
    ///     E.g. { "minimumFontSize": 8 }
    ///     https://developer.apple.com/documentation/webkit/wkwebviewconfiguration?language=objc
    ///     https://developer.apple.com/documentation/webkit/wkpreferences?language=objc
    /// </summary>
    public string? BrowserControlInitParameters => Configuration.StartupParameters.BrowserControlInitParameters;

    /// <summary>
    ///     Gets an HTML string that the browser control will render when initialized.
    ///     Default is none.
    /// </summary>
    /// <remarks>
    ///     Either StartString or StartUrl must be specified.
    /// </remarks>
    /// <seealso cref="StartUrl" />
    /// <exception cref="ApplicationException">
    ///     Thrown if trying to set a value after a native window is initialized.
    /// </exception>
    public string? StartString => Configuration.StartupParameters.StartString;

    /// <summary>
    ///     Gets a URL that the browser control will navigate to when initialized.
    ///     Default is none.
    /// </summary>
    /// <remarks>
    ///     Either StartString or StartUrl must be specified.
    /// </remarks>
    /// <seealso cref="StartString" />
    /// <exception cref="ApplicationException">
    ///     Thrown if trying to set a value after a native window is initialized.
    /// </exception>
    public string? StartUrl => Configuration.StartupParameters.StartUrl;

    /// <summary>
    ///     Gets the local path to store temp files for browser control.
    ///     Default is the user's AppDataLocal folder.
    /// </summary>
    /// <remarks>
    ///     Only available on Windows.
    /// </remarks>
    /// <exception cref="ApplicationException">
    ///     Thrown if a platform is not Windows.
    /// </exception>
    public string? TemporaryFilesPath => Configuration.StartupParameters.TemporaryFilesPath;

    /// <summary>
    ///     Gets the registration id for doing toast notifications.
    ///     The default is to use the window title.
    /// </summary>
    /// <remarks>
    ///     Only available on Windows.
    /// </remarks>
    /// <exception cref="ApplicationException">
    ///     Thrown if a platform is not Windows.
    /// </exception>
    public string? NotificationRegistrationId => Configuration.StartupParameters.NotificationRegistrationId;

    /// <summary>
    ///     Gets the native window title.
    ///     The default is "InfiniFrame".
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public string? Title => InvokeUtility.InvokeAndReturn<string?, InfiniFrameNativeInteropStatus>(
        this,
        InfiniFrameNative.GetTitle,
        validateResult: s => InfiniFrameNative.EnsureSucceeded(s, nameof(InfiniFrameNative.GetTitle)));

    /// <summary>
    ///     Gets the native window Top (Y) coordinate in pixels.
    ///     Default is 0.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int Top => InvokeUtility.InvokeAndReturn(
        this,
        callback: (IntPtr handle, out int value) => InfiniFrameNative.GetPosition(handle, out _, out value),
        validateResult: s => InfiniFrameNative.EnsureSucceeded(s, nameof(InfiniFrameNative.GetPosition)));

    /// <summary>
    ///     Gets whether the native window is always at the top of the z-order.
    ///     Default is false.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool TopMost => InvokeUtility.InvokeAndReturn<bool, InfiniFrameNativeInteropStatus>(this, InfiniFrameNative.GetTopmost, validateResult: s => InfiniFrameNative.EnsureSucceeded(s, nameof(InfiniFrameNative.GetTopmost)));

    /// <summary>
    ///     Gets the native window width in pixels.
    ///     Default is 0.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int Width => InvokeUtility.InvokeAndReturn(
        this,
        callback: (IntPtr handle, out int value) => InfiniFrameNative.GetSize(handle, out value, out _),
        validateResult: s => InfiniFrameNative.EnsureSucceeded(s, nameof(InfiniFrameNative.GetSize)));

    /// <summary>
    ///     Gets the native browser control <see cref="InfiniFrameWindow.Zoom" />.
    ///     Default is 100.
    /// </summary>
    /// <example>100 = 100%, 50 = 50%</example>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int Zoom => InvokeUtility.InvokeAndReturn<int, InfiniFrameNativeInteropStatus>(this, InfiniFrameNative.GetZoom, validateResult: s => InfiniFrameNative.EnsureSucceeded(s, nameof(InfiniFrameNative.GetZoom)));

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool ZoomEnabled => InvokeUtility.InvokeAndReturn<bool, InfiniFrameNativeInteropStatus>(this, InfiniFrameNative.GetZoomEnabled, validateResult: s => InfiniFrameNative.EnsureSucceeded(s, nameof(InfiniFrameNative.GetZoomEnabled)));

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool SupportsRemoteDebugging => RemoteDebuggingUtility.IsSupportedPlatform();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int? RemoteDebuggingPort => Configuration.StartupParameters.RemoteDebuggingPort > 0
        ? Configuration.StartupParameters.RemoteDebuggingPort
        : null;
    #endregion
}
