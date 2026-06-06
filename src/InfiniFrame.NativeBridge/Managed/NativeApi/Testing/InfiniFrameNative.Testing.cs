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
    internal static IntPtr NativeParametersReturnAsIsPtr(ref InfiniFrameNativeParameters parameters) {
        InfiniFrameNativeInteropStatus status = NativeParametersReturnAsIsNative(in parameters, out IntPtr newParametersPtr);
        if (!InfiniFrameNative.EnsureSucceeded(status, out string? errorMessage)) 
            throw new InvalidOperationException($"Native function returned error: {errorMessage}");

        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (newParametersPtr == IntPtr.Zero) throw new InvalidOperationException("Native function returned null pointer");

        return newParametersPtr;
    }

    internal static void FreeInitParams(IntPtr newParametersPtr) {
        if (newParametersPtr == IntPtr.Zero) return;

        InfiniFrameNativeInteropStatus status = FreeInitParamsNative(newParametersPtr);
        if (!InfiniFrameNative.EnsureSucceeded(status, out string? errorMessage)) 
            throw new InvalidOperationException($"Native function returned error: {errorMessage}");
    }

    internal static bool IsColorSchemeChange(IntPtr lParam) {
        InfiniFrameNativeInteropStatus status = IsColorSchemeChangeNative(lParam, out int result);
        if (!InfiniFrameNative.EnsureSucceeded(status, out string? errorMessage)) 
            throw new InvalidOperationException($"Native function returned error: {errorMessage}");

        return result != 0;
    }
}
