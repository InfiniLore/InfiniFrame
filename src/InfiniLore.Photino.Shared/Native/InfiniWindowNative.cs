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
    [DllImport(DllName, EntryPoint = InfiniWindowTests_NativeParametersReturnAsIs, CallingConvention = CallingConvention.Cdecl, SetLastError = true, CharSet = CharSet.Ansi)]
    internal static extern PhotinoNativeParameters NativeParametersReturnAsIs(ref PhotinoNativeParameters parameters);
}
