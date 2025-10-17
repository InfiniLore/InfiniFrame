// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static InfiniLore.InfiniFrame.Native.NativeDll;

namespace InfiniLore.InfiniFrame.Native;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static partial class InfiniFrameNative {
    #region Register
    // ReSharper disable once UnusedMethodReturnValue.Local
    [LibraryImport(DllName, EntryPoint = Photino_register_win32, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial IntPtr RegisterWin32(IntPtr hInstance);

    // ReSharper disable once UnusedMethodReturnValue.Local
    [LibraryImport(DllName, EntryPoint = Photino_register_mac, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial IntPtr RegisterMac();
    #endregion

    #region CTOR-DTOR
    #pragma warning disable SYSLIB1054
    //Not useful to use LibraryImport when passing a user-defined type.
    //See https://stackoverflow.com/questions/77770231/libraryimport-the-type-is-not-supported-by-source-generated-p-invokes
    [DllImport(DllName, EntryPoint = Photino_ctor, CallingConvention = CallingConvention.Cdecl, SetLastError = true, CharSet = CharSet.Ansi)]
    internal static extern IntPtr Constructor(ref InfiniFrameNativeParameters parameters);
    #pragma warning restore SYSLIB1054

    [LibraryImport(DllName, EntryPoint = Photino_dtor), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void Destructor(IntPtr instance);

    [LibraryImport(DllName, EntryPoint = Photino_AddCustomSchemeName, SetLastError = true, StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void AddCustomSchemeName(IntPtr instance, string scheme);

    [LibraryImport(DllName, EntryPoint = Photino_Close, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void Close(IntPtr instance);
    #endregion

    #region Get
    [LibraryImport(DllName, EntryPoint = Photino_getHwnd_win32, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial IntPtr GetWindowHandlerWin32(IntPtr instance);

    [LibraryImport(DllName, EntryPoint = Photino_GetAllMonitors, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void GetAllMonitors(IntPtr instance, CppGetAllMonitorsDelegate callback);

    [LibraryImport(DllName, EntryPoint = Photino_GetTransparentEnabled, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void GetTransparentEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(DllName, EntryPoint = Photino_GetContextMenuEnabled, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void GetContextMenuEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(DllName, EntryPoint = Photino_GetDevToolsEnabled, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void GetDevToolsEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(DllName, EntryPoint = Photino_GetFullScreen, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void GetFullScreen(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool fullScreen);

    [LibraryImport(DllName, EntryPoint = Photino_GetGrantBrowserPermissions, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void GetGrantBrowserPermissions(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool grant);

    [LibraryImport(DllName, EntryPoint = Photino_GetUserAgent, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial IntPtr GetUserAgent(IntPtr instance);

    [LibraryImport(DllName, EntryPoint = Photino_GetMediaAutoplayEnabled, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void GetMediaAutoplayEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(DllName, EntryPoint = Photino_GetFileSystemAccessEnabled, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void GetFileSystemAccessEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(DllName, EntryPoint = Photino_GetWebSecurityEnabled, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void GetWebSecurityEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(DllName, EntryPoint = Photino_GetJavascriptClipboardAccessEnabled, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void GetJavascriptClipboardAccessEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(DllName, EntryPoint = Photino_GetMediaStreamEnabled, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void GetMediaStreamEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(DllName, EntryPoint = Photino_GetSmoothScrollingEnabled, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void GetSmoothScrollingEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(DllName, EntryPoint = Photino_GetIgnoreCertificateErrorsEnabled, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void GetIgnoreCertificateErrorsEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(DllName, EntryPoint = Photino_GetNotificationsEnabled, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void GetNotificationsEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(DllName, EntryPoint = Photino_GetPosition, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void GetPosition(IntPtr instance, out int x, out int y);

    [DllImport(DllName, EntryPoint = Photino_GetResizable, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
    internal static extern void GetResizable(IntPtr instance, out bool resizable);

    [LibraryImport(DllName, EntryPoint = Photino_GetScreenDpi, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial uint GetScreenDpi(IntPtr instance);

    [LibraryImport(DllName, EntryPoint = Photino_GetSize, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void GetSize(IntPtr instance, out int width, out int height);

    [LibraryImport(DllName, EntryPoint = Photino_GetTitle, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial IntPtr GetTitle(IntPtr instance);

    [LibraryImport(DllName, EntryPoint = Photino_GetTopmost, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void GetTopmost(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool topmost);

    [LibraryImport(DllName, EntryPoint = Photino_GetZoom, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void GetZoom(IntPtr instance, out int zoom);

    [DllImport(DllName, EntryPoint = Photino_GetMaximized, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
    internal static extern void GetMaximized(IntPtr instance, out bool maximized);

    [DllImport(DllName, EntryPoint = Photino_GetMinimized, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
    internal static extern void GetMinimized(IntPtr instance, out bool minimized);

    [DllImport(DllName, EntryPoint = Photino_GetZoomEnabled, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
    internal static extern void GetZoomEnabled(IntPtr instance, out bool zoomEnabled);
    #endregion

    #region MARSHAL CALLS FROM Non-UI Thread to UI Thread
    [LibraryImport(DllName, EntryPoint = Photino_Invoke, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void Invoke(IntPtr instance, InvokeCallback callback);
    #endregion

    #region Navigate
    [LibraryImport(DllName, EntryPoint = Photino_NavigateToString, SetLastError = true, StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void NavigateToString(IntPtr instance, string content);

    [LibraryImport(DllName, EntryPoint = Photino_NavigateToUrl, SetLastError = true, StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void NavigateToUrl(IntPtr instance, string url);
    #endregion

    #region Set
    [LibraryImport(DllName, EntryPoint = Photino_setWebView2RuntimePath_win32, SetLastError = true, StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void SetWebView2RuntimePath_win32(IntPtr instance, string webView2RuntimePath);

    [LibraryImport(DllName, EntryPoint = Photino_SetTransparentEnabled, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void SetTransparentEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool enabled);

    [LibraryImport(DllName, EntryPoint = Photino_SetContextMenuEnabled, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void SetContextMenuEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool enabled);

    [LibraryImport(DllName, EntryPoint = Photino_SetDevToolsEnabled, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void SetDevToolsEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool enabled);

    [LibraryImport(DllName, EntryPoint = Photino_SetFullScreen, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void SetFullScreen(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool fullScreen);

    // ReSharper disable once UnusedMember.Local
    [LibraryImport(DllName, EntryPoint = Photino_SetGrantBrowserPermissions, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void SetGrantBrowserPermissions(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool grant);

    [LibraryImport(DllName, EntryPoint = Photino_SetMaximized, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void SetMaximized(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool maximized);

    [LibraryImport(DllName, EntryPoint = Photino_SetMaxSize, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void SetMaxSize(IntPtr instance, int maxWidth, int maxHeight);

    [LibraryImport(DllName, EntryPoint = Photino_SetMinimized, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void SetMinimized(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool minimized);

    [LibraryImport(DllName, EntryPoint = Photino_SetMinSize, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void SetMinSize(IntPtr instance, int minWidth, int minHeight);

    [LibraryImport(DllName, EntryPoint = Photino_SetResizable, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void SetResizable(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool resizable);

    [LibraryImport(DllName, EntryPoint = Photino_SetPosition, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void SetPosition(IntPtr instance, int x, int y);

    [LibraryImport(DllName, EntryPoint = Photino_SetSize, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void SetSize(IntPtr instance, int width, int height);

    [LibraryImport(DllName, EntryPoint = Photino_SetTitle, SetLastError = true, StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void SetTitle(IntPtr instance, string title);

    [LibraryImport(DllName, EntryPoint = Photino_SetTopmost, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void SetTopmost(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool topmost);

    [LibraryImport(DllName, EntryPoint = Photino_SetIconFile, SetLastError = true, StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void SetIconFile(IntPtr instance, string filename);

    [LibraryImport(DllName, EntryPoint = Photino_SetZoom, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void SetZoom(IntPtr instance, int zoom);

    [DllImport(DllName, EntryPoint = Photino_SetZoomEnabled, CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
    internal static extern void SetZoomEnabled(IntPtr instance, bool zoomEnabled);
    #endregion

    #region Misc
    [LibraryImport(DllName, EntryPoint = Photino_Center, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void Center(IntPtr instance);

    [LibraryImport(DllName, EntryPoint = Photino_ClearBrowserAutoFill, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void ClearBrowserAutoFill(IntPtr instance);

    [LibraryImport(DllName, EntryPoint = Photino_SendWebMessage, SetLastError = true, StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void SendWebMessage(IntPtr instance, string message);

    // ReSharper disable once UnusedMember.Local
    [LibraryImport(DllName, EntryPoint = Photino_ShowMessage, SetLastError = true, StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void ShowMessage(IntPtr instance, string title, string body, uint type);

    [LibraryImport(DllName, EntryPoint = Photino_ShowNotification, SetLastError = true, StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void ShowNotification(IntPtr instance, string title, string body);

    [LibraryImport(DllName, EntryPoint = Photino_WaitForExit, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial void WaitForExit(IntPtr instance);
    #endregion

    #region Dialog
    [LibraryImport(DllName, EntryPoint = Photino_ShowOpenFile, SetLastError = true, StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial IntPtr ShowOpenFile(IntPtr inst, string title, string defaultPath, [MarshalAs(UnmanagedType.I1)] bool multiSelect, string[] filters, int filtersCount, out int resultCount);

    [LibraryImport(DllName, EntryPoint = Photino_ShowOpenFolder, SetLastError = true, StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial IntPtr ShowOpenFolder(IntPtr inst, string title, string defaultPath, [MarshalAs(UnmanagedType.I1)] bool multiSelect, out int resultCount);

    [LibraryImport(DllName, EntryPoint = Photino_ShowSaveFile, SetLastError = true, StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial IntPtr ShowSaveFile(IntPtr inst, string title, string defaultPath, string[] filters, int filtersCount);

    [LibraryImport(DllName, EntryPoint = Photino_ShowMessage, SetLastError = true, StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameDialogResult ShowMessage(IntPtr inst, string title, string text, InfiniFrameDialogButtons buttons, InfiniFrameDialogIcon icon);
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

    internal static void GetTitle(IntPtr instance, out string title) {
        IntPtr ptr = GetTitle(instance);
        title = Marshal.PtrToStringAuto(ptr) ?? string.Empty;// The way on how infiniFrame works internally is that the title is always an empty string when we set it to null on our end.
    }
    #endregion
}
