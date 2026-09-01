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
    ///     Registers the application with the macOS process (macOS only).
    ///     This is a legacy method. Use InfiniFrameApplication.Initialize() and ApplicationRegisterMac() instead.
    /// </summary>
    /// <returns>A status code indicating success or failure.</returns>
    [Obsolete("Use InfiniFrameApplication.Initialize() instead.")]
    [SupportedOSPlatform("macOS")]
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_register_mac", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus RegisterMac();

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
