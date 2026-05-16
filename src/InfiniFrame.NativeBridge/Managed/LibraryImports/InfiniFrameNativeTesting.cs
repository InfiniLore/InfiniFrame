// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using static InfiniFrame.NativeBridge.ArtifactManifest;

namespace InfiniFrame.NativeBridge;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static partial class InfiniFrameNativeTesting {
    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniWindowTests_NativeParametersReturnAsIs", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial void NativeParametersReturnAsIsNative(
        [MarshalUsing(typeof(InfiniFrameNativeParametersMarshaller))]
        in InfiniFrameNativeParameters parameters,
        out IntPtr newParameters
    );

    [LibraryImport(NativeLibraryName, EntryPoint = "InfiniWindowTests_FreeInitParams", SetLastError = true), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial void FreeInitParamsNative(IntPtr parameters);

    /// <summary>
    ///     Returns a native pointer to a newly allocated InfiniFrameInitParams clone.
    ///     Ownership is transferred to managed caller, which must call <see cref="FreeInitParams" /> exactly once.
    /// </summary>
    internal static IntPtr NativeParametersReturnAsIsPtr(ref InfiniFrameNativeParameters parameters) {
        NativeParametersReturnAsIsNative(in parameters, out IntPtr newParametersPtr);

        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (newParametersPtr == IntPtr.Zero) throw new InvalidOperationException("Native function returned null pointer");

        return newParametersPtr;
    }

    internal static void FreeInitParams(IntPtr newParametersPtr) {
        if (newParametersPtr == IntPtr.Zero) return;

        FreeInitParamsNative(newParametersPtr);
    }
}
