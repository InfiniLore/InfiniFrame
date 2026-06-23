// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;

namespace InfiniFrame.NativeBridge.Delegates;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Represents a native callback invoked when the native window is restored from a maximized or minimized state.
/// </summary>
[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
public delegate void CppRestoredDelegate();
