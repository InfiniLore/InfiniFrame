// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Native;
using InfiniFrame.StaticAssets;
using InfiniFrame.Utilities;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class InfiniFrameWindow : IInfiniFrameWindow {

    //Pointers to the type and instance.
    private static readonly Lazy<IntPtr> WindowType = new(NativeLibrary.GetMainProgramHandle);
    private int _shutdownRequested;
    private int _shutdownStarted;
    internal StaticAssetSettings? StaticAssets { get; init; }
    public required ILogger<IInfiniFrameWindow> Logger { get; init; }
    public required IServiceProvider? ServiceProvider { get; init; }
    public required IInfiniFrameWindow? Parent { get; init; }
    public required IInfiniFrameWindowEvents Events { get; init; }
    public required IInfiniFrameOptions Configuration { get; init; }
    public IInfiniFrameWindowEventsStore EventsStore => Events.EventsStore;
    
    public IntPtr NativeType => WindowType.Value;
    public IntPtr InstanceHandle { get; private set; }

    public Rectangle CachedPreFullScreenBounds { get; set; } = Rectangle.Empty;
    public Rectangle CachedPreMaximizedBounds { get; set; } = Rectangle.Empty;

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
        // If we're already on the UI thread, no need to dispatch
        if (Environment.CurrentManagedThreadId == ManagedThreadId) workItem();
        else InfiniFrameNative.Invoke(InstanceHandle, workItem.Invoke);
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
        try {
            Logger.LogDebug("Starting message loop for window.");
            Invoke(() => InfiniFrameNative.WaitForExit(InstanceHandle));
        }
        catch (Exception ex) when (IsNonFatalException(ex)) {
            int lastError = 0;
            if (OperatingSystem.IsWindows())
                lastError = Marshal.GetLastWin32Error();

            Logger.LogError(ex, "Error #{LastErrorCode} while running message loop", lastError);
            throw new ApplicationException($"Native code exception. Error # {lastError}  See inner exception for details.", ex);
        }
        finally {
            Interlocked.Exchange(ref _shutdownRequested, 1);
            Interlocked.Exchange(ref _shutdownStarted, 1);
        }
    }

    public Task WaitForCloseAsync(CancellationToken ct = default) {
        if (ct.IsCancellationRequested) return Task.FromCanceled(ct);

        WaitForClose();
        return Task.CompletedTask;
    }

    // Should only be used internally
    void IInfiniFrameWindow.MarkClosedFromNativeCallback() {
        Interlocked.Exchange(ref _shutdownRequested, 1);
        Interlocked.Exchange(ref _shutdownStarted, 1);
        InstanceHandle = IntPtr.Zero;
    }

    /// <summary>
    ///     Closes the native window.
    /// </summary>
    /// <exception cref="ApplicationException">
    ///     Thrown when the window is not initialized.
    /// </exception>
    public void Close() {
        if (Volatile.Read(ref _shutdownStarted) != 0) return;

        Logger.LogDebug(".Close()");
        Events.OnWindowClosingRequested();
        if (InstanceHandle == IntPtr.Zero) return;

        Interlocked.Exchange(ref _shutdownRequested, 1);
        Invoke(() => {
            if (InstanceHandle == IntPtr.Zero) {
                Logger.LogDebug("Window already closed");
                return;
            }

            InfiniFrameNative.Close(InstanceHandle);
            InstanceHandle = IntPtr.Zero;
        });
    }

    public Task CloseAsync(CancellationToken ct = default) {
        if (ct.IsCancellationRequested) return Task.FromCanceled(ct);
        Close();
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Send a message to the native window's native browser control's JavaScript context.
    /// </summary>
    /// <remarks>
    ///     In JavaScript, messages can be received via <code>window.__infiniframe.host.receiveCallback(callback)</code>.
    /// </remarks>
    /// <exception cref="ApplicationException">
    ///     Thrown when the window is not initialized.
    /// </exception>
    /// <param name="message">Message as string</param>
    public void SendWebMessage(string message) {
        if (Volatile.Read(ref _shutdownRequested) != 0
            || Volatile.Read(ref _shutdownStarted) != 0
            || InstanceHandle == IntPtr.Zero) {
            Logger.LogDebug("Skipping SendWebMessage during shutdown");
            return;
        }

        Logger.LogDebug(".SendWebMessage({Message})", message);
        Invoke(() => {
            if (Volatile.Read(ref _shutdownStarted) != 0 || InstanceHandle == IntPtr.Zero) {
                Logger.LogDebug("Window closed before SendWebMessage could execute");
                return;
            }

            InfiniFrameNative.SendWebMessage(InstanceHandle, message);
        });
    }

    public Task SendWebMessageAsync(string message, CancellationToken ct = default) {
        if (ct.IsCancellationRequested) return Task.FromCanceled(ct);

        SendWebMessage(message);
        return Task.CompletedTask;
    }

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
        if (Volatile.Read(ref _shutdownStarted) != 0 || InstanceHandle == IntPtr.Zero) {
            Logger.LogDebug("Skipping SendNotification during shutdown");
            return;
        }

        Logger.LogDebug(".SendNotification({Title}, {Body})", title, body);
        Invoke(() => {
            if (Volatile.Read(ref _shutdownStarted) != 0 || InstanceHandle == IntPtr.Zero) {
                Logger.LogDebug("Window closed before SendNotification could execute");
                return;
            }

            InfiniFrameNative.ShowNotification(InstanceHandle, title, body);
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
            IntPtr ptrResult = InfiniFrameNative.ShowSaveFile(InstanceHandle, title, defaultPath, nativeFilters, filters.Length, null);
            if (ptrResult == IntPtr.Zero) return;

            try {
                result = InfiniFrameNative.PtrToNativeString(ptrResult);
            }
            finally {
                InfiniFrameNative.FreeString(ptrResult);
            }
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
        Invoke(() => result = InfiniFrameNative.ShowMessage(InstanceHandle, title, text ?? string.Empty, buttons, icon));
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

    private static Task<TResult> RunDialogAsync<TResult>(Func<TResult> workItem, CancellationToken ct) {
        return ct.IsCancellationRequested 
            ? Task.FromCanceled<TResult>(ct)
            // Dialog calls are intentionally offloaded for Blazor flows where synchronous dialog invocation is unsafe.
            : Task.Run(workItem, ct);
    }

    public void Initialize() {
        InfiniFrameNativeParameters startupParameters = Configuration.StartupParameters;
        
        try {
            if (!InfiniFrameNativeParametersValidator.Validate(startupParameters, Logger)) {
                Logger.LogCritical("Startup Parameters Are Not Valid, please check the logs");
                throw new ArgumentException("Startup Parameters Are Not Valid, please check the logs");
            }

            Events.OnWindowCreating();

            // All C++ exceptions will bubble up to here.
            try {
                if (OperatingSystem.IsWindows())
                    Invoke(() => InfiniFrameNative.RegisterWin32(NativeType));
                else if (OperatingSystem.IsMacOS())
                    Invoke(InfiniFrameNative.RegisterMac);

                Invoke(() => InstanceHandle = InfiniFrameNative.Constructor(in startupParameters));
            }
            catch (Exception ex) when (IsNonFatalException(ex)) {
                int lastError = 0;
                if (OperatingSystem.IsWindows())
                    lastError = Marshal.GetLastWin32Error();

                Logger.LogError(ex, "Error #{LastErrorCode} while creating native window", lastError);
                throw new ApplicationException($"Native code exception. Error # {lastError}  See inner exception for details.", ex);
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
            IntPtr ptrResults = foldersOnly ? InfiniFrameNative.ShowOpenFolder(InstanceHandle, title, defaultPath, multiSelect, out int resultCount) : InfiniFrameNative.ShowOpenFile(InstanceHandle, title, defaultPath, multiSelect, nativeFilters, nativeFilters.Length, out resultCount);

            if (resultCount == 0 || ptrResults == IntPtr.Zero) return;

            try {
                IntPtr[] ptrArray = new IntPtr[resultCount];
                results = new string?[resultCount];
                Marshal.Copy(ptrResults, ptrArray, 0, resultCount);
                for (int i = 0; i < resultCount; i++) {
                    results[i] = InfiniFrameNative.PtrToNativeString(ptrArray[i]);
                }
            }
            finally {
                InfiniFrameNative.FreeStringArray(ptrResults, resultCount);
            }
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
    ///     Represents a property that gets the handle of the native window on a Windows platform.
    /// </summary>
    /// <remarks>
    ///     Only available on the Windows platform.
    ///     If this property is accessed from a non-Windows platform, a PlatformNotSupportedException will be thrown.
    ///     If this property is accessed before the window is initialized, an ApplicationException will be thrown.
    /// </remarks>
    /// <value>
    ///     The handle of the native window. The value is of type <see cref="IntPtr" />.
    /// </value>
    /// <exception cref="System.ApplicationException">Thrown when the window is not initialized yet.</exception>
    /// <exception cref="System.PlatformNotSupportedException">Thrown when accessed from a non-Windows platform.</exception>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public IntPtr WindowHandle => OperatingSystem.IsWindows()
        ? InvokeUtilities.InvokeAndReturn(this, InfiniFrameNative.GetWindowHandlerWin32)
        : IntPtr.Zero;

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
    public ImmutableArray<InfiniMonitor> Monitors => InvokeUtilities.InvokeAndReturn(this, MonitorsUtility.GetMonitors);

    /// <summary>
    ///     Retrieves the primary monitor information from the native window instance.
    /// </summary>
    /// <exception cref="ApplicationException"> Thrown when the window hasn't been initialized yet. </exception>
    /// <returns>
    ///     Returns a Monitor object representing the main monitor. The main monitor is the first monitor in the list of
    ///     available monitors.
    /// </returns>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public InfiniMonitor MainMonitor => InvokeUtilities.InvokeAndReturn(this, MonitorsUtility.GetMonitors)[0];

    /// <summary>
    ///     Gets the dots per inch (DPI) for the primary display from the native window.
    /// </summary>
    /// <exception cref="ApplicationException">
    ///     An ApplicationException is thrown if the window hasn't been initialized yet.
    /// </exception>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public uint ScreenDpi => InvokeUtilities.InvokeAndReturn(this, InfiniFrameNative.GetScreenDpi);

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
        : InvokeUtilities.InvokeAndReturn<bool>(this, InfiniFrameNative.GetTransparentEnabled);

    /// <summary>
    ///     When true, the user can access the browser control's context menu.
    ///     By default, this is set to true.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool ContextMenuEnabled => InvokeUtilities.InvokeAndReturn<bool>(this, InfiniFrameNative.GetContextMenuEnabled);

    /// <summary>
    ///     When true, the user can access the browser control's developer tools.
    ///     By default, this is set to true.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool DevToolsEnabled => InvokeUtilities.InvokeAndReturn<bool>(this, InfiniFrameNative.GetDevToolsEnabled);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool MediaAutoplayEnabled => InvokeUtilities.InvokeAndReturn<bool>(this, InfiniFrameNative.GetMediaAutoplayEnabled);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public string? UserAgent => InvokeUtilities.InvokeAndReturn<string?>(this, InfiniFrameNative.GetUserAgent);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool FileSystemAccessEnabled => InvokeUtilities.InvokeAndReturn<bool>(this, InfiniFrameNative.GetFileSystemAccessEnabled);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool WebSecurityEnabled => InvokeUtilities.InvokeAndReturn<bool>(this, InfiniFrameNative.GetWebSecurityEnabled);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool JavascriptClipboardAccessEnabled => InvokeUtilities.InvokeAndReturn<bool>(this, InfiniFrameNative.GetJavascriptClipboardAccessEnabled);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool MediaStreamEnabled => InvokeUtilities.InvokeAndReturn<bool>(this, InfiniFrameNative.GetMediaStreamEnabled);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool SmoothScrollingEnabled => InvokeUtilities.InvokeAndReturn<bool>(this, InfiniFrameNative.GetSmoothScrollingEnabled);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IgnoreCertificateErrorsEnabled => InvokeUtilities.InvokeAndReturn<bool>(this, InfiniFrameNative.GetIgnoreCertificateErrorsEnabled);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool NotificationsEnabled => InvokeUtilities.InvokeAndReturn<bool>(this, InfiniFrameNative.GetNotificationsEnabled);

    /// <summary>
    ///     This property returns or sets the fullscreen status of the window.
    ///     When set to true, the native window will cover the entire screen, similar to kiosk mode.
    ///     By default, this is set to false.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool FullScreen => InvokeUtilities.InvokeAndReturn<bool>(this, InfiniFrameNative.GetFullScreen);

    /// <summary>
    ///     Gets whether the native browser control grants all requests for access to local resources
    ///     such as the user's camera and microphone. By default, this is set to true.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool GrantBrowserPermissions => InvokeUtilities.InvokeAndReturn<bool>(this, InfiniFrameNative.GetGrantBrowserPermissions);

    /// <summary>
    ///     Gets the Height property of the native window in pixels.
    ///     The default value is 0.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int Height => InvokeUtilities.InvokeAndReturn<int>(this, InfiniFrameNative.GetHeight);

    /// <summary>
    ///     Gets the icon file for the native window title bar.
    ///     The file must be located on the local machine and cannot be a URL. The default is none.
    /// </summary>
    public string IconFilePath => InvokeUtilities.InvokeAndReturn<string>(this, InfiniFrameNative.GetIconFileName);

    /// <summary>
    ///     Gets the native window Left (X) and Top coordinates (Y) in pixels.
    ///     Default is 0,0 that means the window will be aligned to the top-left edge of the screen.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Point Location => InvokeUtilities.InvokeAndReturn<Point>(this, InfiniFrameNative.GetPosition);

    /// <summary>
    ///     Gets the native window Left (X) coordinate in pixels.
    ///     This represents the horizontal position of the window relative to the screen.
    ///     The default value is 0, which means the window will be aligned to the left edge of the screen.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int Left => InvokeUtilities.InvokeAndReturn<int>(this, InfiniFrameNative.GetLeft);

    /// <summary>
    ///     Gets whether the native window is maximized.
    ///     Default is false.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool Maximized => InvokeUtilities.InvokeAndReturn<bool>(this, InfiniFrameNative.GetMaximized);

    /// <summary>
    ///     Gets whether the native window is currently within focus
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool Focused => InvokeUtilities.InvokeAndReturn<bool>(this, InfiniFrameNative.GetFocused);

    /// <summary>
    ///     Gets the maximum size of the native window in pixels.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Size MaxSize => InvokeUtilities.InvokeAndReturn<Size>(this, InfiniFrameNative.GetMaxSize);

    /// <summary>
    ///     Gets the native window maximum height in pixels.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int MaxHeight => InvokeUtilities.InvokeAndReturn<int>(this, InfiniFrameNative.GetMaxHeight);

    /// <summary>
    ///     Gets the native window maximum width in pixels.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int MaxWidth => InvokeUtilities.InvokeAndReturn<int>(this, InfiniFrameNative.GetMaxWidth);

    /// <summary>
    ///     Gets whether the native window is minimized (hidden).
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool Minimized => InvokeUtilities.InvokeAndReturn<bool>(this, InfiniFrameNative.GetMinimized);

    /// <summary>
    ///     Gets the minimum size of the native window in pixels.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Size MinSize => InvokeUtilities.InvokeAndReturn<Size>(this, InfiniFrameNative.GetMinSize);

    /// <summary>
    ///     Gets the native window minimum height in pixels.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int MinHeight => InvokeUtilities.InvokeAndReturn<int>(this, InfiniFrameNative.GetMinHeight);

    /// <summary>
    ///     Gets the native window minimum width in pixels.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int MinWidth => InvokeUtilities.InvokeAndReturn<int>(this, InfiniFrameNative.GetMinWidth);

    /// <summary>
    ///     Gets whether the user can resize the native window.
    ///     Default is true.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool Resizable => InvokeUtilities.InvokeAndReturn<bool>(this, InfiniFrameNative.GetResizable);

    /// <summary>
    ///     Gets the native window Size. This represents the width and the height of the window in pixels.
    ///     The default Size is 0,0.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Size Size => InvokeUtilities.InvokeAndReturn<Size>(this, InfiniFrameNative.GetSize);

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
    public string Title => InvokeUtilities.InvokeAndReturn<string>(this, InfiniFrameNative.GetTitle);

    /// <summary>
    ///     Gets the native window Top (Y) coordinate in pixels.
    ///     Default is 0.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int Top => InvokeUtilities.InvokeAndReturn<int>(this, InfiniFrameNative.GetTop);

    /// <summary>
    ///     Gets whether the native window is always at the top of the z-order.
    ///     Default is false.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool TopMost => InvokeUtilities.InvokeAndReturn<bool>(this, InfiniFrameNative.GetTopmost);

    /// <summary>
    ///     Gets the native window width in pixels.
    ///     Default is 0.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int Width => InvokeUtilities.InvokeAndReturn<int>(this, InfiniFrameNative.GetWidth);

    /// <summary>
    ///     Gets the native browser control <see cref="InfiniFrameWindow.Zoom" />.
    ///     Default is 100.
    /// </summary>
    /// <example>100 = 100%, 50 = 50%</example>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int Zoom => InvokeUtilities.InvokeAndReturn<int>(this, InfiniFrameNative.GetZoom);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool ZoomEnabled => InvokeUtilities.InvokeAndReturn<bool>(this, InfiniFrameNative.GetZoomEnabled);

    private static bool IsNonFatalException(Exception exception)
        => exception is not (OutOfMemoryException or AccessViolationException);
    #endregion
}
