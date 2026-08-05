// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;

namespace InfiniFrame.NativeBridge.Delegates;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Represents a native callback invoked when files are dropped onto the window.
/// </summary>
/// <param name="paths">Pointer to an array of file path strings.</param>
/// <param name="count">Number of file paths in the array.</param>
/// <param name="x">Screen X coordinate of the drop location.</param>
/// <param name="y">Screen Y coordinate of the drop location.</param>
[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
public delegate void CppFileDroppedDelegate(IntPtr paths, int count, int x, int y);
