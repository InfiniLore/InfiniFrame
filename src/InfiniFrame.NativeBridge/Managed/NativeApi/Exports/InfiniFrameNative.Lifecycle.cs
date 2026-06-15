// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace InfiniFrame.NativeBridge;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public partial class InfiniFrameNative {
    /// <summary>
    ///     Creates a new native window instance with the specified parameters.
    /// </summary>
    /// <param name="parameters">The initialization parameters.</param>
    /// <param name="value">The created native window instance handle.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_ctor", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus Constructor([MarshalUsing(typeof(InfiniFrameNativeParametersMarshaller))] in InfiniFrameNativeParameters parameters, out IntPtr value);

    /// <summary>
    ///     Destroys the native window instance and releases its resources.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_dtor")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus Destructor(IntPtr instance);

    /// <summary>
    ///     Closes the native window.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_Close", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus Close(IntPtr instance);

    /// <summary>
    ///     Blocks until the native window has been closed and all resources are released.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_WaitForExit", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus WaitForExit(IntPtr instance);
}
