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
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_SetTransparentEnabled", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetTransparentEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool enabled);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_SetContextMenuEnabled", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetContextMenuEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool enabled);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_SetZoomEnabled", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetZoomEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool zoomEnabled);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_SetDevToolsEnabled", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetDevToolsEnabled(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool enabled);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_SetFullScreen", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetFullScreen(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool fullScreen);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_SetMaximized", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetMaximized(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool maximized);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_SetMinimized", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetMinimized(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool minimized);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_SetResizable", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetResizable(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool resizable);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_SetTopmost", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetTopmost(IntPtr instance, [MarshalAs(UnmanagedType.I1)] bool topmost);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_SetIconFile", SetLastError = true, StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetIconFile(IntPtr instance, string filename);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_SetTitle", SetLastError = true, StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetTitle(IntPtr instance, string? title);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_SetZoom", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetZoom(IntPtr instance, int zoom);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_SetPosition", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetPosition(IntPtr instance, int x, int y);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_SetSize", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetSize(IntPtr instance, int width, int height);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_SetMaxSize", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetMaxSize(IntPtr instance, int maxWidth, int maxHeight);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_SetMinSize", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetMinSize(IntPtr instance, int minWidth, int minHeight);
}
