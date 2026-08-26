// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;

namespace InfiniFrame.NativeBridge.Delegates;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Represents a native callback invoked when a web message is received from the browser control's JavaScript.
/// </summary>
/// <param name="message">The message content.</param>
/// <param name="origin">The origin of the message.</param>
[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
public delegate void CppWebMessageReceivedDelegate(
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    string message,
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    string? origin
);
