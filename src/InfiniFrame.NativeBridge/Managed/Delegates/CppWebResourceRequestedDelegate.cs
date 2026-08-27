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
    /// <summary>The current ABI version for this descriptor. Must be set to 1.</summary>
    public const uint CurrentAbiVersion = 1;
    /// <summary>Body kind value indicating a buffered (in-memory) response body.</summary>
    public const uint BufferedBodyKind = 1;
    /// <summary>Maximum allowed body size in bytes for a buffered response (256 MB).</summary>
    public const ulong MaxBufferedBodyBytes = 256UL * 1024 * 1024;
    /// <summary>Maximum allowed content-type string length in bytes.</summary>
    public const int MaxContentTypeBytes = 1024;

    /// <summary>The size of this structure in bytes, used for ABI compatibility checks.</summary>
    public uint StructSize;
    /// <summary>The ABI version negotiated between managed and native code.</summary>
    public uint AbiVersion;
    /// <summary>The HTTP status code for the custom scheme response (e.g., 200, 404).</summary>
    public uint StatusCode;
    /// <summary>Indicates the kind of body (buffered, streaming, etc.).</summary>
    public uint BodyKind;
    /// <summary>The length of the response body in bytes.</summary>
    public ulong ContentLength;
    /// <summary>Pointer to the response body data. Owned by managed code until <see cref="Release"/> is invoked.</summary>
    public IntPtr Body;
    /// <summary>Pointer to a UTF-8 content-type string. Owned by managed code until <see cref="Release"/> is invoked.</summary>
    public IntPtr ContentTypeUtf8;
    /// <summary>An opaque context value passed back to <see cref="Release"/> for resource cleanup.</summary>
    public IntPtr OwnerContext;
    /// <summary>Function pointer to the release callback that frees resources owned by this descriptor.</summary>
    public IntPtr Release;

    // Reserved function-pointer slots make a future streaming body kind ABI-compatible without changing this prefix.
    /// <summary>Reserved function-pointer slot for future streaming body support.</summary>
    public IntPtr ReservedRead;
    /// <summary>Reserved function-pointer slot for future streaming body support.</summary>
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
