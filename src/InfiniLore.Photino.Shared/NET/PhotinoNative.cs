using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace InfiniLore.Photino.NET;
public static partial class PhotinoNative {
    private const string DllName = "Photino.Native";

    // ReSharper disable InconsistentNaming
    private const string Photino_register_win32 = nameof(Photino_register_win32);
    private const string Photino_register_mac = nameof(Photino_register_mac);
    private const string Photino_ctor = nameof(Photino_ctor);
    private const string Photino_AddCustomSchemeName = nameof(Photino_AddCustomSchemeName);
    private const string Photino_Close = nameof(Photino_Close);
    private const string Photino_getHwnd_win32 = nameof(Photino_getHwnd_win32);
    private const string Photino_GetAllMonitors = nameof(Photino_GetAllMonitors);
    private const string Photino_GetTransparentEnabled = nameof(Photino_GetTransparentEnabled);
    private const string Photino_GetContextMenuEnabled = nameof(Photino_GetContextMenuEnabled);
    private const string Photino_GetDevToolsEnabled = nameof(Photino_GetDevToolsEnabled);
    private const string Photino_GetFullScreen = nameof(Photino_GetFullScreen);
    private const string Photino_GetGrantBrowserPermissions = nameof(Photino_GetGrantBrowserPermissions);
    private const string Photino_GetUserAgent = nameof(Photino_GetUserAgent);
    private const string Photino_GetMediaAutoplayEnabled = nameof(Photino_GetMediaAutoplayEnabled);
    private const string Photino_GetFileSystemAccessEnabled = nameof(Photino_GetFileSystemAccessEnabled);
    private const string Photino_GetWebSecurityEnabled = nameof(Photino_GetWebSecurityEnabled);
    private const string Photino_GetJavascriptClipboardAccessEnabled = nameof(Photino_GetJavascriptClipboardAccessEnabled);
    private const string Photino_GetMediaStreamEnabled = nameof(Photino_GetMediaStreamEnabled);
    private const string Photino_GetSmoothScrollingEnabled = nameof(Photino_GetSmoothScrollingEnabled);
    private const string Photino_GetIgnoreCertificateErrorsEnabled = nameof(Photino_GetIgnoreCertificateErrorsEnabled);
    private const string Photino_GetNotificationsEnabled = nameof(Photino_GetNotificationsEnabled);
    private const string Photino_GetPosition = nameof(Photino_GetPosition);
    private const string Photino_GetResizable = nameof(Photino_GetResizable);
    private const string Photino_GetScreenDpi = nameof(Photino_GetScreenDpi);
    private const string Photino_GetSize = nameof(Photino_GetSize);
    private const string Photino_GetTitle = nameof(Photino_GetTitle);
    private const string Photino_GetTopmost = nameof(Photino_GetTopmost);
    private const string Photino_GetZoom = nameof(Photino_GetZoom);
    private const string Photino_GetMaximized = nameof(Photino_GetMaximized);
    private const string Photino_GetMinimized = nameof(Photino_GetMinimized);
    private const string Photino_Invoke = nameof(Photino_Invoke);
    private const string Photino_NavigateToString = nameof(Photino_NavigateToString);
    private const string Photino_NavigateToUrl = nameof(Photino_NavigateToUrl);
    private const string Photino_setWebView2RuntimePath_win32 = nameof(Photino_setWebView2RuntimePath_win32);
    private const string Photino_SetTransparentEnabled = nameof(Photino_SetTransparentEnabled);
    private const string Photino_SetContextMenuEnabled = nameof(Photino_SetContextMenuEnabled);
    private const string Photino_SetDevToolsEnabled = nameof(Photino_SetDevToolsEnabled);
    private const string Photino_SetFullScreen = nameof(Photino_SetFullScreen);
    private const string Photino_SetGrantBrowserPermissions = nameof(Photino_SetGrantBrowserPermissions);
    private const string Photino_SetMaximized = nameof(Photino_SetMaximized);
    private const string Photino_SetMaxSize = nameof(Photino_SetMaxSize);
    private const string Photino_SetMinimized = nameof(Photino_SetMinimized);
    private const string Photino_SetMinSize = nameof(Photino_SetMinSize);
    private const string Photino_SetResizable = nameof(Photino_SetResizable);
    private const string Photino_SetPosition = nameof(Photino_SetPosition);
    private const string Photino_SetSize = nameof(Photino_SetSize);
    private const string Photino_SetTitle = nameof(Photino_SetTitle);
    private const string Photino_SetTopmost = nameof(Photino_SetTopmost);
    private const string Photino_SetIconFile = nameof(Photino_SetIconFile);
    private const string Photino_SetZoom = nameof(Photino_SetZoom);
    private const string Photino_Center = nameof(Photino_Center);
    private const string Photino_ClearBrowserAutoFill = nameof(Photino_ClearBrowserAutoFill);
    private const string Photino_SendWebMessage = nameof(Photino_SendWebMessage);
    private const string Photino_ShowNotification = nameof(Photino_ShowNotification);
    private const string Photino_WaitForExit = nameof(Photino_WaitForExit);
    private const string Photino_ShowOpenFile = nameof(Photino_ShowOpenFile);
    private const string Photino_ShowOpenFolder = nameof(Photino_ShowOpenFolder);
    private const string Photino_ShowSaveFile = nameof(Photino_ShowSaveFile);
    private const string Photino_ShowMessage = nameof(Photino_ShowMessage);
    private const string Photino_GetZoomEnabled = nameof(Photino_GetZoomEnabled);
    private const string Photino_SetZoomEnabled = nameof(Photino_SetZoomEnabled);
    // ReSharper restore InconsistentNaming

