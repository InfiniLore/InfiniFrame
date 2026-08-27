// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;

namespace InfiniFrame.NativeBridge;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Provides P/Invoke declarations and marshalling helpers for the native InfiniFrame library.
/// </summary>
public partial class InfiniFrameNative {
    /// <summary>
    ///     Converts a native UTF-8 pointer to a managed string.
    /// </summary>
    /// <param name="ptr">The native UTF-8 string pointer.</param>
    /// <returns>The managed string, or <c>null</c> if the pointer is zero.</returns>
    public static string? MarshalNativeToString(IntPtr ptr) {
        if (ptr == IntPtr.Zero) return null;

        return Marshal.PtrToStringUTF8(ptr);
    }
}
