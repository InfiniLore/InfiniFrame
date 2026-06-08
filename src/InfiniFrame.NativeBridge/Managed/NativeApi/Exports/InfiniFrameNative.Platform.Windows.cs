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
    [SupportedOSPlatform("windows")]
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_register_win32", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus RegisterWin32(IntPtr hInstance);

    [SupportedOSPlatform("windows")]
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_getHwnd_win32", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetWindowHandleWin32(IntPtr instance, out IntPtr value);

    [SupportedOSPlatform("windows")]
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_setWebView2RuntimePath_win32", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetWebView2RuntimePath_win32(IntPtr instance, string webView2RuntimePath);

    [SupportedOSPlatform("windows")]
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_GetNotificationsEnabled", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetNotificationsEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [SupportedOSPlatform("windows")]
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_getWebView2RuntimeVersion_win32", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial InfiniFrameNativeInteropStatus GetWebView2RuntimeVersionPtr(out IntPtr value);

    [SupportedOSPlatform("windows")]
    internal static string? GetWebView2RuntimeVersion() {
        if (!OperatingSystem.IsWindows()) return null;

        InfiniFrameNativeInteropStatus status = GetWebView2RuntimeVersionPtr(out IntPtr ptr);
        if (status != InfiniFrameNativeInteropStatus.Success || ptr == IntPtr.Zero)
            return null;

        try {
            return PtrToNativeString(ptr);
        }
        finally {
            FreeString(ptr);
        }
    }

}
