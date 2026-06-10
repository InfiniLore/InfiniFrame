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

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Point Location => NativeInvoke.InvokeSyncWithValidation(
        logger,
        window.InstanceHandle, 
        window.ManagedThreadId,
        (IntPtr handle, out Point value) => {
            InfiniFrameNativeInteropStatus status = InfiniFrameNative.GetPosition(handle, out int left, out int top);
            value = new Point(left, top);
            return status;
        }
    );
    
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int Top => NativeInvoke.InvokeSyncWithValidation(
        logger,
        window.InstanceHandle, 
        window.ManagedThreadId,
        (IntPtr handle, out int value) => InfiniFrameNative.GetPosition(handle, out _, out value)
    );

    
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int Left => NativeInvoke.InvokeSyncWithValidation(
        logger,
        window.InstanceHandle, 
        window.ManagedThreadId,
        (IntPtr handle, out int value) => InfiniFrameNative.GetPosition(handle, out value, out _)
    );
    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    
    public IInfiniFrameWindow SetLocation(int left, int top) {
        logger.LogDebug(".SetLocation({left}, {right})", left, top);
        window.Invoke(() => {
            InfiniFrameNative.GetPosition(window.InstanceHandle, out int oldLeft, out int oldTop);
            if (oldLeft == left && oldTop == top) return;

            InfiniFrameNative.SetPosition(window.InstanceHandle, left, top);
        });

        return window;
    }
    
    public IInfiniFrameWindow SetLocation(Point location)
        => SetLocation(location.X, location.Y);
    
    public IInfiniFrameWindow SetLeft(int left) {
        logger.LogDebug(".SetLeft({Left})", left);

        window.Invoke(() => {
            InfiniFrameNative.GetPosition(window.InstanceHandle, out int oldLeft, out int top);
            if (left == oldLeft) return;

            InfiniFrameNative.SetPosition(window.InstanceHandle, left, top);
        });

        return window;
    }    
    public IInfiniFrameWindow SetTop(int top) {
        logger.LogDebug(".SetTop({Top})", top);
        window.Invoke(() => {
            InfiniFrameNative.GetPosition(window.InstanceHandle, out int left, out int oldTop);
            if (top == oldTop) return;

            InfiniFrameNative.SetPosition(window.InstanceHandle, left, top);
        });

        return window;
    }
    
    public IInfiniFrameWindow SetTopMost(bool topMost) {
        logger.LogDebug(".SetTopMost({TopMost})", topMost);
        window.Invoke(() => InfiniFrameNative.SetTopmost(window.InstanceHandle, topMost));
        return window;
    }
    
    public IInfiniFrameWindow Offset(int left, int top) {
        logger.LogDebug(".Offset({left}, {top})", left, top);
        window.Invoke(() => {
            InfiniFrameNative.GetPosition(window.InstanceHandle, out int oldLeft, out int oldTop);
            InfiniFrameNative.SetPosition(window.InstanceHandle, oldLeft + left, oldTop + top);
        });
        return window;
    }
    
    public IInfiniFrameWindow Offset(Point offset)
        => Offset(offset.X, offset.Y);
    
    public IInfiniFrameWindow Offset(double left, double top)
        => Offset((int)left, (int)top);
    
    public IInfiniFrameWindow Center() {
        logger.LogDebug(".Center()");
        window.Invoke(() => InfiniFrameNative.Center(window.InstanceHandle));
        return window;
    }
    
    public IInfiniFrameWindow CenterOnCurrentMonitor() {
        window.Invoke(() => {
            MonitorsUtility.GetMonitors(window.InstanceHandle, out ImmutableArray<InfiniMonitor> monitors);
            InfiniFrameNative.GetWindowRectangle(window.InstanceHandle, out Rectangle rectangle);

            // TODO think about proper unhappy flow here
            if (!MonitorsUtility.TryGetCurrentMonitor(monitors, rectangle, out InfiniMonitor monitor)) return;

            Rectangle area = monitor.MonitorArea;

            var newLocation = new Point(area.X + area.Width / 2 - rectangle.Width / 2, area.Y + area.Height / 2 - rectangle.Height / 2);
            InfiniFrameNative.SetPosition(window.InstanceHandle, newLocation.X, newLocation.Y);
        });

        return window;
    }
    
    public IInfiniFrameWindow CenterOnMonitor(int monitorIndex) {
        window.Invoke(() => {
            MonitorsUtility.GetMonitors(window.InstanceHandle, out ImmutableArray<InfiniMonitor> monitors);

            if (monitorIndex < 0 || monitorIndex >= monitors.Length) {
                logger.LogWarning("Monitor index {MonitorIndex} is out of range. Available monitors: {Monitors}", monitorIndex, monitors.Length);
                return;
            }

            InfiniFrameNative.GetSize(window.InstanceHandle, out Size size);
            Rectangle area = monitors[monitorIndex].MonitorArea;

            var newLocation = new Point(area.X + area.Width / 2 - size.Width / 2, area.Y + area.Height / 2 - size.Height / 2);
            InfiniFrameNative.SetPosition(window.InstanceHandle, newLocation.X, newLocation.Y);
        });

        return window;
    }
    
    public IInfiniFrameWindow MoveWithinCurrentMonitorArea(int left, int top) {
        window.Invoke(() => {
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
                var workArea = window.Features.Monitors.GetMainMonitor().WorkArea.Size;
                top = top >= 0
                    ? top - workArea.Height
                    : top;
            }

            InfiniFrameNative.SetPosition(window.InstanceHandle, left, top);
        });
        return window;
    }
    
    public IInfiniFrameWindow MoveWithinCurrentMonitorArea(Point location)
        => MoveWithinCurrentMonitorArea(location.X, location.Y);
    
    public IInfiniFrameWindow MoveWithinCurrentMonitorArea(double left, double top)
        => MoveWithinCurrentMonitorArea((int)left, (int)top);


}
