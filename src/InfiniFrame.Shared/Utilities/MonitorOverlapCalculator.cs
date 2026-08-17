// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections.Immutable;
using System.Drawing;

namespace InfiniFrame.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Pure geometry logic for monitor overlap and nearest-monitor computation.
///     Extracted from <see cref="MonitorsUtility"/> for testability.
/// </summary>
public static class MonitorOverlapCalculator {

    /// <summary>
    ///     Determines which monitor contains or is nearest to the specified window bounds.
    ///     Uses overlap fraction (primary) and Euclidean distance (fallback).
    /// </summary>
    public static bool TryFindBestMonitor(ImmutableArray<InfiniMonitor> monitors, Rectangle windowBounds, out int bestIndex) {
        bestIndex = -1;
        if (monitors.IsDefaultOrEmpty) return false;

        long windowArea = Math.Max(0, (long)windowBounds.Width);
        windowArea *= Math.Max(0, windowBounds.Height);

        double bestWindowFraction = -1.0;
        long bestOverlap = 0;

        for (int i = 0; i < monitors.Length; i++) {
            InfiniMonitor m = monitors[i];

            Rectangle intersection = Rectangle.Intersect(m.MonitorArea, windowBounds);
            long overlap = 0;
            if (intersection.Width > 0 && intersection.Height > 0) {
                overlap = intersection.Width * (long)intersection.Height;
            }

            double windowFraction = windowArea > 0 ? (double)overlap / windowArea : 0.0;

            bool isBetter = windowFraction > bestWindowFraction
                || Math.Abs(windowFraction - bestWindowFraction) < double.Epsilon
                && overlap > bestOverlap;
            if (!isBetter) continue;

            bestWindowFraction = windowFraction;
            bestOverlap = overlap;
            bestIndex = i;
        }

        if (bestIndex != -1 && bestOverlap > 0) return true;

        // Fallback: nearest monitor by center distance
        var windowCenter = new Point(windowBounds.Left + windowBounds.Width / 2, windowBounds.Top + windowBounds.Height / 2);
        double bestDistSq = double.MaxValue;
        foreach (InfiniMonitor m in monitors) {
            Rectangle r = m.MonitorArea;
            var monitorCenter = new Point(r.Left + r.Width / 2, r.Top + r.Height / 2);
            double dx = monitorCenter.X - windowCenter.X;
            double dy = monitorCenter.Y - windowCenter.Y;
            double distSq = dx * dx + dy * dy;
            if (distSq >= bestDistSq) continue;

            bestDistSq = distSq;
            bestIndex = Array.IndexOf(monitors.ToArray(), m);
        }

        return true;
    }
}
