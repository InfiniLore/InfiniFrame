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
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_Center", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus Center(IntPtr instance);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_Restore", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus Restore(IntPtr instance);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_ClearBrowserAutoFill", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus ClearBrowserAutoFill(IntPtr instance);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_SetFocused", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetFocused(IntPtr instance);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_ShowNotification", SetLastError = true, StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus ShowNotification(IntPtr instance, string title, string body);
}
