// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace InfiniFrame.NativeBridge;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public partial class InfiniFrameNative {
    /// <summary>
    ///     Gets whether the transparent background mode is enabled.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="enabled">Whether transparency is enabled.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_GetTransparentEnabled", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetTransparentEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    /// <summary>
    ///     Gets whether the browser context menu is enabled.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="enabled">Whether the context menu is enabled.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_GetContextMenuEnabled", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetContextMenuEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    /// <summary>
    ///     Gets whether zoom functionality is enabled.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="zoomEnabled">Whether zoom is enabled.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_GetZoomEnabled", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetZoomEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool zoomEnabled);

    /// <summary>
    ///     Gets whether developer tools are enabled.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="enabled">Whether dev tools are enabled.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_GetDevToolsEnabled", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetDevToolsEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    /// <summary>
    ///     Gets whether the window is in full-screen mode.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="fullScreen">Whether the window is full-screen.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_GetFullScreen", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetFullScreen(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool fullScreen);

    /// <summary>
    ///     Gets whether browser permissions (camera, microphone, etc.) are automatically granted.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="grant">Whether permissions are granted automatically.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_GetGrantBrowserPermissions", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetGrantBrowserPermissions(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool grant);

    /// <summary>
    ///     Gets whether media autoplay is enabled.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="enabled">Whether media autoplay is enabled.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_GetMediaAutoplayEnabled", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetMediaAutoplayEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    /// <summary>
    ///     Gets whether file system access is enabled.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="enabled">Whether file system access is enabled.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_GetFileSystemAccessEnabled", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetFileSystemAccessEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    /// <summary>
    ///     Gets whether web security features are enabled.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="enabled">Whether web security is enabled.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_GetWebSecurityEnabled", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetWebSecurityEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    /// <summary>
    ///     Gets whether JavaScript clipboard access is enabled.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="enabled">Whether JavaScript clipboard access is enabled.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_GetJavascriptClipboardAccessEnabled", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetJavascriptClipboardAccessEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    /// <summary>
    ///     Gets whether media streaming is enabled.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="enabled">Whether media streaming is enabled.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_GetMediaStreamEnabled", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetMediaStreamEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    /// <summary>
    ///     Gets whether smooth scrolling is enabled.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="enabled">Whether smooth scrolling is enabled.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_GetSmoothScrollingEnabled", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetSmoothScrollingEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    /// <summary>
    ///     Gets whether the status bar (URL hover indicator) is enabled.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="enabled">Whether the status bar is enabled.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_GetStatusBarEnabled", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetStatusBarEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    /// <summary>
    ///     Gets whether browser keyboard shortcuts are enabled.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="enabled">Whether browser shortcuts are enabled.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_GetBrowserShortcutsEnabled", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetBrowserShortcutsEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    /// <summary>
    ///     Gets whether certificate errors are ignored.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="enabled">Whether certificate errors are ignored.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_GetIgnoreCertificateErrorsEnabled", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetIgnoreCertificateErrorsEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    /// <summary>
    ///     Gets whether the window is maximized.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="maximized">Whether the window is maximized.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_GetMaximized", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetMaximized(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool maximized);

    /// <summary>
    ///     Gets whether the window is minimized.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="minimized">Whether the window is minimized.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_GetMinimized", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetMinimized(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool minimized);

    /// <summary>
    ///     Gets whether the window is resizable by the user.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="resizable">Whether the window is resizable.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_GetResizable", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetResizable(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool resizable);

    /// <summary>
    ///     Gets whether the window is topmost (always on top).
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="topmost">Whether the window is topmost.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_GetTopmost", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetTopmost(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool topmost);

    /// <summary>
    ///     Gets whether the native window currently has focus.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="isFocused">Whether the window is focused.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_GetFocused", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetFocused(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool isFocused);

    /// <summary>
    ///     Gets the window position in screen coordinates.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="x">The x-coordinate of the window.</param>
    /// <param name="y">The y-coordinate of the window.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_GetPosition", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetPosition(IntPtr instance, out int x, out int y);

    /// <summary>
    ///     Gets the window size in pixels.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="width">The window width.</param>
    /// <param name="height">The window height.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_GetSize", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetSize(IntPtr instance, out int width, out int height);

    /// <summary>
    ///     Gets the maximum window size.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="maxWidth">The maximum width.</param>
    /// <param name="maxHeight">The maximum height.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_GetMaxSize", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetMaxSize(IntPtr instance, out int maxWidth, out int maxHeight);

    /// <summary>
    ///     Gets the minimum window size.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="minWidth">The minimum width.</param>
    /// <param name="minHeight">The minimum height.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_GetMinSize", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetMinSize(IntPtr instance, out int minWidth, out int minHeight);

    /// <summary>
    ///     Gets the current window background color as raw RGBA components.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="r">Output: red component (0-255).</param>
    /// <param name="g">Output: green component (0-255).</param>
    /// <param name="b">Output: blue component (0-255).</param>
    /// <param name="a">Output: alpha component (0-255).</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_GetBackgroundColor", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetBackgroundColor(IntPtr instance, out byte r, out byte g, out byte b, out byte a);

    /// <summary>
    ///     Gets the screen DPI value for the window's display.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="value">The DPI value.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_GetScreenDpi", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetScreenDpi(IntPtr instance, out uint value);

    /// <summary>
    ///     Gets the current zoom level of the browser control.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="zoom">The zoom level (e.g. 100 = 100%).</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_GetZoom", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetZoom(IntPtr instance, out int zoom);

    /// <summary>
    ///     Retrieves the user agent string pointer from the native layer.
    /// </summary>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_GetUserAgent", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial InfiniFrameNativeInteropStatus GetUserAgentPtr(IntPtr instance, out IntPtr value);
    /// <summary>
    ///     Gets the current user agent string of the browser control.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="userAgent">The user agent string.</param>
    /// <returns>A status code indicating success or failure.</returns>
    internal static InfiniFrameNativeInteropStatus GetUserAgent(IntPtr instance, out string? userAgent) {
        InfiniFrameNativeInteropStatus status = GetUserAgentPtr(instance, out IntPtr ptr);
        try {
            userAgent = MarshalNativeToString(ptr);
        }
        finally {
            if (ptr != IntPtr.Zero) {
                FreeString(ptr);
            }
        }

        return status;
    }

    /// <summary>
    ///     Retrieves the window title string pointer from the native layer.
    /// </summary>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_GetTitle", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial InfiniFrameNativeInteropStatus GetTitlePtr(IntPtr instance, out IntPtr value);
    /// <summary>
    ///     Gets the current window title.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="title">The window title.</param>
    /// <returns>A status code indicating success or failure.</returns>
    internal static InfiniFrameNativeInteropStatus GetTitle(IntPtr instance, out string? title) {
        InfiniFrameNativeInteropStatus status = GetTitlePtr(instance, out IntPtr ptr);
        try {
            title = MarshalNativeToString(ptr);
        }
        finally {
            if (ptr != IntPtr.Zero) {
                FreeString(ptr);
            }
        }

        return status;
    }

    /// <summary>
    ///     Retrieves the icon file name string pointer from the native layer.
    /// </summary>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_GetIconFileName", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial InfiniFrameNativeInteropStatus GetIconFileNamePtr(IntPtr instance, out IntPtr value);
    /// <summary>
    ///     Gets the icon file name used by the native window.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="iconFileName">The icon file name.</param>
    /// <returns>A status code indicating success or failure.</returns>
    internal static InfiniFrameNativeInteropStatus GetIconFileName(IntPtr instance, out string iconFileName) {
        InfiniFrameNativeInteropStatus status = GetIconFileNamePtr(instance, out IntPtr ptr);
        try {
            iconFileName = MarshalNativeToString(ptr) ?? string.Empty;
        }
        finally {
            if (ptr != IntPtr.Zero) {
                FreeString(ptr);
            }
        }

        return status;
    }
}
