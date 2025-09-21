using System.Buffers;

namespace InfiniLore.Photino.NET.Utilities;

internal static class MonitorsUtility {
    public static ReadOnlySpan<Monitor> GetMonitors(IntPtr windowHandle)
    {
        const int initialSize = 10;// Start with a reasonable number of monitors
        var monitors = ArrayPool<Monitor>.Shared.Rent(initialSize);
        try
        {
            var index = 0;

            PhotinoNative.GetAllMonitors(windowHandle, Callback);
            return monitors.AsSpan(0, index);

            int Callback(in NativeMonitor monitor)
            {
                // Resize if we run out of space
                if (index >= monitors.Length)
                {
                    var newMonitors = ArrayPool<Monitor>.Shared.Rent(monitors.Length * 2);

                    // Copy existing data into the resized array before returning the old array
                    Array.Copy(monitors, newMonitors, monitors.Length);
                    ArrayPool<Monitor>.Shared.Return(monitors);
                    monitors = newMonitors;
                }

                monitors[index++] = new Monitor(monitor.Monitor, monitor.Work, monitor.Scale);
                return 1;
            }
        }
        finally
        {
            ArrayPool<Monitor>.Shared.Return(monitors);
        }
    }

    public static Monitor[] GetMonitorsAsArray(IntPtr windowHandle)
    {
        return GetMonitors(windowHandle).ToArray();
    }
}
