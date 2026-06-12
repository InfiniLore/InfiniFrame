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
public class InfiniFrameWindowFeatureState(
    IInfiniFrameWindow window,
    ILogger<InfiniFrameWindowFeatureState> logger
) : IInfiniFrameWindowFeatureState {

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsFullScreen => NativeInvoke.InvokeSyncWithValidation<bool>(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        InfiniFrameNative.GetFullScreen
    );

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsMaximized => NativeInvoke.InvokeSyncWithValidation<bool>(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        InfiniFrameNative.GetMaximized
    );

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsMinimized => NativeInvoke.InvokeSyncWithValidation<bool>(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        InfiniFrameNative.GetMinimized
    );

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsTopMost => NativeInvoke.InvokeSyncWithValidation<bool>(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        InfiniFrameNative.GetTopmost
    );

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsFocused => NativeInvoke.InvokeSyncWithValidation<bool>(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        InfiniFrameNative.GetFocused
    );
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int ZoomFactor => NativeInvoke.InvokeSyncWithValidation<int>(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        InfiniFrameNative.GetZoom
    );

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsZoomEnabled => NativeInvoke.InvokeSyncWithValidation<bool>(
        logger,
        window.InstanceHandle,
        window.ManagedThreadId,
        InfiniFrameNative.GetZoomEnabled
    );


    public Rectangle CachedPreFullScreenBounds { get; set; } = Rectangle.Empty;
    public Rectangle CachedPreMaximizedBounds { get; set; } = Rectangle.Empty;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public void SetMaximized(bool maximized = true) {
        logger.LogDebug(".SetMaximized({Maximized})", maximized);
        if (!window.Features.Decorations.IsChromeless) {
            NativeInvoke.InvokeSyncWithValidation(
                logger,
                window.InstanceHandle,
                window.ManagedThreadId,
                InfiniFrameNative.SetMaximized,
                maximized
            );
            return;
        }

        if (!MonitorsUtility.TryGetCurrentWindowAndMonitor(window, out Rectangle windowRect, out InfiniMonitor monitor)) {
            logger.LogWarning("Monitor {Monitor} not found", monitor);
            return;
        }

        Rectangle workArea = monitor.WorkArea;
        if (maximized) {
            CachedPreMaximizedBounds = windowRect;
            NativeInvoke.InvokeSyncWithValidation(
                logger,
                window.InstanceHandle,
                window.ManagedThreadId,
                InfiniFrameNative.SetPosition,
                workArea.Left,
                workArea.Top
            );
            NativeInvoke.InvokeSyncWithValidation(
                logger,
                window.InstanceHandle,
                window.ManagedThreadId,
                InfiniFrameNative.SetSize,
                workArea.Width,
                workArea.Height
            );
            window.Events.OnMaximized();
        }

        else if (CachedPreMaximizedBounds != Rectangle.Empty) {
            Rectangle oldRect = CachedPreMaximizedBounds;
            NativeInvoke.InvokeSyncWithValidation(
                logger,
                window.InstanceHandle,
                window.ManagedThreadId,
                InfiniFrameNative.SetPosition,
                oldRect.Left,
                oldRect.Top
            );
            NativeInvoke.InvokeSyncWithValidation(
                logger,
                window.InstanceHandle,
                window.ManagedThreadId,
                InfiniFrameNative.SetSize,
                oldRect.Width,
                oldRect.Height
            );
            CachedPreMaximizedBounds = Rectangle.Empty;
            window.Events.OnRestored();
        }
    }

    public void ToggleMaximized() {
        logger.LogDebug(".ToggleMaximized()");
        bool maximized = NativeInvoke.InvokeSyncWithValidation<bool>(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.GetMaximized
        );
        if (!window.Features.Decorations.IsChromeless) {
            NativeInvoke.InvokeSyncWithValidation(
                logger,
                window.InstanceHandle,
                window.ManagedThreadId,
                InfiniFrameNative.SetMaximized,
                !maximized
            );
            return;
        }

        // TODO test on other OS?
        // If the window is chromeless then we need to manually register the maximize size else it will just fullscreen
        if (!MonitorsUtility.TryGetCurrentWindowAndMonitor(window, out Rectangle windowRect, out InfiniMonitor monitor)) {
            logger.LogWarning("Monitor {Monitor} not found", monitor);
            return;
        }

        Rectangle workArea = monitor.WorkArea;
        if (CachedPreMaximizedBounds == Rectangle.Empty) {
            CachedPreMaximizedBounds = windowRect;
            NativeInvoke.InvokeSyncWithValidation(
                logger,
                window.InstanceHandle,
                window.ManagedThreadId,
                InfiniFrameNative.SetPosition,
                workArea.X,
                workArea.Y
            );
            NativeInvoke.InvokeSyncWithValidation(
                logger,
                window.InstanceHandle,
                window.ManagedThreadId,
                InfiniFrameNative.SetSize,
                workArea.Width,
                workArea.Height
            );
            window.Events.OnMaximized();
        }
        else {
            Rectangle oldRect = CachedPreMaximizedBounds;
            NativeInvoke.InvokeSyncWithValidation(
                logger,
                window.InstanceHandle,
                window.ManagedThreadId,
                InfiniFrameNative.SetPosition,
                oldRect.X,
                oldRect.Y
            );
            NativeInvoke.InvokeSyncWithValidation(
                logger,
                window.InstanceHandle,
                window.ManagedThreadId,
                InfiniFrameNative.SetSize,
                oldRect.Width,
                oldRect.Height
            );
            CachedPreMaximizedBounds = Rectangle.Empty;
            window.Events.OnRestored();
        }
    }

    public void SetMinimized(bool minimized = true) {
        logger.LogDebug(".SetMinimized({Minimized})", minimized);
        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.SetMinimized,
            minimized
        );
    }

    public void SetFullScreen(bool fullScreen = true) {
        logger.LogDebug(".SetFullScreen({FullScreen})", fullScreen);
        if (IsFullScreen == fullScreen) {
            logger.LogDebug("Window is already of the same fullscreen state of {fullscreen}", fullScreen);
            return;
        }

        if (fullScreen) {
            ImmutableArray<InfiniMonitor> monitors = MonitorsUtility.GetMonitors(window);

            (int left, int top) = NativeInvoke.InvokeSyncWithValidation<int, int>(
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

            CachedPreFullScreenBounds = new Rectangle(left, top, width, height);
            if (!MonitorsUtility.TryGetCurrentMonitor(monitors, CachedPreFullScreenBounds, out InfiniMonitor currentMonitor)) {
                logger.LogError("Failed to get current monitor, defaulting to simple fullscreen call");
                NativeInvoke.InvokeSyncWithValidation(
                    logger,
                    window.InstanceHandle,
                    window.ManagedThreadId,
                    InfiniFrameNative.SetFullScreen,
                    true
                );
            }
            else {
                Rectangle currentMonitorArea = currentMonitor.MonitorArea;
                NativeInvoke.InvokeSyncWithValidation(
                    logger,
                    window.InstanceHandle,
                    window.ManagedThreadId,
                    InfiniFrameNative.SetFullScreen,
                    true
                );
                NativeInvoke.InvokeSyncWithValidation(
                    logger,
                    window.InstanceHandle,
                    window.ManagedThreadId,
                    InfiniFrameNative.SetPosition,
                    currentMonitorArea.X,
                    currentMonitorArea.Y
                );
                NativeInvoke.InvokeSyncWithValidation(
                    logger,
                    window.InstanceHandle,
                    window.ManagedThreadId,
                    InfiniFrameNative.SetSize,
                    currentMonitorArea.Width,
                    currentMonitorArea.Height
                );
            }

            return;
        }

        // Set Fullscreen to false => Restore to previous state
        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.SetFullScreen,
            false
        );
        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.SetPosition,
            CachedPreFullScreenBounds.X,
            CachedPreFullScreenBounds.Y
        );
        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.SetSize,
            CachedPreFullScreenBounds.Width,
            CachedPreFullScreenBounds.Height
        );
    }

    public void SetFocused() {
        if (window.Features.Lifecycle.IsClosedOrClosing()) return;

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.SetFocused
        );
    }

    public void SetZoom(int zoom) {
        if (window.Features.Lifecycle.IsClosedOrClosing()) return;

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.SetZoom,
            zoom
        );

    }

    public void SetZoomEnabled(bool zoomEnabled = true) {
        if (window.Features.Lifecycle.IsClosedOrClosing()) return;

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.SetZoomEnabled,
            zoomEnabled
        );

    }

    public void SetTopMost(bool topMost = true) {
        logger.LogDebug(".SetTopMost({TopMost})", topMost);

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window.InstanceHandle,
            window.ManagedThreadId,
            InfiniFrameNative.SetTopmost,
            topMost
        );
    }
}
