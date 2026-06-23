// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;

namespace InfiniFrame.NativeBridge.Delegates;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Represents a native callback invoked when a custom scheme resource is requested.
///     Returns a pointer to the resource data, the number of bytes, and the content type.
/// </summary>
/// <param name="url">The URL of the requested resource.</param>
/// <param name="outNumBytes">The number of bytes in the returned data.</param>
/// <param name="outContentType">The MIME content type of the resource.</param>
/// <returns>A native pointer to the resource data, or <see cref="IntPtr.Zero"/> if not found.</returns>
[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
public delegate IntPtr CppWebResourceRequestedDelegate(string url, out int outNumBytes, out string? outContentType);
