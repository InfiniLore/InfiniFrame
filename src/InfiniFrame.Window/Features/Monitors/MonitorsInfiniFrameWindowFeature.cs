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
/// <summary>
///     Runtime feature implementation for querying connected monitors, retrieving the main monitor, and obtaining
///     screen DPI information.
/// </summary>
public class MonitorsInfiniFrameWindowFeature(
    IInfiniFrameWindow window,
    ILogger<MonitorsInfiniFrameWindowFeature> logger
) : IMonitorsInfiniFrameWindowFeature {

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="IMonitorsInfiniFrameWindowFeature.GetMonitors" />
    public IEnumerable<InfiniMonitor> GetMonitors() {
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (window.IsClosedOrClosing()) return [];

        return MonitorsUtility.GetMonitors(window);
    }

    /// <inheritdoc cref="IMonitorsInfiniFrameWindowFeature.GetMainMonitor" />
    public InfiniMonitor GetMainMonitor()
        => GetMonitors().FirstOrDefault();

    /// <inheritdoc cref="IMonitorsInfiniFrameWindowFeature.GetMainMonitorScreenDpi" />
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
