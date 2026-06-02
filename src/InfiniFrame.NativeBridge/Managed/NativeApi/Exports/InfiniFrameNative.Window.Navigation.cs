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
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_NavigateToString", SetLastError = true, StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus NavigateToString(IntPtr instance, string content);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_NavigateToUrl", SetLastError = true, StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus NavigateToUrl(IntPtr instance, string url);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_SendWebMessage", SetLastError = true, StringMarshalling = StringMarshalling.Utf8), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SendWebMessage(IntPtr instance, string message);
}
