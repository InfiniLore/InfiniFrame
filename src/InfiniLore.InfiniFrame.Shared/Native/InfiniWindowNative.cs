// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;
using static InfiniLore.InfiniFrame.Native.NativeDll;

namespace InfiniLore.InfiniFrame.Native;
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
    private static extern void NativeParametersReturnAsIs(ref InfiniFrameNativeParameters parameters, out IntPtr newParameters);
    #pragma warning restore SYSLIB1054

    internal static InfiniFrameNativeParameters NativeParametersReturnAsIs(ref InfiniFrameNativeParameters parameters) {
        NativeParametersReturnAsIs(ref parameters, out IntPtr newParameters);
        return Marshal.PtrToStructure<InfiniFrameNativeParameters>(newParameters);
    }
}
