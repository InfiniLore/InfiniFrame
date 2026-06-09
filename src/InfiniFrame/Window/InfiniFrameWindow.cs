// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Debugging;
using InfiniFrame.NativeBridge;
using InfiniFrame.NativeBridge.Dialogs;
using InfiniFrame.NativeBridge.Parameters;
using InfiniFrame.StaticAssets;
using InfiniFrame.Utilities;
using Microsoft.Extensions.DependencyInjection;
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
public sealed class InfiniFrameWindow(
    ILogger<InfiniFrameWindow> logger,
    IInfiniFrameOptions configuration,
    IInfiniFrameWindowDebugging debugging,
    IInfiniFrameEvents events,
    IInfiniFrameWindowFeatures features,
    IInfiniFrameStaticAssets? staticAssets
) : IInfiniFrameWindow {
    private static readonly Lazy<IntPtr> WindowType = new(NativeLibrary.GetMainProgramHandle);

    public IInfiniFrameOptions Configuration { get; } = configuration;
    public IInfiniFrameWindowDebugging Debugging { get; } = debugging;
    public IInfiniFrameEvents Events { get; } = events;
    public IInfiniFrameWindowFeatures Features { get; } = features;
    public IInfiniFrameStaticAssets? StaticAssets { get; } = staticAssets;

    public IInfiniFrameEventsStore EventsStore => Events.EventsStore;

    public IntPtr NativeType => WindowType.Value;

    public IntPtr InstanceHandle { get; private set; }

    public Rectangle CachedPreFullScreenBounds { get; set; } = Rectangle.Empty;
    public Rectangle CachedPreMaximizedBounds { get; set; } = Rectangle.Empty;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    

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
            if (OperatingSystem.IsWindows()) handle = NativeInvoke.InvokeSyncWithValidation<IntPtr>(InstanceHandle, ManagedThreadId, InfiniFrameNative.GetWindowHandleWin32);
            else if (OperatingSystem.IsMacOS()) handle = NativeInvoke.InvokeSyncWithValidation<IntPtr>(InstanceHandle, ManagedThreadId, InfiniFrameNative.GetWindowHandleMac);
            else if (OperatingSystem.IsLinux()) handle = NativeInvoke.InvokeSyncWithValidation<IntPtr>(InstanceHandle, ManagedThreadId, InfiniFrameNative.GetWindowHandleLinux);
            else throw new PlatformNotSupportedException();

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
    public ImmutableArray<InfiniMonitor> Monitors => NativeInvoke.InvokeSyncWithValidation<ImmutableArray<InfiniMonitor>>(InstanceHandle, ManagedThreadId, MonitorsUtility.GetMonitors);

    /// <summary>
    ///     Retrieves the primary monitor information from the native window instance.
    /// </summary>
    /// <exception cref="ApplicationException"> Thrown when the window hasn't been initialized yet. </exception>
    /// <returns>
    ///     Returns a Monitor object representing the main monitor. The main monitor is the first monitor in the list of
    ///     available monitors.
    /// </returns>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public InfiniMonitor MainMonitor => NativeInvoke.InvokeSyncWithValidation<ImmutableArray<InfiniMonitor>>(InstanceHandle, ManagedThreadId, MonitorsUtility.GetMonitors).FirstOrDefault();

    /// <summary>
    ///     Gets the dots per inch (DPI) for the primary display from the native window.
    /// </summary>
    /// <exception cref="ApplicationException">
    ///     An ApplicationException is thrown if the window hasn't been initialized yet.
    /// </exception>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public uint ScreenDpi => NativeInvoke.InvokeSyncWithValidation<uint>(InstanceHandle, ManagedThreadId, InfiniFrameNative.GetScreenDpi);

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
        ? Configuration.StartupParameters.Transparent // on windows it can only be set at startup
        : NativeInvoke.InvokeSyncWithValidation<bool>(InstanceHandle, ManagedThreadId, InfiniFrameNative.GetTransparentEnabled);

    /// <summary>
    ///     When true, the user can access the browser control's context menu.
    ///     By default, this is set to true.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool ContextMenuEnabled => NativeInvoke.InvokeSyncWithValidation<bool>(InstanceHandle, ManagedThreadId, InfiniFrameNative.GetContextMenuEnabled);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool MediaAutoplayEnabled => NativeInvoke.InvokeSyncWithValidation<bool>(InstanceHandle, ManagedThreadId, InfiniFrameNative.GetMediaAutoplayEnabled);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public string? UserAgent => NativeInvoke.InvokeSyncWithValidation<string?>(InstanceHandle, ManagedThreadId,
        InfiniFrameNative.GetUserAgent);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool FileSystemAccessEnabled => NativeInvoke.InvokeSyncWithValidation<bool>(InstanceHandle, ManagedThreadId, InfiniFrameNative.GetFileSystemAccessEnabled);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool WebSecurityEnabled => NativeInvoke.InvokeSyncWithValidation<bool>(InstanceHandle, ManagedThreadId, InfiniFrameNative.GetWebSecurityEnabled);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool JavascriptClipboardAccessEnabled => NativeInvoke.InvokeSyncWithValidation<bool>(InstanceHandle, ManagedThreadId, InfiniFrameNative.GetJavascriptClipboardAccessEnabled);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool MediaStreamEnabled => NativeInvoke.InvokeSyncWithValidation<bool>(InstanceHandle, ManagedThreadId, InfiniFrameNative.GetMediaStreamEnabled);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool SmoothScrollingEnabled => NativeInvoke.InvokeSyncWithValidation<bool>(InstanceHandle, ManagedThreadId, InfiniFrameNative.GetSmoothScrollingEnabled);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IgnoreCertificateErrorsEnabled => NativeInvoke.InvokeSyncWithValidation<bool>(InstanceHandle, ManagedThreadId, InfiniFrameNative.GetIgnoreCertificateErrorsEnabled);

    [SupportedOSPlatform("windows")]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool NotificationsEnabled => NativeInvoke.InvokeSyncWithValidation<bool>(InstanceHandle, ManagedThreadId, InfiniFrameNative.GetNotificationsEnabled);

    /// <summary>
    ///     This property returns or sets the fullscreen status of the window.
    ///     When set to true, the native window will cover the entire screen, similar to kiosk mode.
    ///     By default, this is set to false.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool FullScreen => NativeInvoke.InvokeSyncWithValidation<bool>(InstanceHandle, ManagedThreadId, InfiniFrameNative.GetFullScreen);

    /// <summary>
    ///     Gets whether the native browser control grants all requests for access to local resources
    ///     such as the user's camera and microphone. By default, this is set to true.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool GrantBrowserPermissions => NativeInvoke.InvokeSyncWithValidation<bool>(InstanceHandle, ManagedThreadId, InfiniFrameNative.GetGrantBrowserPermissions);

    /// <summary>
    ///     Gets the Height property of the native window in pixels.
    ///     The default value is 0.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int Height => NativeInvoke.InvokeSyncWithValidation(InstanceHandle, ManagedThreadId,
        callback: (IntPtr handle, out int value) => InfiniFrameNative.GetSize(handle, out _, out value));

    /// <summary>
    ///     Gets the icon file for the native window title bar.
    ///     The file must be located on the local machine and cannot be a URL. The default is none.
    /// </summary>
    public string? IconFilePath => NativeInvoke.InvokeSyncWithValidation<string?>(InstanceHandle, ManagedThreadId, InfiniFrameNative.GetIconFileName);

    /// <summary>
    ///     Gets the native window Left (X) and Top coordinates (Y) in pixels.
    ///     Default is 0,0 that means the window will be aligned to the top-left edge of the screen.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Point Location => NativeInvoke.InvokeSyncWithValidation(InstanceHandle, ManagedThreadId,
        callback: (IntPtr handle, out Point value) => {
            InfiniFrameNativeInteropStatus status = InfiniFrameNative.GetPosition(handle, out int left, out int top);
            value = new Point(left, top);
            return status;
        });

    /// <summary>
    ///     Gets the native window Left (X) coordinate in pixels.
    ///     This represents the horizontal position of the window relative to the screen.
    ///     The default value is 0, which means the window will be aligned to the left edge of the screen.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int Left => NativeInvoke.InvokeSyncWithValidation(InstanceHandle, ManagedThreadId,
        callback: (IntPtr handle, out int value) => InfiniFrameNative.GetPosition(handle, out value, out _));

    /// <summary>
    ///     Gets whether the native window is maximized.
    ///     Default is false.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool Maximized => NativeInvoke.InvokeSyncWithValidation<bool>(InstanceHandle, ManagedThreadId, InfiniFrameNative.GetMaximized);

    /// <summary>
    ///     Gets whether the native window is currently within focus
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool Focused => NativeInvoke.InvokeSyncWithValidation<bool>(InstanceHandle, ManagedThreadId, InfiniFrameNative.GetFocused);

    /// <summary>
    ///     Gets the maximum size of the native window in pixels.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Size MaxSize => NativeInvoke.InvokeSyncWithValidation(InstanceHandle, ManagedThreadId,
        callback: (IntPtr handle, out Size value) => {
            InfiniFrameNativeInteropStatus status = InfiniFrameNative.GetMaxSize(handle, out int width, out int height);
            value = new Size(width, height);
            return status;
        });

    /// <summary>
    ///     Gets the native window maximum height in pixels.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int MaxHeight => NativeInvoke.InvokeSyncWithValidation(InstanceHandle, ManagedThreadId,
        callback: (IntPtr handle, out int value) => InfiniFrameNative.GetMaxSize(handle, out _, out value));

    /// <summary>
    ///     Gets the native window maximum width in pixels.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int MaxWidth => NativeInvoke.InvokeSyncWithValidation(InstanceHandle, ManagedThreadId,
        callback: (IntPtr handle, out int value) => InfiniFrameNative.GetMaxSize(handle, out value, out _));

    /// <summary>
    ///     Gets whether the native window is minimized (hidden).
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool Minimized => NativeInvoke.InvokeSyncWithValidation<bool>(InstanceHandle, ManagedThreadId, InfiniFrameNative.GetMinimized);

    /// <summary>
    ///     Gets the minimum size of the native window in pixels.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Size MinSize => NativeInvoke.InvokeSyncWithValidation(InstanceHandle, ManagedThreadId,
        callback: (IntPtr handle, out Size value) => {
            InfiniFrameNativeInteropStatus status = InfiniFrameNative.GetMinSize(handle, out int width, out int height);
            value = new Size(width, height);
            return status;
        });

    /// <summary>
    ///     Gets the native window minimum height in pixels.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int MinHeight => NativeInvoke.InvokeSyncWithValidation(InstanceHandle, ManagedThreadId,
        callback: (IntPtr handle, out int value) => InfiniFrameNative.GetMinSize(handle, out _, out value));

    /// <summary>
    ///     Gets the native window minimum width in pixels.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int MinWidth => NativeInvoke.InvokeSyncWithValidation(InstanceHandle, ManagedThreadId,
        callback: (IntPtr handle, out int value) => InfiniFrameNative.GetMinSize(handle, out value, out _));

    /// <summary>
    ///     Gets whether the user can resize the native window.
    ///     Default is true.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool Resizable => NativeInvoke.InvokeSyncWithValidation<bool>(InstanceHandle, ManagedThreadId, InfiniFrameNative.GetResizable);

    /// <summary>
    ///     Gets the native window Size. This represents the width and the height of the window in pixels.
    ///     The default Size is 0,0.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Size Size => NativeInvoke.InvokeSyncWithValidation(InstanceHandle, ManagedThreadId,
        callback: (IntPtr handle, out Size value) => {
            InfiniFrameNativeInteropStatus status = InfiniFrameNative.GetSize(handle, out int width, out int height);
            value = new Size(width, height);
            return status;
        });

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
    public string? Title => NativeInvoke.InvokeSyncWithValidation<string?>(InstanceHandle, ManagedThreadId,
        InfiniFrameNative.GetTitle);

    /// <summary>
    ///     Gets the native window Top (Y) coordinate in pixels.
    ///     Default is 0.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int Top => NativeInvoke.InvokeSyncWithValidation(InstanceHandle, ManagedThreadId,
        callback: (IntPtr handle, out int value) => InfiniFrameNative.GetPosition(handle, out _, out value));

    /// <summary>
    ///     Gets whether the native window is always at the top of the z-order.
    ///     Default is false.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool TopMost => NativeInvoke.InvokeSyncWithValidation<bool>(InstanceHandle, ManagedThreadId, InfiniFrameNative.GetTopmost);

    /// <summary>
    ///     Gets the native window width in pixels.
    ///     Default is 0.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int Width => NativeInvoke.InvokeSyncWithValidation(InstanceHandle, ManagedThreadId,
        callback: (IntPtr handle, out int value) => InfiniFrameNative.GetSize(handle, out value, out _));

    /// <summary>
    ///     Gets the native browser control <see cref="InfiniFrameWindow.Zoom" />.
    ///     Default is 100.
    /// </summary>
    /// <example>100 = 100%, 50 = 50%</example>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int Zoom => NativeInvoke.InvokeSyncWithValidation<int>(InstanceHandle, ManagedThreadId, InfiniFrameNative.GetZoom);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool ZoomEnabled => NativeInvoke.InvokeSyncWithValidation<bool>(InstanceHandle, ManagedThreadId, InfiniFrameNative.GetZoomEnabled);

    #endregion
}
