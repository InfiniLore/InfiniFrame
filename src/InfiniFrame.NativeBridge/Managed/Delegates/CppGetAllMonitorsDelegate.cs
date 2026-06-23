// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Runtime.InteropServices;

namespace InfiniFrame.NativeBridge.Delegates;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Represents a native callback invoked for each monitor during monitor enumeration.
///     Return non-zero to continue enumeration, zero to stop.
/// </summary>
/// <param name="monitor">The monitor information.</param>
/// <returns>Non-zero to continue enumeration, zero to stop.</returns>
[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Auto)]
public delegate int CppGetAllMonitorsDelegate(in NativeMonitor monitor);
