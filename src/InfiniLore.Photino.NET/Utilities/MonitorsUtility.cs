using System.Collections.Immutable;
using System.Drawing;

namespace InfiniLore.Photino.NET.Utilities;

internal static class MonitorsUtility {
    public static ImmutableArray<Monitor> GetMonitors(IntPtr windowHandle) {
        ImmutableArray<Monitor>.Builder builder = ImmutableArray.CreateBuilder<Monitor>();

        PhotinoNative.GetAllMonitors(windowHandle, Callback);
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
}
