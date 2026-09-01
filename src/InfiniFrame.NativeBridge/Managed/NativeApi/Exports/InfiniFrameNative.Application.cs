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
    ///     Creates a new native application instance with the specified parameters.
    /// </summary>
    /// <param name="parameters">The application initialization parameters.</param>
    /// <param name="value">The created native application instance handle.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_Application_ctor", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus ApplicationConstructor(IntPtr parameters, out IntPtr value);

    /// <summary>
    ///     Destroys the native application instance and releases its resources.
    /// </summary>
    /// <param name="instance">The native application instance handle.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_Application_dtor")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus ApplicationDestructor(IntPtr instance);

    /// <summary>
    ///     Runs the application message loop, blocking until all windows close or Shutdown is called.
    /// </summary>
    /// <param name="instance">The native application instance handle.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_Application_Run", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus ApplicationRun(IntPtr instance);

    /// <summary>
    ///     Signals the application message loop to exit.
    /// </summary>
    /// <param name="instance">The native application instance handle.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_Application_Shutdown", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus ApplicationShutdown(IntPtr instance);

    /// <summary>
    ///     Checks if Shutdown has been called on the application.
    /// </summary>
    /// <param name="instance">The native application instance handle.</param>
    /// <param name="value">Receives true if shutdown was requested.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_Application_IsShutdownRequested", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus ApplicationIsShutdownRequested(IntPtr instance, out byte value);

    /// <summary>
    ///     Registers the Win32 window class and sets DPI awareness. Windows only.
    /// </summary>
    /// <param name="instance">The native application instance handle.</param>
    /// <param name="hInstance">The Win32 HINSTANCE handle.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [SupportedOSPlatform("windows")]
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_Application_register_win32", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus ApplicationRegisterWin32(IntPtr instance, IntPtr hInstance);

    /// <summary>
    ///     Sets up NSApplication delegate and activation policy. macOS only.
    /// </summary>
    /// <param name="instance">The native application instance handle.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [SupportedOSPlatform("macos")]
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_Application_register_mac", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus ApplicationRegisterMac(IntPtr instance);
}
