// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace InfiniFrame.NativeBridge;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public partial class InfiniFrameNative {
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_GetTransparentEnabled", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetTransparentEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_GetContextMenuEnabled", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetContextMenuEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_GetZoomEnabled", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetZoomEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool zoomEnabled);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_GetDevToolsEnabled", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetDevToolsEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_GetFullScreen", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetFullScreen(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool fullScreen);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_GetGrantBrowserPermissions", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetGrantBrowserPermissions(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool grant);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_GetMediaAutoplayEnabled", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetMediaAutoplayEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_GetFileSystemAccessEnabled", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetFileSystemAccessEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_GetWebSecurityEnabled", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetWebSecurityEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_GetJavascriptClipboardAccessEnabled", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetJavascriptClipboardAccessEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_GetMediaStreamEnabled", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetMediaStreamEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_GetSmoothScrollingEnabled", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetSmoothScrollingEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_GetIgnoreCertificateErrorsEnabled", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetIgnoreCertificateErrorsEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool enabled);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_GetMaximized", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetMaximized(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool maximized);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_GetMinimized", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetMinimized(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool minimized);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_GetResizable", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetResizable(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool resizable);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_GetTopmost", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetTopmost(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool topmost);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_GetFocused", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetFocused(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool isFocused);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_GetPosition", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetPosition(IntPtr instance, out int x, out int y);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_GetSize", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetSize(IntPtr instance, out int width, out int height);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_GetMaxSize", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetMaxSize(IntPtr instance, out int maxWidth, out int maxHeight);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_GetMinSize", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetMinSize(IntPtr instance, out int minWidth, out int minHeight);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_GetScreenDpi", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetScreenDpi(IntPtr instance, out uint value);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_GetZoom", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetZoom(IntPtr instance, out int zoom);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_GetUserAgent", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial InfiniFrameNativeInteropStatus GetUserAgentPtr(IntPtr instance, out IntPtr value);
    internal static InfiniFrameNativeInteropStatus GetUserAgent(IntPtr instance, out string? userAgent) {
        InfiniFrameNativeInteropStatus status = GetUserAgentPtr(instance, out IntPtr ptr);
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

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_GetTitle", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial InfiniFrameNativeInteropStatus GetTitlePtr(IntPtr instance, out IntPtr value);
    internal static InfiniFrameNativeInteropStatus GetTitle(IntPtr instance, out string? title) {
        InfiniFrameNativeInteropStatus status = GetTitlePtr(instance, out IntPtr ptr);
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

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_GetIconFileName", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial InfiniFrameNativeInteropStatus GetIconFileNamePtr(IntPtr instance, out IntPtr value);
    internal static InfiniFrameNativeInteropStatus GetIconFileName(IntPtr instance, out string iconFileName) {
        InfiniFrameNativeInteropStatus status = GetIconFileNamePtr(instance, out IntPtr ptr);
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
}
