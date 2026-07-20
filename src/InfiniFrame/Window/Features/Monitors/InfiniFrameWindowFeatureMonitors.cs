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

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="IInfiniFrameWindowFeatureMonitors.GetMonitors" />
    public IEnumerable<InfiniMonitor> GetMonitors() {
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (window.IsClosedOrClosing()) return [];

        return MonitorsUtility.GetMonitors(window);
    }

    /// <inheritdoc cref="IInfiniFrameWindowFeatureMonitors.GetMainMonitor" />
    public InfiniMonitor GetMainMonitor()
        => GetMonitors().FirstOrDefault();

    /// <inheritdoc cref="IInfiniFrameWindowFeatureMonitors.GetMainMonitorScreenDpi" />
    public int GetMainMonitorScreenDpi() {
        if (window.IsClosedOrClosing()) return -1;

        return (int)NativeInvoke.InvokeSyncWithValidation<uint>(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.GetScreenDpi
        );
    }
}
