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
public class InfiniFrameWindowFeaturePosition(
    IInfiniFrameWindow window,
    ILogger<InfiniFrameWindowFeaturePosition> logger
) : IInfiniFrameWindowFeaturePosition {

    /// <inheritdoc cref="IInfiniFrameWindowFeaturePosition.Location"/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Point Location => NativeInvoke.InvokeSyncWithValidation(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        callback: (IntPtr handle, out Point value) => {
            InfiniFrameNativeInteropStatus status = InfiniFrameNative.GetPosition(handle, out int left, out int top);
            value = new Point(left, top);
            return status;
        }
    );

    /// <inheritdoc cref="IInfiniFrameWindowFeaturePosition.Top"/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int Top => NativeInvoke.InvokeSyncWithValidation(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        callback: (IntPtr handle, out int value) => InfiniFrameNative.GetPosition(handle, out _, out value)
    );


    /// <inheritdoc cref="IInfiniFrameWindowFeaturePosition.Left"/>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int Left => NativeInvoke.InvokeSyncWithValidation(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        callback: (IntPtr handle, out int value) => InfiniFrameNative.GetPosition(handle, out value, out _)
    );

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------

    /// <inheritdoc cref="IInfiniFrameWindowFeaturePosition.SetLocation(int, int)"/>
    public void SetLocation(int left, int top) {
        logger.LogDebug(".SetLocation({left}, {right})", left, top);

        (int oldLeft, int oldTop) = NativeInvoke.InvokeSyncWithValidation<int, int>(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.GetPosition
        );
        if (oldLeft == left && oldTop == top) return;

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.SetPosition,
            left,
            top
        );
    }

    /// <inheritdoc cref="IInfiniFrameWindowFeaturePosition.SetLocation(Point)"/>
    public void SetLocation(Point location)
        => SetLocation(location.X, location.Y);

    /// <inheritdoc cref="IInfiniFrameWindowFeaturePosition.SetLeft"/>
    public void SetLeft(int left) {
        logger.LogDebug(".SetLeft({Left})", left);

        (int oldLeft, int top) = NativeInvoke.InvokeSyncWithValidation<int, int>(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.GetPosition
        );
        if (oldLeft == left) return;

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.SetPosition,
            left,
            top
        );

    }
    /// <inheritdoc cref="IInfiniFrameWindowFeaturePosition.SetTop"/>
    public void SetTop(int top) {
        logger.LogDebug(".SetTop({Top})", top);

        (int left, int oldTop) = NativeInvoke.InvokeSyncWithValidation<int, int>(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.GetPosition
        );
        if (oldTop == Top) return;

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.SetPosition,
            left,
            top
        );
    }

    /// <inheritdoc cref="IInfiniFrameWindowFeaturePosition.Offset(int, int)"/>
    public void Offset(int left, int top) {
        logger.LogDebug(".Offset({left}, {top})", left, top);

        (int oldLeft, int oldTop) = NativeInvoke.InvokeSyncWithValidation<int, int>(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.GetPosition
        );

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.SetPosition,
            oldLeft + left,
            oldTop + top
        );
    }

    /// <inheritdoc cref="IInfiniFrameWindowFeaturePosition.Offset(Point)"/>
    public void Offset(Point offset)
        => Offset(offset.X, offset.Y);

    /// <inheritdoc cref="IInfiniFrameWindowFeaturePosition.Offset(double, double)"/>
    public void Offset(double left, double top)
        => Offset((int)left, (int)top);

    /// <inheritdoc cref="IInfiniFrameWindowFeaturePosition.Center"/>
    public void Center() {
        logger.LogDebug(".Center()");
        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.Center
        );
    }

    /// <inheritdoc cref="IInfiniFrameWindowFeaturePosition.CenterOnCurrentMonitor"/>
    public void CenterOnCurrentMonitor() {
        ImmutableArray<InfiniMonitor> monitors = MonitorsUtility.GetMonitors(window);
        (int x, int y) = NativeInvoke.InvokeSyncWithValidation<int, int>(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.GetPosition
        );

        (int width, int height) = NativeInvoke.InvokeSyncWithValidation<int, int>(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.GetSize
        );

        // TODO think about proper unhappy flow here
        if (!MonitorsUtility.TryGetCurrentMonitor(monitors, new Rectangle(x, y, width, height), out InfiniMonitor monitor)) return;

        Rectangle area = monitor.MonitorArea;

        var newLocation = new Point(area.X + area.Width / 2 - width / 2, area.Y + area.Height / 2 - height / 2);

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.SetPosition,
            newLocation.X,
            newLocation.Y
        );
    }

    /// <inheritdoc cref="IInfiniFrameWindowFeaturePosition.CenterOnMonitor"/>
    public void CenterOnMonitor(int monitorIndex) {
        ImmutableArray<InfiniMonitor> monitors = MonitorsUtility.GetMonitors(window);

        if (monitorIndex < 0 || monitorIndex >= monitors.Length) {
            logger.LogWarning("Monitor index {MonitorIndex} is out of range. Available monitors: {Monitors}", monitorIndex, monitors.Length);
            return;
        }

        (int width, int height) = NativeInvoke.InvokeSyncWithValidation<int, int>(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.GetSize
        );
        Rectangle area = monitors[monitorIndex].MonitorArea;

        var newLocation = new Point(area.X + area.Width / 2 - width / 2, area.Y + area.Height / 2 - height / 2);
        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.SetPosition,
            newLocation.X,
            newLocation.Y
        );
    }

    /// <inheritdoc cref="IInfiniFrameWindowFeaturePosition.MoveWithinCurrentMonitorArea(int, int)"/>
    public void MoveWithinCurrentMonitorArea(int left, int top) {
        MonitorsUtility.TryGetCurrentWindowAndMonitor(window, out Rectangle windowRect, out InfiniMonitor monitor);
        int horizontalWindowEdge = left + windowRect.Width;
        int verticalWindowEdge = top + windowRect.Height;

        int leftBound = monitor.WorkArea.X;
        int topBound = monitor.WorkArea.Y;
        int rightBound = monitor.WorkArea.X + monitor.WorkArea.Width;
        int bottomBound = monitor.WorkArea.Y + monitor.WorkArea.Height;

        left = horizontalWindowEdge > rightBound
            ? Math.Max(rightBound - window.Features.Size.Width, leftBound)
            : Math.Max(left, leftBound);
        top = verticalWindowEdge > bottomBound
            ? Math.Max(bottomBound - window.Features.Size.Height, topBound)
            : Math.Max(top, topBound);

        // Bug:
        // For some reason the vertical position is not handled correctly.
        // Whenever a positive value is set, the window appears at the
        // very bottom of the screen, and the only visible thing is the
        // application window title bar. As a workaround we make a
        // negative value out of the vertical position to "pull" the window up.
        // Note:
        // This behavior seems to be a macOS thing. In the InfiniFrame.Native
        // project files it is commented to be expected behavior for macOS.
        // There is some code trying to mitigate this problem, but it might
        // not work as expected. Further investigation is necessary.
        // Update:
        // This behavior seems to have changed with macOS Sonoma.
        // Therefore, we determine the version of macOS and only apply the
        // workaround for older versions.
        if (OperatingSystem.IsMacOS() && OperatingSystem.IsMacOSVersionAtLeast(23)) {
            Size workArea = window.Features.Monitors.GetMainMonitor().WorkArea.Size;
            top = top >= 0
                ? top - workArea.Height
                : top;
        }

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.SetPosition,
            left,
            top
        );
    }

    /// <inheritdoc cref="IInfiniFrameWindowFeaturePosition.MoveWithinCurrentMonitorArea(Point)"/>
    public void MoveWithinCurrentMonitorArea(Point location)
        => MoveWithinCurrentMonitorArea(location.X, location.Y);

    /// <inheritdoc cref="IInfiniFrameWindowFeaturePosition.MoveWithinCurrentMonitorArea(double, double)"/>
    public void MoveWithinCurrentMonitorArea(double left, double top)
        => MoveWithinCurrentMonitorArea((int)left, (int)top);


}
