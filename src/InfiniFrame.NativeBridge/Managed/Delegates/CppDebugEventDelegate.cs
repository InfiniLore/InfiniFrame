// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;

namespace InfiniFrame.NativeBridge.Delegates;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Represents a native callback invoked when a debug/devtools event occurs in the browser control.
/// </summary>
/// <param name="kind">The kind of debug event.</param>
/// <param name="message">The event message.</param>
/// <param name="level">The log level.</param>
/// <param name="uri">The URI associated with the event.</param>
/// <param name="statusCode">The HTTP status code.</param>
/// <param name="timestampUnixMillisecondsUtc">The event timestamp in UTC milliseconds.</param>
/// <param name="platformPayload">Optional platform-specific payload.</param>
[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
public delegate void CppDebugEventDelegate(
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    string kind,
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    string? message,
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    string? level,
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    string? uri,
    int statusCode,
    long timestampUnixMillisecondsUtc,
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    string? platformPayload
);
