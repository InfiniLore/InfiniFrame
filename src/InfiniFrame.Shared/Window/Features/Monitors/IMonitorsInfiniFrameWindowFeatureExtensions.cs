// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Fluent extension methods for <see cref="IMonitorsInfiniFrameWindowFeature" /> on <see cref="IInfiniFrameWindow" />.
/// </summary>
public static class IMonitorsInfiniFrameWindowFeatureExtensions {
    /// <param name="window">The window instance.</param>
    extension(IInfiniFrameWindow window) {
        /// <summary>
        ///     Gets all available monitors for the window.
        /// </summary>
        /// <returns>A collection of <see cref="InfiniMonitor" /> instances.</returns>
        public IEnumerable<InfiniMonitor> GetMonitors()
            => window.Features.Monitors.GetMonitors();
        /// <summary>
        ///     Gets the main (primary) monitor for the window.
        /// </summary>
        /// <returns>The main <see cref="InfiniMonitor" /> instance.</returns>
        public InfiniMonitor GetMainMonitor()
            => window.Features.Monitors.GetMainMonitor();
        /// <summary>
        ///     Gets the screen DPI of the main monitor for the window.
        /// </summary>
        /// <returns>The DPI value of the main monitor.</returns>
        public int GetMainMonitorScreenDpi()
            => window.Features.Monitors.GetMainMonitorScreenDpi();
    }

}
