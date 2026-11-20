// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Provides information about connected displays including work area, DPI, and monitor enumeration.
/// </summary>
public interface IMonitorsInfiniFrameWindowFeature {
    /// <summary>
    ///     Gets all available monitors.
    /// </summary>
    /// <returns>A collection of <see cref="InfiniMonitor" /> instances.</returns>
    IEnumerable<InfiniMonitor> GetMonitors();

    /// <summary>
    ///     Gets the main (primary) monitor.
    /// </summary>
    /// <returns>The main <see cref="InfiniMonitor" /> instance.</returns>
    InfiniMonitor GetMainMonitor();

    /// <summary>
    ///     Gets the screen DPI of the main monitor.
    ///     This value is read-only and reflects the per-monitor DPI
    ///     as reported by the operating system. The standard minimum
    ///     DPI is 96 (100% scaling). Higher values indicate display
    ///     scaling (e.g., 192 for 200% scaling on a 4K display).
    ///     On Linux this is the scale-derived logical DPI (96 × scale
    ///     factor): GTK4 removed physical screen-DPI queries, and the
    ///     scale-derived value keeps native and JavaScript DPI reporting
    ///     in parity. It matches the historical value (96) on unscaled
    ///     displays.
    /// </summary>
    /// <returns>The DPI value of the main monitor, guaranteed to be at least 96.</returns>
    int GetMainMonitorScreenDpi();
}
