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
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void ContextAction(IntPtr context);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void OperationCompletedCallback(
        IntPtr context,
        ulong operationId,
        int result,
        int nativeCode,
        IntPtr failureUtf8
    );
    /// <summary>
    ///     Dispatches a callback to execute synchronously on the native window thread.
    /// </summary>
    /// <param name="instance">The native window instance handle.</param>
    /// <param name="callback">The action to execute on the window thread.</param>
    /// <returns>A status code indicating success or failure.</returns>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_Invoke", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus Invoke(IntPtr instance, Action callback);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_BeginInvoke", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus BeginInvoke(
        IntPtr instance,
        ulong operationId,
        ContextAction callback,
        IntPtr callbackContext,
        OperationCompletedCallback completion,
        IntPtr completionContext
    );

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNative_CancelOperation", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus CancelOperation(IntPtr instance, ulong operationId, int result);
}
