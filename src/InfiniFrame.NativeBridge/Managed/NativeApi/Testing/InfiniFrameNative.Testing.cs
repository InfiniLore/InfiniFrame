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
    /// <summary>
    ///     Native test helper that returns native parameters as-is for round-trip verification.
    /// </summary>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNativeTests_NativeParametersReturnAsIs", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial InfiniFrameNativeInteropStatus NativeParametersReturnAsIsNative(
        [MarshalUsing(typeof(InfiniFrameNativeParametersMarshaller))]
        in InfiniFrameNativeParameters parameters,
        out IntPtr newParameters
    );

    /// <summary>
    ///     Native test helper that frees init parameters allocated by native code.
    /// </summary>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNativeTests_FreeInitParams", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial InfiniFrameNativeInteropStatus FreeInitParamsNative(IntPtr parameters);

    /// <summary>
    ///     Native test helper that checks if a Windows message indicates a color scheme change.
    /// </summary>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNativeTests_IsColorSchemeChange", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial InfiniFrameNativeInteropStatus IsColorSchemeChangeNative(IntPtr lParam, out int result);

    /// <summary>
    ///     Returns a native pointer to a newly allocated InfiniFrameInitParams clone.
    ///     Ownership is transferred to managed caller, which must call <see cref="FreeInitParams" /> exactly once.
    /// </summary>
    /// <param name="parameters">The parameters to clone.</param>
    /// <param name="newParametersPtr">The native pointer to the cloned parameters.</param>
    /// <returns>A status code indicating success or failure.</returns>
    internal static InfiniFrameNativeInteropStatus NativeParametersReturnAsIsPtr(ref InfiniFrameNativeParameters parameters, out IntPtr newParametersPtr) {
        var status = NativeParametersReturnAsIsNative(in parameters, out newParametersPtr);

        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (newParametersPtr == IntPtr.Zero) throw new InvalidOperationException("Native function returned null pointer");

        return status;
    }

    /// <summary>
    ///     Frees init parameters that were allocated by native code during testing.
    /// </summary>
    /// <param name="newParametersPtr">The native pointer to free.</param>
    /// <returns>A status code indicating success or failure.</returns>
    internal static InfiniFrameNativeInteropStatus FreeInitParams(IntPtr newParametersPtr) 
        => FreeInitParamsNative(newParametersPtr);

    /// <summary>
    ///     Checks whether a window message indicates a color scheme change.
    /// </summary>
    /// <param name="lParam">The lParam from the window message.</param>
    /// <param name="result">Whether the message indicates a color scheme change.</param>
    /// <returns>A status code indicating success or failure.</returns>
    internal static InfiniFrameNativeInteropStatus IsColorSchemeChange(IntPtr lParam, out bool result) {
        InfiniFrameNativeInteropStatus status = IsColorSchemeChangeNative(lParam, out int resultInt);

        result = resultInt != 0;
        return status;
    }
}
