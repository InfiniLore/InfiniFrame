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
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_FreeString", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus FreeString(IntPtr value);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_FreeStringArray", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus FreeStringArray(IntPtr values, int count);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrame_GetLastErrorMessage", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial InfiniFrameNativeInteropStatus GetLastErrorMessagePtr(out IntPtr value);

    internal static string? GetLastErrorMessage() {
        InfiniFrameNativeInteropStatus status = GetLastErrorMessagePtr(out IntPtr ptr);
        if (status != InfiniFrameNativeInteropStatus.Success || ptr == IntPtr.Zero) return null;

        try {
            return PtrToNativeString(ptr);
        }
        finally {
            FreeString(ptr);
        }
    }
}
