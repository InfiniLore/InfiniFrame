// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using static InfiniLore.Photino.Native.NativeDll;
using System.Runtime.InteropServices;

namespace InfiniLore.Photino.Native;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class InfiniWindowNative {
    #pragma warning disable SYSLIB1054
    [DllImport(
        DllName,
        EntryPoint = InfiniWindowTests_NativeParametersReturnAsIs,
        CallingConvention = CallingConvention.Cdecl,
        SetLastError = true,
        CharSet = CharSet.Ansi
    )]
    private static extern void NativeParametersReturnAsIs(ref PhotinoNativeParameters parameters, out IntPtr newParameters);
    #pragma warning restore SYSLIB1054

    internal static PhotinoNativeParameters NativeParametersReturnAsIs(ref PhotinoNativeParameters parameters) {
        NativeParametersReturnAsIs(ref parameters, out IntPtr newParameters);
        return Marshal.PtrToStructure<PhotinoNativeParameters>(newParameters);       
    }
}
