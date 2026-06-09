// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Debugging;
using InfiniFrame.NativeBridge;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class InfiniFrameWindow(
    IInfiniFrameOptions configuration,
    IInfiniFrameWindowDebugging debugging,
    IInfiniFrameEvents events,
    IInfiniFrameWindowFeatures features
) : IInfiniFrameWindow {
    private static readonly Lazy<IntPtr> WindowType = new(NativeLibrary.GetMainProgramHandle);

    public IInfiniFrameOptions Configuration { get; } = configuration;
    public IInfiniFrameWindowDebugging Debugging { get; } = debugging;
    public IInfiniFrameEvents Events { get; } = events;
    public IInfiniFrameWindowFeatures Features { get; } = features;

    public IInfiniFrameEventsStore EventsStore => Events.EventsStore;

    public IntPtr NativeType => WindowType.Value;

    private IntPtr InstanceHandle { get; set; }
    IntPtr IInfiniFrameWindow.InstanceHandle {
        get => InstanceHandle;
        set => InstanceHandle = value;
    }

    public Rectangle CachedPreFullScreenBounds { get; set; } = Rectangle.Empty;
    public Rectangle CachedPreMaximizedBounds { get; set; } = Rectangle.Empty;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    #region PROPERTIES
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public IntPtr WindowHandle {
        get {
            if (Features.Lifecycle.IsClosedOrClosing()) return IntPtr.Zero;
            
            IntPtr handle;
            if (OperatingSystem.IsWindows()) handle = NativeInvoke.InvokeSyncWithValidation<IntPtr>(InstanceHandle, ManagedThreadId, InfiniFrameNative.GetWindowHandleWin32);
            else if (OperatingSystem.IsMacOS()) handle = NativeInvoke.InvokeSyncWithValidation<IntPtr>(InstanceHandle, ManagedThreadId, InfiniFrameNative.GetWindowHandleMac);
            else if (OperatingSystem.IsLinux()) handle = NativeInvoke.InvokeSyncWithValidation<IntPtr>(InstanceHandle, ManagedThreadId, InfiniFrameNative.GetWindowHandleLinux);
            else throw new PlatformNotSupportedException();

            return handle;
        }
    }

    
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Guid Id { get; } = Guid.NewGuid();

    public int ManagedThreadId { get; } = Environment.CurrentManagedThreadId;
    
    public bool Chromeless => Configuration.StartupParameters.Chromeless;
    
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool Transparent => OperatingSystem.IsWindows()
        ? Configuration.StartupParameters.Transparent // on windows it can only be set at startup
        : NativeInvoke.InvokeSyncWithValidation<bool>(InstanceHandle, ManagedThreadId, InfiniFrameNative.GetTransparentEnabled);
    
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
    
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool FullScreen => NativeInvoke.InvokeSyncWithValidation<bool>(InstanceHandle, ManagedThreadId, InfiniFrameNative.GetFullScreen);
    
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool GrantBrowserPermissions => NativeInvoke.InvokeSyncWithValidation<bool>(InstanceHandle, ManagedThreadId, InfiniFrameNative.GetGrantBrowserPermissions);
    
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int Height => NativeInvoke.InvokeSyncWithValidation(InstanceHandle, ManagedThreadId,
        callback: (IntPtr handle, out int value) => InfiniFrameNative.GetSize(handle, out _, out value));
    
    public string? IconFilePath => NativeInvoke.InvokeSyncWithValidation<string?>(InstanceHandle, ManagedThreadId, InfiniFrameNative.GetIconFileName);
    
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Point Location => NativeInvoke.InvokeSyncWithValidation(InstanceHandle, ManagedThreadId,
        callback: (IntPtr handle, out Point value) => {
            InfiniFrameNativeInteropStatus status = InfiniFrameNative.GetPosition(handle, out int left, out int top);
            value = new Point(left, top);
            return status;
        });
    
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int Left => NativeInvoke.InvokeSyncWithValidation(InstanceHandle, ManagedThreadId,
        callback: (IntPtr handle, out int value) => InfiniFrameNative.GetPosition(handle, out value, out _));
    
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool Maximized => NativeInvoke.InvokeSyncWithValidation<bool>(InstanceHandle, ManagedThreadId, InfiniFrameNative.GetMaximized);
    
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool Focused => NativeInvoke.InvokeSyncWithValidation<bool>(InstanceHandle, ManagedThreadId, InfiniFrameNative.GetFocused);
    
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Size MaxSize => NativeInvoke.InvokeSyncWithValidation(InstanceHandle, ManagedThreadId,
        callback: (IntPtr handle, out Size value) => {
            InfiniFrameNativeInteropStatus status = InfiniFrameNative.GetMaxSize(handle, out int width, out int height);
            value = new Size(width, height);
            return status;
        });
    
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int MaxHeight => NativeInvoke.InvokeSyncWithValidation(InstanceHandle, ManagedThreadId,
        callback: (IntPtr handle, out int value) => InfiniFrameNative.GetMaxSize(handle, out _, out value));
    
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int MaxWidth => NativeInvoke.InvokeSyncWithValidation(InstanceHandle, ManagedThreadId,
        callback: (IntPtr handle, out int value) => InfiniFrameNative.GetMaxSize(handle, out value, out _));
    
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool Minimized => NativeInvoke.InvokeSyncWithValidation<bool>(InstanceHandle, ManagedThreadId, InfiniFrameNative.GetMinimized);
    
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Size MinSize => NativeInvoke.InvokeSyncWithValidation(InstanceHandle, ManagedThreadId,
        callback: (IntPtr handle, out Size value) => {
            InfiniFrameNativeInteropStatus status = InfiniFrameNative.GetMinSize(handle, out int width, out int height);
            value = new Size(width, height);
            return status;
        });
    
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int MinHeight => NativeInvoke.InvokeSyncWithValidation(InstanceHandle, ManagedThreadId,
        callback: (IntPtr handle, out int value) => InfiniFrameNative.GetMinSize(handle, out _, out value));
    
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int MinWidth => NativeInvoke.InvokeSyncWithValidation(InstanceHandle, ManagedThreadId,
        callback: (IntPtr handle, out int value) => InfiniFrameNative.GetMinSize(handle, out value, out _));
    
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool Resizable => NativeInvoke.InvokeSyncWithValidation<bool>(InstanceHandle, ManagedThreadId, InfiniFrameNative.GetResizable);
    
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Size Size => NativeInvoke.InvokeSyncWithValidation(InstanceHandle, ManagedThreadId,
        callback: (IntPtr handle, out Size value) => {
            InfiniFrameNativeInteropStatus status = InfiniFrameNative.GetSize(handle, out int width, out int height);
            value = new Size(width, height);
            return status;
        });
    
    public string? BrowserControlInitParameters => Configuration.StartupParameters.BrowserControlInitParameters;
    
    public string? StartString => Configuration.StartupParameters.StartString;
    
    public string? StartUrl => Configuration.StartupParameters.StartUrl;
    
    public string? TemporaryFilesPath => Configuration.StartupParameters.TemporaryFilesPath;
    
    public string? NotificationRegistrationId => Configuration.StartupParameters.NotificationRegistrationId;
    
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public string? Title => NativeInvoke.InvokeSyncWithValidation<string?>(InstanceHandle, ManagedThreadId,
        InfiniFrameNative.GetTitle);
    
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int Top => NativeInvoke.InvokeSyncWithValidation(InstanceHandle, ManagedThreadId,
        callback: (IntPtr handle, out int value) => InfiniFrameNative.GetPosition(handle, out _, out value));
    
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool TopMost => NativeInvoke.InvokeSyncWithValidation<bool>(InstanceHandle, ManagedThreadId, InfiniFrameNative.GetTopmost);
    
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int Width => NativeInvoke.InvokeSyncWithValidation(InstanceHandle, ManagedThreadId,
        callback: (IntPtr handle, out int value) => InfiniFrameNative.GetSize(handle, out value, out _));
    
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int Zoom => NativeInvoke.InvokeSyncWithValidation<int>(InstanceHandle, ManagedThreadId, InfiniFrameNative.GetZoom);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool ZoomEnabled => NativeInvoke.InvokeSyncWithValidation<bool>(InstanceHandle, ManagedThreadId, InfiniFrameNative.GetZoomEnabled);

    #endregion
}
