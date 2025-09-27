using InfiniLore.Photino.NET;
using System.Collections.Immutable;
using System.Drawing;

namespace InfiniLore.Photino.Utilities;

internal static class MonitorsUtility {
    public static ImmutableArray<Monitor> GetMonitors(IPhotinoWindow window) {
        ImmutableArray<Monitor>.Builder builder = ImmutableArray.CreateBuilder<Monitor>();

        PhotinoNative.GetAllMonitors(window.InstanceHandle, Callback);
        return builder.ToImmutable();

        int Callback(in NativeMonitor monitor) {
            builder.Add(new Monitor(monitor.Monitor, monitor.Work, monitor.Scale));
            return 1;
        }
    }
    
    public static bool TryGetCurrentMonitor(ImmutableArray<Monitor> monitors, Rectangle windowBounds, out Monitor monitor) {
        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (Monitor temp in monitors) {
            if (!temp.MonitorArea.IntersectsWith(windowBounds)) continue;

            monitor = temp;
            return true;
        }
        
        monitor = default;
        return false;
    }

    public static bool TryGetCurrentWindowAndMonitor(IPhotinoWindow window, out Rectangle windowRect, out Monitor monitor) {
        ImmutableArray<Monitor> monitors = GetMonitors(window);
        PhotinoNative.GetWindowRectangle(window.InstanceHandle, out windowRect);
        return TryGetCurrentMonitor(monitors, windowRect, out monitor);
    }
}
