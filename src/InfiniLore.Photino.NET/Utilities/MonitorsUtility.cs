using System.Collections.Immutable;

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
}
