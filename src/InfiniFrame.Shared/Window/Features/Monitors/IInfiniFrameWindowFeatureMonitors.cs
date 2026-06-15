// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowFeatureMonitors {
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
    /// </summary>
    /// <returns>The DPI value of the main monitor.</returns>
    int GetMainMonitorScreenDpi();
}
