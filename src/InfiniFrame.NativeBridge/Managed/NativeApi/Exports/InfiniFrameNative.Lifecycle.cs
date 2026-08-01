// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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
    private static partial InfiniFrameNativeInteropStatus ConstructorNative(IntPtr parameters, out IntPtr value);

    internal static InfiniFrameNativeInteropStatus Constructor(
        in InfiniFrameNativeParameters parameters,
        out IntPtr value
    ) {
        var marshaller = new InfiniFrameNativeParametersMarshaller.ManagedToUnmanagedIn();
        marshaller.FromManaged(parameters);
        InfiniFrameNativeParametersMarshaller.Unmanaged unmanaged = marshaller.ToUnmanaged();
        IntPtr unmanagedPtr = IntPtr.Zero;

        try {
            unmanagedPtr = Marshal.AllocHGlobal(Marshal.SizeOf<InfiniFrameNativeParametersMarshaller.Unmanaged>());
            Marshal.StructureToPtr(unmanaged, unmanagedPtr, false);
            return ConstructorNative(unmanagedPtr, out value);
        }
        finally {
            if (unmanagedPtr != IntPtr.Zero) Marshal.FreeHGlobal(unmanagedPtr);
            marshaller.Free();
        }
    }

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

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_SetReadyCallback", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetReadyCallback(IntPtr instance, ContextAction callback, IntPtr context);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_SetTeardownCallback", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus SetTeardownCallback(IntPtr instance, ContextAction callback, IntPtr context);
}
