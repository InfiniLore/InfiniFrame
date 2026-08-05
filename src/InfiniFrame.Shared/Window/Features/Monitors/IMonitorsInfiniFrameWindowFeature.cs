// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IMonitorsInfiniFrameWindowFeature {
    /// <summary>
    ///     Gets all available monitors.
    /// </summary>
    /// <returns>A collection of <see cref="InfiniMonitor"/> instances.</returns>
    IEnumerable<InfiniMonitor> GetMonitors();

    /// <summary>
    ///     Gets the main (primary) monitor.
    /// </summary>
    /// <returns>The main <see cref="InfiniMonitor"/> instance.</returns>
    InfiniMonitor GetMainMonitor();

    /// <summary>
    ///     Gets the screen DPI of the main monitor.
    ///     This value is read-only and reflects the per-monitor DPI
    ///     as reported by the operating system. The standard minimum
    ///     DPI is 96 (100% scaling). Higher values indicate display
    ///     scaling (e.g., 192 for 200% scaling on a 4K display).
    /// </summary>
    /// <returns>The DPI value of the main monitor, guaranteed to be at least 96.</returns>
    int GetMainMonitorScreenDpi();
}