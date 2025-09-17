using System.Drawing;

namespace InfiniLore.Photino;

/// <summary>
///     Represents information about a monitor.
/// </summary>
public readonly struct Monitor
{
    /// <summary>
    ///     The full area of the monitor.
    /// </summary>
    public readonly Rectangle MonitorArea;

    /// <summary>
    ///     The working area of the monitor excluding taskbars, docked windows, and docked tool bars.
    /// </summary>
    public readonly Rectangle WorkArea;

    /// <summary>
    ///     The scale factor of the monitor. Standard value is 1.0.
    /// </summary>
    public readonly double Scale;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Monitor" /> struct.
    /// </summary>
    /// <param name="monitor">The area of monitor.</param>
    /// <param name="work">The working area of the monitor.</param>
    /// <param name="scale">The scale factor of the monitor.</param>
    public Monitor(Rectangle monitor, Rectangle work, double scale)
    {
        MonitorArea = monitor;
        WorkArea = work;
        Scale = scale;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Monitor" /> struct using native structures.
    /// </summary>
    /// <param name="monitor">The area of monitor as <see cref="NativeRect" /></param>
    /// <param name="work">The working area as <see cref="NativeRect" /></param>
    /// <param name="scale">The scale factor of the monitor.</param>
    public Monitor(NativeRect monitor, NativeRect work, double scale)
        : this(new Rectangle(monitor.x, monitor.y, monitor.width, monitor.height), new Rectangle(work.x, work.y, work.width, work.height), scale)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Monitor" /> struct using a native monitor structure.
    /// </summary>
    /// <param name="nativeMonitor">The native monitor structure.</param>
    public Monitor(NativeMonitor nativeMonitor)
        : this(nativeMonitor.monitor, nativeMonitor.work, nativeMonitor.scale)
    {
    }
}
