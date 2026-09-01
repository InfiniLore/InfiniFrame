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
    /// <summary>
    ///     Gets the native NSWindow handle for the specified instance (macOS only).
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="value">The NSWindow handle pointer.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [SupportedOSPlatform("macOS")]
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_getNSWindow_mac", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetWindowHandleMac(IntPtr instance, out IntPtr value);
}
