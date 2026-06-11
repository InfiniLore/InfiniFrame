// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class IInfiniFrameWindowFeatureMonitorsExtensions {
    public static IEnumerable<InfiniMonitor> GetMonitors(this IInfiniFrameWindow window)
        => window.Features.Monitors.GetMonitors();
    
    public static InfiniMonitor GetMainMonitor(this IInfiniFrameWindow window)
        => window.Features.Monitors.GetMainMonitor();
    
    public static int GetMainMonitorScreenDpi(this IInfiniFrameWindow window)
        => window.Features.Monitors.GetMainMonitorScreenDpi();
}
