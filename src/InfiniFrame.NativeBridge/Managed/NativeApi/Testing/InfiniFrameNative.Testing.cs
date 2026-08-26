// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniFrame.NativeBridge;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static partial class InfiniFrameNativeTesting {
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNativeTests_MacPooledHostCount", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial InfiniFrameNativeInteropStatus MacPooledHostCountNative(out nuint value);

    public static nuint MacPooledHostCount() {
        if (!OperatingSystem.IsMacOS()) throw new PlatformNotSupportedException();

        InfiniFrameNativeInteropStatus status = MacPooledHostCountNative(out nuint value);
        if (status != InfiniFrameNativeInteropStatus.Success) throw new InvalidOperationException($"Native pool query failed: {status}");

        return value;
    }
    /// <summary>
    ///     Native test helper that returns native parameters as-is for round-trip verification.
    /// </summary>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNativeTests_NativeParametersReturnAsIs", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial InfiniFrameNativeInteropStatus NativeParametersReturnAsIsNative(IntPtr parameters, out IntPtr newParameters);

    /// <summary>
    ///     Native test helper that frees init parameters allocated by native code.
    /// </summary>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNativeTests_FreeInitParams", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial InfiniFrameNativeInteropStatus FreeInitParamsNative(IntPtr parameters);

    /// <summary>Cross-platform native consumer used to verify the custom-scheme ABI and release callback.</summary>
    [LibraryImport(ArtifactManifest.NativeLibraryName, EntryPoint = "InfiniFrameNativeTests_ConsumeCustomSchemeResponse", SetLastError = true)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial InfiniFrameNativeInteropStatus ConsumeCustomSchemeResponse(
        IntPtr callback,
        out ulong contentLength,
        out uint byteSum,
        out int valid
    );

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
        var marshaller = new InfiniFrameNativeParametersMarshaller.ManagedToUnmanagedIn();
        marshaller.FromManaged(parameters);
        var unmanaged = marshaller.ToUnmanaged();
        InfiniFrameNativeInteropStatus status;
        IntPtr unmanagedPtr = IntPtr.Zero;

        try {
            unmanagedPtr = Marshal.AllocHGlobal(Marshal.SizeOf<InfiniFrameNativeParametersMarshaller.Unmanaged>());
            Marshal.StructureToPtr(unmanaged, unmanagedPtr, false);
            status = NativeParametersReturnAsIsNative(unmanagedPtr, out newParametersPtr);
        }
        finally {
            if (unmanagedPtr != IntPtr.Zero) Marshal.FreeHGlobal(unmanagedPtr);
            marshaller.Free();
        }

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
