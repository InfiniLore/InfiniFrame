// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace InfiniFrame.NativeBridge;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public partial class InfiniFrameNative {
    /// <summary>
    ///     Registers the Win32 window class (Windows only).
    /// </summary>
    /// <param name="hInstance">The HINSTANCE for the application.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [SupportedOSPlatform("windows")]
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_register_win32", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus RegisterWin32(IntPtr hInstance);

    /// <summary>
    ///     Gets the native HWND handle for the specified instance (Windows only).
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="value">The HWND handle.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [SupportedOSPlatform("windows")]
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_getHwnd_win32", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetWindowHandleWin32(IntPtr instance, out IntPtr value);

    /// <summary>
    ///     Sets a custom WebView2 runtime path for the native instance (Windows only).
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="webView2RuntimePath">The path to the WebView2 runtime.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [SupportedOSPlatform("windows")]
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_setWebView2RuntimePath_win32", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetWebView2RuntimePath_win32(IntPtr instance, string webView2RuntimePath);

    /// <summary>
    ///     Gets whether notifications are enabled for the native instance (Windows only).
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="enabled">Whether notifications are enabled.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [SupportedOSPlatform("windows")]
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_GetNotificationsEnabled", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetNotificationsEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    /// <summary>
    ///     Retrieves the WebView2 runtime version string pointer (Windows only).
    /// </summary>
    [SupportedOSPlatform("windows")]
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_getWebView2RuntimeVersion_win32", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial InfiniFrameNativeInteropStatus GetWebView2RuntimeVersionPtr(out IntPtr value);

    /// <summary>
    ///     Gets the installed WebView2 runtime version string (Windows only).
    /// </summary>
    /// <returns>The version string, or <c>null</c> if not available.</returns>
    [SupportedOSPlatform("windows")]
    internal static string? GetWebView2RuntimeVersion() {
        if (!OperatingSystem.IsWindows()) return null;

        InfiniFrameNativeInteropStatus status = GetWebView2RuntimeVersionPtr(out IntPtr ptr);
        if (status != InfiniFrameNativeInteropStatus.Success || ptr == IntPtr.Zero)
            return null;

        try {
            return MarshalNativeToString(ptr);
        }
        finally {
            FreeString(ptr);
        }
    }
}
