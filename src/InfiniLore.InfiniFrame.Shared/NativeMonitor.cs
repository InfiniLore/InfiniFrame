using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace InfiniLore.Photino;
/// <summary>
///     The <c>NativeMonitor</c> structure is used for communicating information about the monitor setup
///     to and from native system calls. This structure is defined in a sequential layout for direct,
///     unmanaged access to the underlying memory.
/// </summary>
[StructLayout(LayoutKind.Sequential), SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public struct NativeMonitor {
    public NativeRect Monitor { get; set; }
    public NativeRect Work { get; set; }
    public double Scale { get; set; }
}
