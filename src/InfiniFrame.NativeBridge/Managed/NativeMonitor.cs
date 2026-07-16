// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace InfiniFrame.NativeBridge;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     The <c>NativeMonitor</c> structure is used for communicating information about the monitor setup
///     to and from native system calls. This structure is defined in a sequential layout for direct,
///     unmanaged access to the underlying memory.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public struct NativeMonitor {
    /// <summary>
    ///     The bounding rectangle of the monitor in virtual screen coordinates.
    /// </summary>
    public NativeRect Monitor { get; set; }
    /// <summary>
    ///     The working area rectangle of the monitor (excluding taskbars and docked windows).
    /// </summary>
    public NativeRect Work { get; set; }
    /// <summary>
    ///     The display scale factor of the monitor (e.g. 1.0 for 100%, 1.25 for 125%).
    /// </summary>
    public double Scale { get; set; }
}
