using System.Drawing;

namespace InfiniLore.Photino;

// ReSharper disable twice NotAccessedPositionalProperty.Global
/// <summary>
///     Represents information about a monitor.
/// </summary>
/// <param name="MonitorArea">The full area of the monitor</param>
/// <param name="WorkArea">The working area of the monitor excluding taskbars, docked windows, and docked tool bars.</param>
/// <param name="Scale">The scale factor of the monitor. The standard value is 1.0.</param>
public readonly record struct Monitor(Rectangle MonitorArea, Rectangle WorkArea, double Scale) {
    /// <summary>
    ///     Initializes a new instance of the <see cref="Monitor" /> struct using native structures.
    /// </summary>
    /// <param name="monitor">The area of monitor as <see cref="NativeRect" /></param>
    /// <param name="work">The working area as <see cref="NativeRect" /></param>
    /// <param name="scale">The scale factor of the monitor.</param>
    public Monitor(NativeRect monitor, NativeRect work, double scale)
        : this(
        new Rectangle(monitor.X, monitor.Y, monitor.Width, monitor.Height),
        new Rectangle(work.X, work.Y, work.Width, work.Height),
        scale
        ) {
    }
}
