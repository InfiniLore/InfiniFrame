// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge;
using InfiniFrame.Utilities;
using Microsoft.Extensions.Logging;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowFeatureMonitors(
    IInfiniFrameWindow window,
    ILogger<InfiniFrameWindowFeatureMonitors> logger
) : IInfiniFrameWindowFeatureMonitors {

    /// <inheritdoc cref="IInfiniFrameWindowFeatureMonitors.GetMonitors"/>
    public IEnumerable<InfiniMonitor> GetMonitors() {
        if (window.IsClosedOrClosing()) return [];

        return MonitorsUtility.GetMonitors(window);
    }

    /// <inheritdoc cref="IInfiniFrameWindowFeatureMonitors.GetMainMonitor"/>
    public InfiniMonitor GetMainMonitor() 
        => GetMonitors().FirstOrDefault();

    /// <inheritdoc cref="IInfiniFrameWindowFeatureMonitors.GetMainMonitorScreenDpi"/>
    public int GetMainMonitorScreenDpi() {
        if (window.IsClosedOrClosing()) return -1;
        
        return (int)NativeInvoke.InvokeSyncWithValidation<uint>(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.GetScreenDpi
        );
    }
}