    #region Register
    // ReSharper disable once UnusedMethodReturnValue.Local
    [LibraryImport(DllName, EntryPoint = Photino_register_win32, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial IntPtr RegisterWin32(IntPtr hInstance);

    // ReSharper disable once UnusedMethodReturnValue.Local
    [LibraryImport(DllName, EntryPoint = Photino_register_mac, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial IntPtr RegisterMac();
    #endregion

    #region CTOR-DTOR
    //Not useful to use LibraryImport when passing a user-defined type.
    //See https://stackoverflow.com/questions/77770231/libraryimport-the-type-is-not-supported-by-source-generated-p-invokes
    [DllImport(DllName, EntryPoint = Photino_ctor, CallingConvention = CallingConvention.Cdecl, SetLastError = true, CharSet = CharSet.Ansi)]
    internal static extern IntPtr Ctor(ref PhotinoNativeParameters parameters);

    //necessary?
    // [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    // static extern void Photino_dtor(IntPtr instance);  

    [LibraryImport(DllName, EntryPoint = Photino_AddCustomSchemeName, SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void AddCustomSchemeName(IntPtr instance, string scheme);

    [LibraryImport(DllName, EntryPoint = Photino_Close, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void Close(IntPtr instance);
    #endregion

    #region Get
    [LibraryImport(DllName, EntryPoint = Photino_getHwnd_win32, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial IntPtr GetWindowHandlerWin32(IntPtr instance);

    [LibraryImport(DllName, EntryPoint = Photino_GetAllMonitors, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void GetAllMonitors(IntPtr instance, CppGetAllMonitorsDelegate callback);

    [LibraryImport(DllName, EntryPoint = Photino_GetTransparentEnabled, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void GetTransparentEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(DllName, EntryPoint = Photino_GetContextMenuEnabled, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void GetContextMenuEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(DllName, EntryPoint = Photino_GetDevToolsEnabled, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void GetDevToolsEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(DllName, EntryPoint = Photino_GetFullScreen, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void GetFullScreen(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool fullScreen);

    [LibraryImport(DllName, EntryPoint = Photino_GetGrantBrowserPermissions, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void GetGrantBrowserPermissions(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool grant);

    [LibraryImport(DllName, EntryPoint = Photino_GetUserAgent, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial IntPtr GetUserAgent(IntPtr instance);

    [LibraryImport(DllName, EntryPoint = Photino_GetMediaAutoplayEnabled, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void GetMediaAutoplayEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(DllName, EntryPoint = Photino_GetFileSystemAccessEnabled, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void GetFileSystemAccessEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(DllName, EntryPoint = Photino_GetWebSecurityEnabled, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void GetWebSecurityEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(DllName, EntryPoint = Photino_GetJavascriptClipboardAccessEnabled, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void GetJavascriptClipboardAccessEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(DllName, EntryPoint = Photino_GetMediaStreamEnabled, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void GetMediaStreamEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(DllName, EntryPoint = Photino_GetSmoothScrollingEnabled, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void GetSmoothScrollingEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(DllName, EntryPoint = Photino_GetIgnoreCertificateErrorsEnabled, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void GetIgnoreCertificateErrorsEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(DllName, EntryPoint = Photino_GetNotificationsEnabled, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void GetNotificationsEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(DllName, EntryPoint = Photino_GetPosition, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void GetPosition(IntPtr instance, out int x, out int y);

    [DllImport(DllName, EntryPoint = Photino_GetResizable, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
    internal static extern void GetResizable(IntPtr instance, out bool resizable);

    [LibraryImport(DllName, EntryPoint = Photino_GetScreenDpi, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial uint GetScreenDpi(IntPtr instance);

    [LibraryImport(DllName, EntryPoint = Photino_GetSize, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void GetSize(IntPtr instance, out int width, out int height);

    [LibraryImport(DllName, EntryPoint = Photino_GetTitle, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial IntPtr GetTitle(IntPtr instance);

    [LibraryImport(DllName, EntryPoint = Photino_GetTopmost, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void GetTopmost(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool topmost);

    [LibraryImport(DllName, EntryPoint = Photino_GetZoom, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void GetZoom(IntPtr instance, out int zoom);

    [DllImport(DllName, EntryPoint = Photino_GetMaximized, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
    internal static extern void GetMaximized(IntPtr instance, out bool maximized);

    [DllImport(DllName, EntryPoint = Photino_GetMinimized, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
    internal static extern void GetMinimized(IntPtr instance, out bool minimized);
    
    [DllImport(DllName, EntryPoint = Photino_GetZoomEnabled, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
    internal static extern void GetZoomEnabled(IntPtr instance, out bool zoomEnabled);
    #endregion

    #region MARSHAL CALLS FROM Non-UI Thread to UI Thread
    [LibraryImport(DllName, EntryPoint = Photino_Invoke, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void Invoke(IntPtr instance, InvokeCallback callback);
    #endregion

    #region Navigate
    [LibraryImport(DllName, EntryPoint = Photino_NavigateToString, SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void NavigateToString(IntPtr instance, string content);

    [LibraryImport(DllName, EntryPoint = Photino_NavigateToUrl, SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void NavigateToUrl(IntPtr instance, string url);
    #endregion

    #region Set
    [LibraryImport(DllName, EntryPoint = Photino_setWebView2RuntimePath_win32, SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void SetWebView2RuntimePath_win32(IntPtr instance, string webView2RuntimePath);

    [LibraryImport(DllName, EntryPoint = Photino_SetTransparentEnabled, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void SetTransparentEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool enabled);

    [LibraryImport(DllName, EntryPoint = Photino_SetContextMenuEnabled, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void SetContextMenuEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool enabled);

    [LibraryImport(DllName, EntryPoint = Photino_SetDevToolsEnabled, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void SetDevToolsEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool enabled);

    [LibraryImport(DllName, EntryPoint = Photino_SetFullScreen, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void SetFullScreen(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool fullScreen);

    // ReSharper disable once UnusedMember.Local
    [LibraryImport(DllName, EntryPoint = Photino_SetGrantBrowserPermissions, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void SetGrantBrowserPermissions(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool grant);

    [LibraryImport(DllName, EntryPoint = Photino_SetMaximized, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void SetMaximized(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool maximized);

    [LibraryImport(DllName, EntryPoint = Photino_SetMaxSize, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void SetMaxSize(IntPtr instance, int maxWidth, int maxHeight);

    [LibraryImport(DllName, EntryPoint = Photino_SetMinimized, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void SetMinimized(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool minimized);

    [LibraryImport(DllName, EntryPoint = Photino_SetMinSize, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void SetMinSize(IntPtr instance, int minWidth, int minHeight);

    [LibraryImport(DllName, EntryPoint = Photino_SetResizable, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void SetResizable(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool resizable);

    [LibraryImport(DllName, EntryPoint = Photino_SetPosition, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void SetPosition(IntPtr instance, int x, int y);

    [LibraryImport(DllName, EntryPoint = Photino_SetSize, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void SetSize(IntPtr instance, int width, int height);

    [LibraryImport(DllName, EntryPoint = Photino_SetTitle, SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void SetTitle(IntPtr instance, string title);

    [LibraryImport(DllName, EntryPoint = Photino_SetTopmost, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void SetTopmost(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool topmost);

    [LibraryImport(DllName, EntryPoint = Photino_SetIconFile, SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void SetIconFile(IntPtr instance, string filename);

    [LibraryImport(DllName, EntryPoint = Photino_SetZoom, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void SetZoom(IntPtr instance, int zoom);
    
    [DllImport(DllName, EntryPoint = Photino_SetZoomEnabled, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
    internal static extern void SetZoomEnabled(IntPtr instance, bool zoomEnabled);
    #endregion

    #region Misc
    [LibraryImport(DllName, EntryPoint = Photino_Center, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void Center(IntPtr instance);

    [LibraryImport(DllName, EntryPoint = Photino_ClearBrowserAutoFill, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void ClearBrowserAutoFill(IntPtr instance);

    [LibraryImport(DllName, EntryPoint = Photino_SendWebMessage, SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void SendWebMessage(IntPtr instance, string message);

    // ReSharper disable once UnusedMember.Local
    [LibraryImport(DllName, EntryPoint = Photino_ShowMessage, SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void ShowMessage(IntPtr instance, string title, string body, uint type);

    [LibraryImport(DllName, EntryPoint = Photino_ShowNotification, SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void ShowNotification(IntPtr instance, string title, string body);

    [LibraryImport(DllName, EntryPoint = Photino_WaitForExit, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial void WaitForExit(IntPtr instance);
    #endregion

    #region Dialog
    [LibraryImport(DllName, EntryPoint = Photino_ShowOpenFile, SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial IntPtr ShowOpenFile(IntPtr inst, string title, string defaultPath, [MarshalAs(UnmanagedType.I1)] bool multiSelect, string[] filters, int filtersCount, out int resultCount);

    [LibraryImport(DllName, EntryPoint = Photino_ShowOpenFolder, SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial IntPtr ShowOpenFolder(IntPtr inst, string title, string defaultPath, [MarshalAs(UnmanagedType.I1)] bool multiSelect, out int resultCount);

    [LibraryImport(DllName, EntryPoint = Photino_ShowSaveFile, SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial IntPtr ShowSaveFile(IntPtr inst, string title, string defaultPath, string[] filters, int filtersCount);

    [LibraryImport(DllName, EntryPoint = Photino_ShowMessage, SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvCdecl) })]
    internal static partial PhotinoDialogResult ShowMessage(IntPtr inst, string title, string text, PhotinoDialogButtons buttons, PhotinoDialogIcon icon);
    #endregion

    #region Overloads
    internal static void GetHeight(IntPtr instance, out int height) => GetSize(instance, out _, out height);
    internal static void GetWidth(IntPtr instance, out int width) => GetSize(instance, out width, out _);

    internal static void GetLeft(IntPtr instance, out int left) => GetPosition(instance, out left, out _);
    internal static void GetTop(IntPtr instance, out int top) => GetPosition(instance, out _, out top);

    internal static void GetSize(IntPtr instance, out Size size) {
        GetSize(instance, out int width, out int height);
        size = new Size(width, height);
    }

    internal static void GetPosition(IntPtr instance, out Point position) {
        GetPosition(instance, out int left, out int top);
        position = new Point(left, top);
    }

    internal static void GetWindowRectangle(IntPtr instance, out int x, out int y, out int width, out int height) {
        GetSize(instance, out width, out height);
        GetPosition(instance, out x, out y);
    }

    internal static void GetWindowRectangle(IntPtr instance, out Rectangle rectangle) {
        GetWindowRectangle(instance, out int x, out int y, out int width, out int height);
        rectangle = new Rectangle(x, y, width, height);
    }

    internal static void GetUserAgent(IntPtr instance, out string? userAgent) {
        IntPtr ptr = GetUserAgent(instance);
        userAgent = Marshal.PtrToStringAuto(ptr);
    }

    internal static void GetTitle(IntPtr instance, out string? title) {
        IntPtr ptr = GetTitle(instance);
        title = Marshal.PtrToStringAuto(ptr);
    }
    #endregion
}
