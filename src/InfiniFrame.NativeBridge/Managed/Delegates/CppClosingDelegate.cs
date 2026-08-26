// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;

namespace InfiniFrame.NativeBridge.Delegates;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Represents a native callback invoked when the native window is about to close.
///     Return non-zero to cancel closing, zero to allow it.
/// </summary>
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate byte CppClosingDelegate();
