// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Delegates;
using InfiniFrame.NativeBridge.Dialogs;
using InfiniFrame.NativeBridge.Parameters;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using static InfiniFrame.NativeBridge.ArtifactManifest;

namespace InfiniFrame.NativeBridge;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static partial class InfiniFrameNative {

    #region MARSHAL CALLS FROM Non-UI Thread to UI Thread
    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_Invoke", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus Invoke(IntPtr instance, Action callback);
    #endregion
    #region Register
    // ReSharper disable once UnusedMethodReturnValue.Local
    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_register_win32", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus RegisterWin32(IntPtr hInstance);

    // ReSharper disable once UnusedMethodReturnValue.Local
    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_register_mac", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus RegisterMac();
    #endregion

    #region CTOR-DTOR
    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_ctor", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus Constructor([MarshalUsing(typeof(InfiniFrameNativeParametersMarshaller))] in InfiniFrameNativeParameters parameters, out IntPtr value);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_dtor"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus Destructor(IntPtr instance);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_AddCustomSchemeName", SetLastError = true, StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus AddCustomSchemeName(IntPtr instance, string scheme);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_Close", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus Close(IntPtr instance);
    #endregion

    #region Get
    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_getHwnd_win32", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus GetWindowHandlerWin32(IntPtr instance, out IntPtr value);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_GetAllMonitors", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus GetAllMonitors(IntPtr instance, CppGetAllMonitorsDelegate callback);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_GetTransparentEnabled", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus GetTransparentEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_GetContextMenuEnabled", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus GetContextMenuEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_GetDevToolsEnabled", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus GetDevToolsEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_GetFullScreen", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus GetFullScreen(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool fullScreen);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_GetGrantBrowserPermissions", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus GetGrantBrowserPermissions(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool grant);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_GetUserAgent", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus GetUserAgent(IntPtr instance, out IntPtr value);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_GetMediaAutoplayEnabled", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus GetMediaAutoplayEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_GetFileSystemAccessEnabled", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus GetFileSystemAccessEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_GetWebSecurityEnabled", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus GetWebSecurityEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_GetJavascriptClipboardAccessEnabled", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus GetJavascriptClipboardAccessEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_GetMediaStreamEnabled", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus GetMediaStreamEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_GetSmoothScrollingEnabled", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus GetSmoothScrollingEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_GetIgnoreCertificateErrorsEnabled", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus GetIgnoreCertificateErrorsEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_GetNotificationsEnabled", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus GetNotificationsEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_GetPosition", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus GetPosition(IntPtr instance, out int x, out int y);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_GetResizable", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus GetResizable(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool resizable);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_GetScreenDpi", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus GetScreenDpi(IntPtr instance, out uint value);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_GetSize", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus GetSize(IntPtr instance, out int width, out int height);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_GetMaxSize", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus GetMaxSize(IntPtr instance, out int maxWidth, out int maxHeight);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_GetMinSize", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus GetMinSize(IntPtr instance, out int minWidth, out int minHeight);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_GetTitle", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus GetTitle(IntPtr instance, out IntPtr value);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_GetTopmost", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus GetTopmost(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool topmost);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_GetZoom", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus GetZoom(IntPtr instance, out int zoom);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_GetMaximized", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus GetMaximized(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool maximized);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_GetMinimized", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus GetMinimized(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool minimized);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_GetZoomEnabled", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus GetZoomEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool zoomEnabled);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_GetIconFileName", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus GetIconFileName(IntPtr instance, out IntPtr value);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_GetFocused", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus GetFocused(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool isFocused);
    #endregion

    #region Navigate
    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_NavigateToString", SetLastError = true, StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus NavigateToString(IntPtr instance, string content);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_NavigateToUrl", SetLastError = true, StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus NavigateToUrl(IntPtr instance, string url);
    #endregion

    #region Set
    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_setWebView2RuntimePath_win32", SetLastError = true, StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus SetWebView2RuntimePath_win32(IntPtr instance, string webView2RuntimePath);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_SetTransparentEnabled", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus SetTransparentEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool enabled);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_SetContextMenuEnabled", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus SetContextMenuEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool enabled);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_SetDevToolsEnabled", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus SetDevToolsEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool enabled);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_SetFullScreen", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus SetFullScreen(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool fullScreen);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_SetMaximized", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus SetMaximized(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool maximized);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_SetMaxSize", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus SetMaxSize(IntPtr instance, int maxWidth, int maxHeight);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_SetMinimized", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus SetMinimized(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool minimized);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_SetMinSize", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus SetMinSize(IntPtr instance, int minWidth, int minHeight);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_SetResizable", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus SetResizable(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool resizable);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_SetPosition", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus SetPosition(IntPtr instance, int x, int y);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_SetSize", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus SetSize(IntPtr instance, int width, int height);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_SetTitle", SetLastError = true, StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus SetTitle(IntPtr instance, string? title);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_SetTopmost", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus SetTopmost(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool topmost);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_SetIconFile", SetLastError = true, StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus SetIconFile(IntPtr instance, string filename);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_SetZoom", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus SetZoom(IntPtr instance, int zoom);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_SetZoomEnabled", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus SetZoomEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool zoomEnabled);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_SetFocused", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus SetFocused(IntPtr instance);
    #endregion

    #region Misc
    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_Center", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus Center(IntPtr instance);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_Restore", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus Restore(IntPtr instance);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_ClearBrowserAutoFill", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus ClearBrowserAutoFill(IntPtr instance);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_SendWebMessage", SetLastError = true, StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus SendWebMessage(IntPtr instance, string message);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_ShowNotification", SetLastError = true, StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus ShowNotification(IntPtr instance, string title, string body);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_WaitForExit", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus WaitForExit(IntPtr instance);
    #endregion

    #region Dialog
    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_ShowOpenFile", SetLastError = true, StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus ShowOpenFile(IntPtr inst, string title, string defaultPath, [MarshalAs(UnmanagedType.I1)] bool multiSelect, string[] filters, int filtersCount, out int resultCount, out IntPtr values);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_ShowOpenFolder", SetLastError = true, StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus ShowOpenFolder(IntPtr inst, string title, string defaultPath, [MarshalAs(UnmanagedType.I1)] bool multiSelect, out int resultCount, out IntPtr values);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_ShowSaveFile", SetLastError = true, StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus ShowSaveFile(IntPtr inst, string title, string defaultPath, string[] filters, int filtersCount, string? defaultFileName, out IntPtr value);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_ShowMessage", SetLastError = true, StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus ShowMessage(IntPtr inst, string title, string text, InfiniFrameDialogButtons buttons, InfiniFrameDialogIcon icon, out InfiniFrameDialogResult value);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_FreeString", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus FreeString(IntPtr value);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_FreeStringArray", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatus FreeStringArray(IntPtr values, int count);

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniFrame_GetLastErrorMessage", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial InfiniFrameNativeStatus GetLastErrorMessagePtr(out IntPtr value);
    #endregion

    #region Overloads
    internal static string? PtrToNativeString(IntPtr ptr) {
        if (ptr == IntPtr.Zero) return null;

        return OperatingSystem.IsWindows()
            ? Marshal.PtrToStringUni(ptr)
            : Marshal.PtrToStringUTF8(ptr);
    }

    internal static InfiniFrameNativeStatus GetHeight(IntPtr instance, out int height)
        => GetSize(instance, out _, out height);

    internal static InfiniFrameNativeStatus GetWidth(IntPtr instance, out int width)
        => GetSize(instance, out width, out _);

    internal static InfiniFrameNativeStatus GetMaxHeight(IntPtr instance, out int maxHeight)
        => GetMaxSize(instance, out _, out maxHeight);

    internal static InfiniFrameNativeStatus GetMaxWidth(IntPtr instance, out int maxWidth)
        => GetMaxSize(instance, out maxWidth, out _);

    internal static InfiniFrameNativeStatus GetMinHeight(IntPtr instance, out int minHeight)
        => GetMinSize(instance, out _, out minHeight);

    internal static InfiniFrameNativeStatus GetMinWidth(IntPtr instance, out int minWidth)
        => GetMinSize(instance, out minWidth, out _);

    internal static InfiniFrameNativeStatus GetLeft(IntPtr instance, out int left)
        => GetPosition(instance, out left, out _);

    internal static InfiniFrameNativeStatus GetTop(IntPtr instance, out int top)
        => GetPosition(instance, out _, out top);

    internal static InfiniFrameNativeStatus GetSize(IntPtr instance, out Size size) {
        InfiniFrameNativeStatus status = GetSize(instance, out int width, out int height);
        size = new Size(width, height);
        return status;
    }

    internal static InfiniFrameNativeStatus GetMaxSize(IntPtr instance, out Size size) {
        InfiniFrameNativeStatus status = GetMaxSize(instance, out int width, out int height);
        size = new Size(width, height);
        return status;
    }

    internal static InfiniFrameNativeStatus GetMinSize(IntPtr instance, out Size size) {
        InfiniFrameNativeStatus status = GetMinSize(instance, out int width, out int height);
        size = new Size(width, height);
        return status;
    }

    internal static InfiniFrameNativeStatus GetPosition(IntPtr instance, out Point position) {
        InfiniFrameNativeStatus status = GetPosition(instance, out int left, out int top);
        position = new Point(left, top);
        return status;
    }

    internal static InfiniFrameNativeStatus GetWindowRectangle(IntPtr instance, out int x, out int y, out int width, out int height) {
        InfiniFrameNativeStatus sizeStatus = GetSize(instance, out width, out height);
        if (sizeStatus != InfiniFrameNativeStatus.Success) {
            x = 0;
            y = 0;
            return sizeStatus;
        }

        return GetPosition(instance, out x, out y);
    }

    internal static InfiniFrameNativeStatus GetWindowRectangle(IntPtr instance, out Rectangle rectangle) {
        InfiniFrameNativeStatus status = GetWindowRectangle(instance, out int x, out int y, out int width, out int height);
        rectangle = new Rectangle(x, y, width, height);
        return status;
    }

    internal static InfiniFrameNativeStatus GetUserAgent(IntPtr instance, out string? userAgent) {
        InfiniFrameNativeStatus status = GetUserAgent(instance, out IntPtr ptr);
        try {
            userAgent = PtrToNativeString(ptr);
        }
        finally {
            if (ptr != IntPtr.Zero) {
                FreeString(ptr);
            }
        }

        return status;
    }

    internal static InfiniFrameNativeStatus GetTitle(IntPtr instance, out string? title) {
        InfiniFrameNativeStatus status = GetTitle(instance, out IntPtr ptr);
        try {
            title = PtrToNativeString(ptr);
        }
        finally {
            if (ptr != IntPtr.Zero) {
                FreeString(ptr);
            }
        }

        return status;
    }

    internal static InfiniFrameNativeStatus GetIconFileName(IntPtr instance, out string iconFileName) {
        InfiniFrameNativeStatus status = GetIconFileName(instance, out IntPtr ptr);
        try {
            iconFileName = PtrToNativeString(ptr) ?? string.Empty;
        }
        finally {
            if (ptr != IntPtr.Zero) {
                FreeString(ptr);
            }
        }

        return status;
    }

    internal static string? GetLastErrorMessage() {
        InfiniFrameNativeStatus status = GetLastErrorMessagePtr(out IntPtr ptr);
        if (status != InfiniFrameNativeStatus.Success || ptr == IntPtr.Zero) return null;

        try {
            return PtrToNativeString(ptr);
        }
        finally {
            FreeString(ptr);
        }
    }

    internal static InfiniFrameNativeStatus EnsureSucceeded(InfiniFrameNativeStatus status, string operationName) {
        
        int fallbackLastError = Marshal.GetLastPInvokeError();

        if (status is InfiniFrameNativeStatus.Success && fallbackLastError is 0) return status;
        
        InfiniFrameNativeStatus fallbackStatus = GetLastErrorMessagePtr(out IntPtr ptr);

        string? fallbackMessage;
        if (fallbackStatus != InfiniFrameNativeStatus.Success || ptr == IntPtr.Zero) {
            fallbackMessage = "No native error message provided.";
        }
        else {
            try {
                fallbackMessage = PtrToNativeString(ptr);
            }
            finally {
                FreeString(ptr);
            }
        }
        
        throw new ApplicationException($"Native interop call '{operationName}' failed with unknown status state. Fallback last error {fallbackLastError}. {fallbackMessage} {fallbackStatus}");
    }
    #endregion
}
