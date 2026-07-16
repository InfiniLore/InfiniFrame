// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge;
using InfiniFrame.NativeBridge.Delegates;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Immutable;
using System.Drawing;

namespace InfiniFrame.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Provides utility methods for retrieving and working with monitor information.
/// </summary>
internal static class MonitorsUtility {
    /// <summary>
    ///     Retrieves all monitors available to the specified window.
    /// </summary>
    /// <param name="window">The window instance used to query monitor information.</param>
    /// <returns>An immutable array of <see cref="InfiniMonitor" /> structs representing all available monitors.</returns>
    public static ImmutableArray<InfiniMonitor> GetMonitors(IInfiniFrameWindow window) {
        ImmutableArray<InfiniMonitor>.Builder builder = ImmutableArray.CreateBuilder<InfiniMonitor>();

        NativeInvoke.InvokeSyncWithValidation(
            NullLogger<IInfiniFrameWindow>.Instance,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.GetAllMonitors,
            (CppGetAllMonitorsDelegate) Callback
        );
        return builder.ToImmutable();

        int Callback(in NativeMonitor monitor) {
            builder.Add(new InfiniMonitor(monitor.Monitor, monitor.Work, monitor.Scale));
            return 1;
        }
    }
    
    /// <summary>
    ///     Attempts to determine the monitor that contains or is nearest to the specified window bounds.
    /// </summary>
    /// <param name="monitors">The array of available monitors.</param>
    /// <param name="windowBounds">The bounds of the window.</param>
    /// <param name="monitor">When this method returns, contains the best matching monitor.</param>
    /// <returns><c>true</c> if a monitor was found; otherwise, <c>false</c>.</returns>
    public static bool TryGetCurrentMonitor(ImmutableArray<InfiniMonitor> monitors, Rectangle windowBounds, out InfiniMonitor monitor) {
        monitor = default;
        if (monitors.IsDefaultOrEmpty) return false;

        long windowArea = Math.Max(0, windowBounds.Width);
        windowArea *= Math.Max(0, windowBounds.Height);

        int bestIndex = -1;
        double bestWindowFraction = -1.0;
        long bestOverlap = 0;

        for (int i = 0; i < monitors.Length; i++) {
            InfiniMonitor m = monitors[i];

            Rectangle intersection = Rectangle.Intersect(m.MonitorArea, windowBounds);
            long overlap = 0;
            if (intersection.Width > 0 && intersection.Height > 0) {
                overlap = (long)intersection.Width * intersection.Height;
            }

            // fraction of the *window* that lies on this monitor
            double windowFraction = windowArea > 0 ? (double)overlap / windowArea : 0.0;

            // choose the monitor with the highest fraction (tie-break: larger absolute overlap)
            bool isBetter = windowFraction > bestWindowFraction
                || Math.Abs(windowFraction - bestWindowFraction) < double.Epsilon
                && overlap > bestOverlap;
            if (!isBetter) continue;

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
        foreach (InfiniMonitor m in monitors) {
            Rectangle r = m.MonitorArea;
            var monitorCenter = new Point(r.Left + r.Width / 2, r.Top + r.Height / 2);
            double dx = monitorCenter.X - windowCenter.X;
            double dy = monitorCenter.Y - windowCenter.Y;
            double distSq = dx * dx + dy * dy;
            if (distSq >= bestDistSq) continue;

            bestDistSq = distSq;
            monitor = m;
        }

        return true;
    }

    /// <summary>
    ///     Attempts to retrieve the current window bounds and the monitor it is on.
    /// </summary>
    /// <param name="window">The window instance.</param>
    /// <param name="windowRect">When this method returns, contains the window bounds.</param>
    /// <param name="monitor">When this method returns, contains the monitor the window is on.</param>
    /// <returns><c>true</c> if the window and monitor information was retrieved; otherwise, <c>false</c>.</returns>
    public static bool TryGetCurrentWindowAndMonitor(IInfiniFrameWindow window, out Rectangle windowRect, out InfiniMonitor monitor) {
        ImmutableArray<InfiniMonitor> monitors = GetMonitors(window);

        (int x, int y) = NativeInvoke.InvokeSyncWithValidation<int, int>(
            NullLogger<IInfiniFrameWindow>.Instance, 
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.GetPosition
        );
        
        (int width, int height) = NativeInvoke.InvokeSyncWithValidation<int, int>(
            NullLogger<IInfiniFrameWindow>.Instance, 
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.GetSize
        );
        
        windowRect = new Rectangle(x, y, width, height);
        
        // ReSharper disable once InvertIf

        return TryGetCurrentMonitor(monitors, windowRect, out monitor);
    }
}
