using InfiniLore.Photino.NET;
using System.Collections.Immutable;
using System.Drawing;

namespace InfiniLore.Photino.Utilities;
using InfiniLore.Photino.Native;

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
        monitor = default;
        if (monitors.IsDefaultOrEmpty) return false;

        long windowArea = Math.Max(0, windowBounds.Width);
        windowArea *= Math.Max(0, windowBounds.Height);

        int bestIndex = -1;
        double bestWindowFraction = -1.0;
        long bestOverlap = 0;

        for (int i = 0; i < monitors.Length; i++) {
            Monitor m = monitors[i];

            Rectangle intersection = Rectangle.Intersect(m.MonitorArea, windowBounds);
            long overlap = 0;
            if (intersection.Width > 0 && intersection.Height > 0) {
                overlap = (long)intersection.Width * intersection.Height;
            }

            // fraction of the *window* that lies on this monitor
            double windowFraction = windowArea > 0 ? (double)overlap / windowArea : 0.0;

            // choose the monitor with the highest fraction (tie-break: larger absolute overlap)
            if (!(windowFraction > bestWindowFraction) && (!(Math.Abs(windowFraction - bestWindowFraction) < double.Epsilon) || overlap <= bestOverlap)) continue;

            bestWindowFraction = windowFraction;
            bestOverlap = overlap;
            bestIndex = i;
        }

        // If we found some overlap, return the monitor with the largest share of the window
        if (bestIndex != -1 && bestOverlap > 0) {
            monitor = monitors[bestIndex];
            return true;
        }

        // No overlap at all: fallback to the nearest monitor by center distance
        var windowCenter = new Point(windowBounds.Left + windowBounds.Width / 2, windowBounds.Top + windowBounds.Height / 2);
        double bestDistSq = double.MaxValue;
        foreach (Monitor m in monitors) {
            Rectangle r = m.MonitorArea;
            var monitorCenter = new Point(r.Left + r.Width / 2, r.Top + r.Height / 2);
            double dx = monitorCenter.X - windowCenter.X;
            double dy = monitorCenter.Y - windowCenter.Y;
            double distSq = dx * dx + dy * dy;
            if (!(distSq < bestDistSq)) continue;

            bestDistSq = distSq;
            monitor = m;
        }

        return true;
    }

    public static bool TryGetCurrentWindowAndMonitor(IPhotinoWindow window, out Rectangle windowRect, out Monitor monitor) {
        ImmutableArray<Monitor> monitors = GetMonitors(window);
        PhotinoNative.GetWindowRectangle(window.InstanceHandle, out windowRect);
        return TryGetCurrentMonitor(monitors, windowRect, out monitor);
    }
}
