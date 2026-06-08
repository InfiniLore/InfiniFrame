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
public partial class InfiniFrameNativeTesting {
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNativeTests_NativeParametersReturnAsIs", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial InfiniFrameNativeInteropStatus NativeParametersReturnAsIsNative(
        [MarshalUsing(typeof(InfiniFrameNativeParametersMarshaller))]
        in InfiniFrameNativeParameters parameters,
        out IntPtr newParameters
    );

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNativeTests_FreeInitParams", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial InfiniFrameNativeInteropStatus FreeInitParamsNative(IntPtr parameters);

    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNativeTests_IsColorSchemeChange", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial InfiniFrameNativeInteropStatus IsColorSchemeChangeNative(IntPtr lParam, out int result);

    /// <summary>
    ///     Returns a native pointer to a newly allocated InfiniFrameInitParams clone.
    ///     Ownership is transferred to managed caller, which must call <see cref="FreeInitParams" /> exactly once.
    /// </summary>
    internal static InfiniFrameNativeInteropStatus NativeParametersReturnAsIsPtr(ref InfiniFrameNativeParameters parameters, out IntPtr newParametersPtr) {
        var status = NativeParametersReturnAsIsNative(in parameters, out newParametersPtr);

        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (newParametersPtr == IntPtr.Zero) throw new InvalidOperationException("Native function returned null pointer");

        return status;
    }

    internal static InfiniFrameNativeInteropStatus FreeInitParams(IntPtr newParametersPtr) 
        => FreeInitParamsNative(newParametersPtr);

    internal static InfiniFrameNativeInteropStatus IsColorSchemeChange(IntPtr lParam, out bool result) {
        InfiniFrameNativeInteropStatus status = IsColorSchemeChangeNative(lParam, out int resultInt);

        result = resultInt != 0;
        return status;
    }
}
