using System.Buffers;

namespace InfiniLore.Photino.NET.Utilities;

internal static class MonitorsUtility
{
    public static ReadOnlySpan<Monitor> GetMonitors(IntPtr windowHandle)
    {
        var monitors = ArrayPool<Monitor>.Shared.Rent(3);
        try
        {
            var index = 0;
      
            PhotinoNative.GetAllMonitors(windowHandle, Callback);
            return monitors.AsSpan(0, index);
        
            int Callback(in NativeMonitor monitor)
            {
                if (index >= monitors.Length)
                {
                    ArrayPool<Monitor>.Shared.Return(monitors);
                    monitors = ArrayPool<Monitor>.Shared.Rent(monitors.Length * 2);
                    index = 0;
                }
                monitors[index++] = new Monitor(monitor);
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
