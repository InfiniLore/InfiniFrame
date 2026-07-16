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
    ///     Gets the native GTK window handle for the specified instance (Linux only).
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="value">The GTK window handle pointer.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [SupportedOSPlatform("linux")]
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_getGtkWindow_linux", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetWindowHandleLinux(IntPtr instance, out IntPtr value);
}
