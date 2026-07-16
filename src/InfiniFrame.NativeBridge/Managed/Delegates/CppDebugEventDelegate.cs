// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;

namespace InfiniFrame.NativeBridge.Delegates;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
public delegate void CppDebugEventDelegate(
    string kind,
    string? message,
    string? level,
    string? uri,
    int statusCode,
    long timestampUnixMillisecondsUtc,
    string? platformPayload
);
