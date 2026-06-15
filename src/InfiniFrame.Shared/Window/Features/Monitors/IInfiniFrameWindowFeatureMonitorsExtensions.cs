// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class IInfiniFrameWindowFeatureMonitorsExtensions {
    /// <summary>
    ///     Gets all available monitors for the window.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <returns>A collection of <see cref="InfiniMonitor"/> instances.</returns>
    public static IEnumerable<InfiniMonitor> GetMonitors(this IInfiniFrameWindow window)
        => window.Features.Monitors.GetMonitors();

    /// <summary>
    ///     Gets the main (primary) monitor for the window.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <returns>The main <see cref="InfiniMonitor"/> instance.</returns>
    public static InfiniMonitor GetMainMonitor(this IInfiniFrameWindow window)
        => window.Features.Monitors.GetMainMonitor();

    /// <summary>
    ///     Gets the screen DPI of the main monitor for the window.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <returns>The DPI value of the main monitor.</returns>
    public static int GetMainMonitorScreenDpi(this IInfiniFrameWindow window)
        => window.Features.Monitors.GetMainMonitorScreenDpi();
}
