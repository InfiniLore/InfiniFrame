// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;

namespace InfiniFrame.NativeBridge.Delegates;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/// <summary>
///     Versioned custom-scheme response descriptor. The native caller owns the descriptor itself (it is stack allocated),
///     while managed code owns every pointer stored in it until <see cref="Release" /> is invoked exactly once.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct CustomSchemeResponse {
    public const uint CurrentAbiVersion = 1;
    public const uint BufferedBodyKind = 1;
    public const ulong MaxBufferedBodyBytes = 256UL * 1024 * 1024;
    public const int MaxContentTypeBytes = 1024;

    public uint StructSize;
    public uint AbiVersion;
    public uint StatusCode;
    public uint BodyKind;
    public ulong ContentLength;
    public IntPtr Body;
    public IntPtr ContentTypeUtf8;
    public IntPtr OwnerContext;
    public IntPtr Release;

    // Reserved function-pointer slots make a future streaming body kind ABI-compatible without changing this prefix.
    public IntPtr ReservedRead;
    public IntPtr ReservedSeek;
}

/// <summary>
///     Produces a custom-scheme response in a caller-owned descriptor.
/// </summary>
/// <param name="url">Platform-native URL string (UTF-16 on Windows, UTF-8 on Unix).</param>
/// <param name="response">Caller-owned descriptor, initially zeroed.</param>
/// <returns>Non-zero when a response was produced; zero for not found or handler failure.</returns>
[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
public delegate int CppWebResourceRequestedDelegate(
    [MarshalAs(UnmanagedType.LPUTF8Str)]
    string url,
    ref CustomSchemeResponse response
);
