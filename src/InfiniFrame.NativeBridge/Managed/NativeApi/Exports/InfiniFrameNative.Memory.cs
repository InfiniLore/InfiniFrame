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
    ///     Frees a native string that was allocated by the native library.
    /// </summary>
    /// <param name="value">The native string pointer to free.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_FreeString", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus FreeString(IntPtr value);

    /// <summary>
    ///     Frees a native string array that was allocated by the native library.
    /// </summary>
    /// <param name="values">The native pointer to the string array.</param>
    /// <param name="count">The number of strings in the array.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_FreeStringArray", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus FreeStringArray(IntPtr values, int count);

    /// <summary>
    ///     Retrieves the last error message from the native library.
    /// </summary>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_GetLastErrorMessage", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial InfiniFrameNativeInteropStatus GetLastErrorMessagePtr(out IntPtr value);

    /// <summary>
    ///     Gets the last error message string from the native library, or <c>null</c> if none is available.
    /// </summary>
    /// <returns>The error message string, or <c>null</c>.</returns>
    internal static string? GetLastErrorMessage() {
        InfiniFrameNativeInteropStatus status = GetLastErrorMessagePtr(out IntPtr ptr);
        if (status != InfiniFrameNativeInteropStatus.Success || ptr == IntPtr.Zero) return null;

        try {
            return MarshalNativeToString(ptr);
        }
        finally {
            FreeString(ptr);
        }
    }
}
