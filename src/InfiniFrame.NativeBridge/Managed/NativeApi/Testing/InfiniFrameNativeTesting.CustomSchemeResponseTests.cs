// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace InfiniFrame.NativeBridge;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static partial class InfiniFrameNativeTesting {

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNativeTests_ParseOrigin", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial InfiniFrameNativeInteropStatus ParseOriginNative(
        IntPtr value,
        out IntPtr scheme,
        out IntPtr host,
        out IntPtr port,
        out int valid
    );

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNativeTests_IsSameOrigin", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial InfiniFrameNativeInteropStatus IsSameOriginNative(
        IntPtr left,
        IntPtr right,
        out int result
    );

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNativeTests_BuildHeaders", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial InfiniFrameNativeInteropStatus BuildHeadersNative(
        IntPtr contentType,
        IntPtr resourceUri,
        IntPtr requestOrigin,
        out IntPtr headers
    );

    internal static InfiniFrameNativeInteropStatus ParseOrigin(
        string value,
        out IntPtr scheme,
        out IntPtr host,
        out IntPtr port,
        out int valid
    ) {
        IntPtr valuePtr = MarshalStringToNative(value);
        try {
            return ParseOriginNative(valuePtr, out scheme, out host, out port, out valid);
        }
        finally {
            FreeNativeString(valuePtr);
        }
    }

    internal static InfiniFrameNativeInteropStatus IsSameOrigin(
        string left,
        string right,
        out int result
    ) {
        IntPtr leftPtr = MarshalStringToNative(left);
        IntPtr rightPtr = MarshalStringToNative(right);
        try {
            return IsSameOriginNative(leftPtr, rightPtr, out result);
        }
        finally {
            FreeNativeString(leftPtr);
            FreeNativeString(rightPtr);
        }
    }

    internal static InfiniFrameNativeInteropStatus BuildHeaders(
        string contentType,
        string resourceUri,
        string requestOrigin,
        out IntPtr headers
    ) {
        IntPtr contentTypePtr = MarshalStringToNative(contentType);
        IntPtr resourceUriPtr = MarshalStringToNative(resourceUri);
        IntPtr requestOriginPtr = MarshalStringToNative(requestOrigin);
        try {
            return BuildHeadersNative(contentTypePtr, resourceUriPtr, requestOriginPtr, out headers);
        }
        finally {
            FreeNativeString(contentTypePtr);
            FreeNativeString(resourceUriPtr);
            FreeNativeString(requestOriginPtr);
        }
    }

    internal static string? MarshalNativeToString(IntPtr ptr) {
        if (ptr == IntPtr.Zero) return null;
#if WINDOWS
        return Marshal.PtrToStringUni(ptr);
#else
        return Marshal.PtrToStringUTF8(ptr);
#endif
    }

    internal static InfiniFrameNativeInteropStatus FreeTestString(IntPtr value)
        => InfiniFrameNative.FreeString(value);

    private static IntPtr MarshalStringToNative(string? value) {
        if (value == null) return IntPtr.Zero;
#if WINDOWS
        return Marshal.StringToHGlobalUni(value);
#else
        return Marshal.StringToHGlobalAnsi(value);
#endif
    }

    private static void FreeNativeString(IntPtr ptr) {
        if (ptr != IntPtr.Zero) Marshal.FreeHGlobal(ptr);
    }
}
