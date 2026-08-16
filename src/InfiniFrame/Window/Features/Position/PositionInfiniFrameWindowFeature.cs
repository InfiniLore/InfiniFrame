// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge;
using InfiniFrame.Utilities;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class PositionInfiniFrameWindowFeature(
    IInfiniFrameWindow window,
    ILogger<PositionInfiniFrameWindowFeature> logger
) : IPositionInfiniFrameWindowFeature {

    /// <inheritdoc cref="IPositionInfiniFrameWindowFeature.Location" />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Point Location => NativeInvoke.InvokeSyncWithValidation(
        logger,
        window,
        window.ManagedThreadId,
        callback: (IntPtr handle, out Point value) => {
            InfiniFrameNativeInteropStatus status = InfiniFrameNative.GetPosition(handle, out int left, out int top);
            value = new Point(left, top);
            return status;
        }
    );

    /// <inheritdoc cref="IPositionInfiniFrameWindowFeature.Top" />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int Top => NativeInvoke.InvokeSyncWithValidation(
        logger,
        window,
        window.ManagedThreadId,
        callback: (IntPtr handle, out int value) => InfiniFrameNative.GetPosition(handle, out _, out value)
    );


    /// <inheritdoc cref="IPositionInfiniFrameWindowFeature.Left" />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int Left => NativeInvoke.InvokeSyncWithValidation(
        logger,
        window,
        window.ManagedThreadId,
        callback: (IntPtr handle, out int value) => InfiniFrameNative.GetPosition(handle, out value, out _)
    );

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------

    /// <inheritdoc cref="IPositionInfiniFrameWindowFeature.SetLocation(int, int)" />
    public void SetLocation(int left, int top) {
        logger.LogDebug(".SetLocation({left}, {right})", left, top);

        (int oldLeft, int oldTop) = NativeInvoke.InvokeSyncWithValidation<int, int>(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.GetPosition
        );
        if (oldLeft == left && oldTop == top) return;

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.SetPosition,
            left,
            top
        );
    }

    /// <inheritdoc cref="IPositionInfiniFrameWindowFeature.SetLocation(Point)" />
    public void SetLocation(Point location)
        => SetLocation(location.X, location.Y);

    /// <inheritdoc cref="IPositionInfiniFrameWindowFeature.SetLeft" />
    public void SetLeft(int left) {
        logger.LogDebug(".SetLeft({Left})", left);

        (int oldLeft, int top) = NativeInvoke.InvokeSyncWithValidation<int, int>(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.GetPosition
        );
        if (oldLeft == left) return;

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.SetPosition,
            left,
            top
        );

    }
    /// <inheritdoc cref="IPositionInfiniFrameWindowFeature.SetTop" />
    public void SetTop(int top) {
        logger.LogDebug(".SetTop({Top})", top);

        (int left, int oldTop) = NativeInvoke.InvokeSyncWithValidation<int, int>(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.GetPosition
        );
        if (oldTop == top) return;

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.SetPosition,
            left,
            top
        );
    }

    /// <inheritdoc cref="IPositionInfiniFrameWindowFeature.Offset(int, int)" />
    public void Offset(int left, int top) {
        logger.LogDebug(".Offset({left}, {top})", left, top);

        (int oldLeft, int oldTop) = NativeInvoke.InvokeSyncWithValidation<int, int>(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.GetPosition
        );

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.SetPosition,
            oldLeft + left,
            oldTop + top
        );
    }

    /// <inheritdoc cref="IPositionInfiniFrameWindowFeature.Offset(Point)" />
    public void Offset(Point offset)
        => Offset(offset.X, offset.Y);

    /// <inheritdoc cref="IPositionInfiniFrameWindowFeature.Offset(double, double)" />
    public void Offset(double left, double top)
        => Offset((int)left, (int)top);

    /// <inheritdoc cref="IPositionInfiniFrameWindowFeature.Center" />
    public void Center() {
        logger.LogDebug(".Center()");
        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.Center
        );
    }

    /// <inheritdoc cref="IPositionInfiniFrameWindowFeature.CenterOnCurrentMonitor" />
    public void CenterOnCurrentMonitor() {
        ImmutableArray<InfiniMonitor> monitors = MonitorsUtility.GetMonitors(window);
        (int x, int y) = NativeInvoke.InvokeSyncWithValidation<int, int>(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.GetPosition
        );

        (int width, int height) = NativeInvoke.InvokeSyncWithValidation<int, int>(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.GetSize
        );

        if (!MonitorsUtility.TryGetCurrentMonitor(monitors, new Rectangle(x, y, width, height), out InfiniMonitor monitor)) {
            logger.LogWarning("Could not determine monitor for window at ({X}, {Y}) - skipping centering.", x, y);
            return;
        }

        Rectangle area = monitor.MonitorArea;
        Point newLocation = PositionCalculations.ComputeCenter(area, width, height);

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.SetPosition,
            newLocation.X,
            newLocation.Y
        );
    }

    /// <inheritdoc cref="IPositionInfiniFrameWindowFeature.CenterOnMonitor" />
    public void CenterOnMonitor(int monitorIndex) {
        ImmutableArray<InfiniMonitor> monitors = MonitorsUtility.GetMonitors(window);

        if (monitorIndex < 0 || monitorIndex >= monitors.Length) {
            logger.LogWarning("Monitor index {MonitorIndex} is out of range. Available monitors: {Monitors}", monitorIndex, monitors.Length);
            return;
        }

        (int width, int height) = NativeInvoke.InvokeSyncWithValidation<int, int>(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.GetSize
        );
        Rectangle area = monitors[monitorIndex].MonitorArea;
        Point newLocation = PositionCalculations.ComputeCenter(area, width, height);
        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.SetPosition,
            newLocation.X,
            newLocation.Y
        );
    }

    /// <inheritdoc cref="IPositionInfiniFrameWindowFeature.MoveWithinCurrentMonitorArea(int, int)" />
    public void MoveWithinCurrentMonitorArea(int left, int top) {
        MonitorsUtility.TryGetCurrentWindowAndMonitor(window, out Rectangle windowRect, out InfiniMonitor monitor);

        (left, top) = PositionCalculations.ClampToMonitorArea(
            left, top, windowRect.Width, windowRect.Height, monitor.WorkArea
        );

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.SetPosition,
            left,
            top
        );
    }

    /// <inheritdoc cref="IPositionInfiniFrameWindowFeature.MoveWithinCurrentMonitorArea(Point)" />
    public void MoveWithinCurrentMonitorArea(Point location)
        => MoveWithinCurrentMonitorArea(location.X, location.Y);

    /// <inheritdoc cref="IPositionInfiniFrameWindowFeature.MoveWithinCurrentMonitorArea(double, double)" />
    public void MoveWithinCurrentMonitorArea(double left, double top)
        => MoveWithinCurrentMonitorArea((int)left, (int)top);


}