// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using static InfiniFrame.Native.NativeDll;

namespace InfiniFrame.Native;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static partial class InfiniFrameNative {
    internal static void EnsureSucceeded(InfiniFrameNativeStatusCode status) {
        if (status == InfiniFrameNativeStatusCode.Success) return;

        string? nativeError = GetLastErrorMessageAndFree();
        string message = string.IsNullOrWhiteSpace(nativeError)
            ? $"Native call failed with status {(int)status} ({status})."
            : $"Native call failed with status {(int)status} ({status}). {nativeError}";
        throw new InvalidOperationException(message);
    }

    #region MARSHAL CALLS FROM Non-UI Thread to UI Thread
    [LibraryImport(DllName, EntryPoint = InfiniFrame_Invoke, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode Invoke(IntPtr instance, InvokeCallback callback);
    #endregion
    #region Register
    // ReSharper disable once UnusedMethodReturnValue.Local
    [LibraryImport(DllName, EntryPoint = InfiniFrame_register_win32, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode RegisterWin32(IntPtr hInstance);

    // ReSharper disable once UnusedMethodReturnValue.Local
    [LibraryImport(DllName, EntryPoint = InfiniFrame_register_mac, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode RegisterMac();
    #endregion

    #region CTOR-DTOR
    [LibraryImport(DllName, EntryPoint = InfiniFrame_ctor, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial IntPtr Constructor([MarshalUsing(typeof(InfiniFrameNativeParametersMarshaller))] in InfiniFrameNativeParameters parameters);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_dtor), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode Destructor(IntPtr instance);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_GetLastErrorMessage, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial IntPtr GetLastErrorMessage();

    [LibraryImport(DllName, EntryPoint = InfiniFrame_AddCustomSchemeName, SetLastError = true, StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode AddCustomSchemeName(IntPtr instance, string scheme);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_Close, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode Close(IntPtr instance);
    #endregion

    #region Get
    [LibraryImport(DllName, EntryPoint = InfiniFrame_getHwnd_win32, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial IntPtr GetWindowHandlerWin32(IntPtr instance);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_GetAllMonitors, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode GetAllMonitors(IntPtr instance, CppGetAllMonitorsDelegate callback);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_GetTransparentEnabled, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode GetTransparentEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_GetContextMenuEnabled, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode GetContextMenuEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_GetDevToolsEnabled, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode GetDevToolsEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_GetFullScreen, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode GetFullScreen(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool fullScreen);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_GetGrantBrowserPermissions, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode GetGrantBrowserPermissions(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool grant);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_GetUserAgent, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial IntPtr GetUserAgent(IntPtr instance);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_GetMediaAutoplayEnabled, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode GetMediaAutoplayEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_GetFileSystemAccessEnabled, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode GetFileSystemAccessEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_GetWebSecurityEnabled, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode GetWebSecurityEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_GetJavascriptClipboardAccessEnabled, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode GetJavascriptClipboardAccessEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_GetMediaStreamEnabled, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode GetMediaStreamEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_GetSmoothScrollingEnabled, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode GetSmoothScrollingEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_GetIgnoreCertificateErrorsEnabled, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode GetIgnoreCertificateErrorsEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_GetNotificationsEnabled, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode GetNotificationsEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_GetPosition, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode GetPosition(IntPtr instance, out int x, out int y);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_GetResizable, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode GetResizable(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool resizable);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_GetScreenDpi, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial uint GetScreenDpi(IntPtr instance);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_GetSize, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode GetSize(IntPtr instance, out int width, out int height);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_GetMaxSize, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode GetMaxSize(IntPtr instance, out int maxWidth, out int maxHeight);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_GetMinSize, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode GetMinSize(IntPtr instance, out int minWidth, out int minHeight);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_GetTitle, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial IntPtr GetTitle(IntPtr instance);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_GetTopmost, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode GetTopmost(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool topmost);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_GetZoom, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode GetZoom(IntPtr instance, out int zoom);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_GetMaximized, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode GetMaximized(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool maximized);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_GetMinimized, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode GetMinimized(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool minimized);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_GetZoomEnabled, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode GetZoomEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool zoomEnabled);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_GetIconFileName, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial IntPtr GetIconFileName(IntPtr instance);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_GetFocused, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode GetFocused(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool isFocused);
    #endregion

    #region Navigate
    [LibraryImport(DllName, EntryPoint = InfiniFrame_NavigateToString, SetLastError = true, StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode NavigateToString(IntPtr instance, string content);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_NavigateToUrl, SetLastError = true, StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode NavigateToUrl(IntPtr instance, string url);
    #endregion

    #region Set
    [LibraryImport(DllName, EntryPoint = InfiniFrame_setWebView2RuntimePath_win32, SetLastError = true, StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode SetWebView2RuntimePath_win32(IntPtr instance, string webView2RuntimePath);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_SetTransparentEnabled, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode SetTransparentEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool enabled);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_SetContextMenuEnabled, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode SetContextMenuEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool enabled);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_SetDevToolsEnabled, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode SetDevToolsEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool enabled);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_SetFullScreen, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode SetFullScreen(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool fullScreen);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_SetMaximized, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode SetMaximized(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool maximized);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_SetMaxSize, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode SetMaxSize(IntPtr instance, int maxWidth, int maxHeight);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_SetMinimized, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode SetMinimized(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool minimized);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_SetMinSize, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode SetMinSize(IntPtr instance, int minWidth, int minHeight);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_SetResizable, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode SetResizable(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool resizable);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_SetPosition, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode SetPosition(IntPtr instance, int x, int y);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_SetSize, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode SetSize(IntPtr instance, int width, int height);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_SetTitle, SetLastError = true, StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode SetTitle(IntPtr instance, string title);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_SetTopmost, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode SetTopmost(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool topmost);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_SetIconFile, SetLastError = true, StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode SetIconFile(IntPtr instance, string filename);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_SetZoom, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode SetZoom(IntPtr instance, int zoom);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_SetZoomEnabled, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode SetZoomEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool zoomEnabled);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_SetFocused, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode SetFocused(IntPtr instance);
    #endregion

    #region Misc
    [LibraryImport(DllName, EntryPoint = InfiniFrame_Center, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode Center(IntPtr instance);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_Restore, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode Restore(IntPtr instance);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_ClearBrowserAutoFill, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode ClearBrowserAutoFill(IntPtr instance);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_SendWebMessage, SetLastError = true, StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode SendWebMessage(IntPtr instance, string message);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_ShowNotification, SetLastError = true, StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode ShowNotification(IntPtr instance, string title, string body);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_WaitForExit, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode WaitForExit(IntPtr instance);
    #endregion

    #region Dialog
    [LibraryImport(DllName, EntryPoint = InfiniFrame_ShowOpenFile, SetLastError = true, StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial IntPtr ShowOpenFile(IntPtr inst, string title, string defaultPath, [MarshalAs(UnmanagedType.I1)] bool multiSelect, string[] filters, int filtersCount, out int resultCount);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_ShowOpenFolder, SetLastError = true, StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial IntPtr ShowOpenFolder(IntPtr inst, string title, string defaultPath, [MarshalAs(UnmanagedType.I1)] bool multiSelect, out int resultCount);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_ShowSaveFile, SetLastError = true, StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial IntPtr ShowSaveFile(IntPtr inst, string title, string defaultPath, string[] filters, int filtersCount, string? defaultFileName);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_ShowMessage, SetLastError = true, StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameDialogResult ShowMessage(IntPtr inst, string title, string text, InfiniFrameDialogButtons buttons, InfiniFrameDialogIcon icon);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_FreeString, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode FreeString(IntPtr value);

    [LibraryImport(DllName, EntryPoint = InfiniFrame_FreeStringArray, SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeStatusCode FreeStringArray(IntPtr values, int count);
    #endregion

    #region Overloads
    internal static string? PtrToNativeString(IntPtr ptr) {
        if (ptr == IntPtr.Zero) return null;

        return OperatingSystem.IsWindows()
            ? Marshal.PtrToStringUni(ptr)
            : Marshal.PtrToStringUTF8(ptr);
    }

    internal static string? PtrToNativeStringAndFree(IntPtr ptr) {
        if (ptr == IntPtr.Zero) return null;

        try {
            return PtrToNativeString(ptr);
        }
        finally {
            FreeString(ptr);
        }
    }

    internal static string? GetLastErrorMessageAndFree() {
        IntPtr ptr = GetLastErrorMessage();
        return PtrToNativeStringAndFree(ptr);
    }

    internal static InfiniFrameNativeStatusCode GetHeight(IntPtr instance, out int height) => GetSize(instance, out _, out height);
    internal static InfiniFrameNativeStatusCode GetWidth(IntPtr instance, out int width) => GetSize(instance, out width, out _);
    internal static InfiniFrameNativeStatusCode GetMaxHeight(IntPtr instance, out int maxHeight) => GetMaxSize(instance, out _, out maxHeight);
    internal static InfiniFrameNativeStatusCode GetMaxWidth(IntPtr instance, out int maxWidth) => GetMaxSize(instance, out maxWidth, out _);
    internal static InfiniFrameNativeStatusCode GetMinHeight(IntPtr instance, out int minHeight) => GetMinSize(instance, out _, out minHeight);
    internal static InfiniFrameNativeStatusCode GetMinWidth(IntPtr instance, out int minWidth) => GetMinSize(instance, out minWidth, out _);

    internal static InfiniFrameNativeStatusCode GetLeft(IntPtr instance, out int left) => GetPosition(instance, out left, out _);
    internal static InfiniFrameNativeStatusCode GetTop(IntPtr instance, out int top) => GetPosition(instance, out _, out top);

    internal static InfiniFrameNativeStatusCode GetSize(IntPtr instance, out Size size) {
        InfiniFrameNativeStatusCode status = GetSize(instance, out int width, out int height);
        size = new Size(width, height);
        return status;
    }

    internal static InfiniFrameNativeStatusCode GetMaxSize(IntPtr instance, out Size size) {
        InfiniFrameNativeStatusCode status = GetMaxSize(instance, out int width, out int height);
        size = new Size(width, height);
        return status;
    }

    internal static InfiniFrameNativeStatusCode GetMinSize(IntPtr instance, out Size size) {
        InfiniFrameNativeStatusCode status = GetMinSize(instance, out int width, out int height);
        size = new Size(width, height);
        return status;
    }

    internal static InfiniFrameNativeStatusCode GetPosition(IntPtr instance, out Point position) {
        InfiniFrameNativeStatusCode status = GetPosition(instance, out int left, out int top);
        position = new Point(left, top);
        return status;
    }

    internal static InfiniFrameNativeStatusCode GetWindowRectangle(IntPtr instance, out int x, out int y, out int width, out int height) {
        InfiniFrameNativeStatusCode sizeStatus = GetSize(instance, out width, out height);
        if (sizeStatus != InfiniFrameNativeStatusCode.Success) {
            x = 0;
            y = 0;
            return sizeStatus;
        }

        return GetPosition(instance, out x, out y);
    }

    internal static InfiniFrameNativeStatusCode GetWindowRectangle(IntPtr instance, out Rectangle rectangle) {
        InfiniFrameNativeStatusCode status = GetWindowRectangle(instance, out int x, out int y, out int width, out int height);
        rectangle = new Rectangle(x, y, width, height);
        return status;
    }

    internal static InfiniFrameNativeStatusCode GetUserAgent(IntPtr instance, out string? userAgent) {
        IntPtr ptr = GetUserAgent(instance);
        InfiniFrameNativeStatusCode status = (InfiniFrameNativeStatusCode)Marshal.GetLastPInvokeError();
        if (status != InfiniFrameNativeStatusCode.Success) {
            userAgent = null;
            return status;
        }

        userAgent = PtrToNativeStringAndFree(ptr);
        return status;
    }

    internal static InfiniFrameNativeStatusCode GetTitle(IntPtr instance, out string title) {
        IntPtr ptr = GetTitle(instance);
        InfiniFrameNativeStatusCode status = (InfiniFrameNativeStatusCode)Marshal.GetLastPInvokeError();
        if (status != InfiniFrameNativeStatusCode.Success) {
            title = string.Empty;
            return status;
        }

        title = PtrToNativeStringAndFree(ptr) ?? string.Empty;// The way on how infiniFrame works internally is that the title is always an empty string when we set it to null on our end.
        return status;
    }

    internal static InfiniFrameNativeStatusCode GetIconFileName(IntPtr instance, out string iconFileName) {
        IntPtr ptr = GetIconFileName(instance);
        InfiniFrameNativeStatusCode status = (InfiniFrameNativeStatusCode)Marshal.GetLastPInvokeError();
        if (status != InfiniFrameNativeStatusCode.Success) {
            iconFileName = string.Empty;
            return status;
        }

        iconFileName = PtrToNativeStringAndFree(ptr) ?? string.Empty;
        return status;
    }
    #endregion
}
