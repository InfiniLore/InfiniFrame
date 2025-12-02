// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;
using static InfiniFrame.Native.NativeDll;

namespace InfiniFrame.Native;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class InfiniWindowNative {
    #pragma warning disable SYSLIB1054
    [DllImport(
        DllName,
        EntryPoint = InfiniWindowTests_NativeParametersReturnAsIs,
        CallingConvention = CallingConvention.Cdecl,
        SetLastError = true
    )]
    private static extern void NativeParametersReturnAsIs(
        [In] ref InfiniFrameNativeParameters parameters,
        out IntPtr newParameters
    );
    #pragma warning restore SYSLIB1054

    internal static InfiniFrameNativeParameters NativeParametersReturnAsIs(ref InfiniFrameNativeParameters parameters) {
        NativeParametersReturnAsIs(ref parameters, out IntPtr newParametersPtr);

        if (newParametersPtr == IntPtr.Zero) throw new InvalidOperationException("Native function returned null pointer");

        try {
            // Marshal with explicit type to ensure proper handling
            var result = Marshal.PtrToStructure<InfiniFrameNativeParameters>(newParametersPtr);

            // Don't free the pointer - the C++ side allocated it with 'new' 
            // and should manage its lifetime, or you need a corresponding delete call
            return result;
        }
        catch (Exception ex) {
            throw new InvalidOperationException($"Failed to marshal returned structure from native code. Pointer: {newParametersPtr:X}", ex);
        }
    }
}
