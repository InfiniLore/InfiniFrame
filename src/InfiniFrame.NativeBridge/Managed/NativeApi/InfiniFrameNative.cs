// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;

namespace InfiniFrame.NativeBridge;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public partial class InfiniFrameNative {
    /// <summary>
    ///     Converts a native pointer to a managed string, using Unicode on Windows and UTF-8 on other platforms.
    /// </summary>
    /// <param name="ptr">The native string pointer.</param>
    /// <returns>The managed string, or <c>null</c> if the pointer is zero.</returns>
    public static string? PtrToNativeString(IntPtr ptr) {
        if (ptr == IntPtr.Zero) return null;

        return OperatingSystem.IsWindows()
            ? Marshal.PtrToStringUni(ptr)
            : Marshal.PtrToStringUTF8(ptr);
    }
}
