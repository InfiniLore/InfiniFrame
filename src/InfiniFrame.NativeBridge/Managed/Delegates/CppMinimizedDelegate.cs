// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;

namespace InfiniFrame.NativeBridge.Delegates;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Represents a native callback invoked when the native window is minimized.
/// </summary>
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void CppMinimizedDelegate();