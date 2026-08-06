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
    ///     Sets the taskbar progress indicator with the specified state, current value, and total value.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="state">The taskbar progress state.</param>
    /// <param name="current">The current progress value.</param>
    /// <param name="total">The total progress value.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [SupportedOSPlatform("windows")]
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_SetTaskbarProgress", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetTaskbarProgress(IntPtr instance, int state, ulong current, ulong total);

    /// <summary>
    ///     Clears the taskbar progress indicator.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [SupportedOSPlatform("windows")]
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_ClearTaskbarProgress", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus ClearTaskbarProgress(IntPtr instance);

    /// <summary>
    ///     Flashes the taskbar icon using the specified mode and count.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="mode">The flash mode.</param>
    /// <param name="count">The number of times to flash.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [SupportedOSPlatform("windows")]
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_SetTaskbarFlash", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetTaskbarFlash(IntPtr instance, int mode, uint count);

    /// <summary>
    ///     Stops the taskbar icon from flashing.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [SupportedOSPlatform("windows")]
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_StopTaskbarFlash", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus StopTaskbarFlash(IntPtr instance);

    /// <summary>
    ///     Gets whether taskbar progress is supported on the current platform.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="supported">Output: true if taskbar progress is supported.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [SupportedOSPlatform("windows")]
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_GetTaskbarProgressSupported", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus GetTaskbarProgressSupported(IntPtr instance, [MarshalAs(UnmanagedType.I1)] out bool supported);
}
