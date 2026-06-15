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

    /// <summary>
    /// Retrieves a collection of monitors associated with the current window.
    /// If the window is in a closed or closing state, an empty collection is returned.
    /// Otherwise, this method synchronously invokes a native operation to retrieve
    /// monitor information for the window.
    /// </summary>
    /// <returns>
    /// An enumerable collection of InfiniMonitor objects representing the monitors
    /// associated with the window, or an empty collection if the window is closed or closing.
    /// </returns>
    public IEnumerable<InfiniMonitor> GetMonitors() {
        if (window.IsClosedOrClosing()) return [];

        return MonitorsUtility.GetMonitors(window);
    }

    /// <summary>
    /// Retrieves the primary monitor from the list of available monitors.
    /// </summary>
    /// <returns>
    /// The primary monitor of type <see cref="InfiniMonitor"/> if found,
    /// otherwise the default value of <see cref="InfiniMonitor"/>.
    /// </returns>
    public InfiniMonitor GetMainMonitor() 
        => GetMonitors().FirstOrDefault();

    /// <summary>
    /// Retrieves the screen DPI (dots per inch) of the main monitor associated with the current window.
    /// Returns -1 if the window is closed or in the process of closing.
    /// </summary>
    /// <return>The screen DPI of the main monitor as an integer, or -1 if the window is unavailable.</return>
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