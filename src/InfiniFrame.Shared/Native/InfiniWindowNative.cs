// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Runtime.CompilerServices;
using static InfiniFrame.Native.NativeDll;

namespace InfiniFrame.Native;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static partial class InfiniWindowNative {
    [LibraryImport( DllName, EntryPoint = InfiniWindowTests_NativeParametersReturnAsIs, SetLastError = true),
     UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial void NativeParametersReturnAsIs(
        [MarshalUsing(typeof(InfiniFrameNativeParametersMarshaller))] in InfiniFrameNativeParameters parameters,
        out IntPtr newParameters
    );

    internal static InfiniFrameNativeParameters NativeParametersReturnAsIs(ref InfiniFrameNativeParameters parameters) {
        NativeParametersReturnAsIs(in parameters, out IntPtr newParametersPtr);

        if (newParametersPtr == IntPtr.Zero) throw new InvalidOperationException("Native function returned null pointer");

        try {
            // Marshal with explicit type to ensure proper handling
            var result = Marshal.PtrToStructure<InfiniFrameNativeParameters>(newParametersPtr);

            // Don't free the pointer - the C++ side allocated it with 'new' 
            // and should manage its lifetime, or you need a corresponding delete call
            return result;
        }
        catch (ArgumentException ex) {
            throw new InvalidOperationException($"Failed to marshal returned structure from native code. Pointer: {newParametersPtr:X}", ex);
        }
        catch (InvalidOperationException ex) {
            throw new InvalidOperationException($"Failed to marshal returned structure from native code. Pointer: {newParametersPtr:X}", ex);
        }
    }
}
