// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge;
using InfiniFrame.Utilities;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowFeatureMonitors(
    IInfiniFrameWindow window,
    ILogger<InfiniFrameWindowFeatureMonitors> logger
) : IInfiniFrameWindowFeatureMonitors {

    public IEnumerable<InfiniMonitor> GetMonitors() {
        if (window.IsClosedOrClosing()) return [];

        return NativeInvoke.InvokeSyncWithValidation<ImmutableArray<InfiniMonitor>>(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            MonitorsUtility.GetMonitors
        );
    }

    public InfiniMonitor GetMainMonitor() 
        => GetMonitors().FirstOrDefault();
    
    public uint GetMainMonitorScreenDpi() {
        if (window.IsClosedOrClosing()) return 0;
        
        return NativeInvoke.InvokeSyncWithValidation<uint>(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.GetScreenDpi
        );
    }
}

public static class InfiniFrameWindowFeatureMonitorsExtensions {
    // TODO EOD
}