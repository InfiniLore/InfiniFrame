// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections.Immutable;
using System.Diagnostics;
using System.Drawing;
using InfiniFrame.NativeBridge;
using InfiniFrame.Utilities;
using Microsoft.Extensions.Logging;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Runtime feature implementation for querying and modifying window state including full screen, maximize,
///     minimize, topmost, focus, and zoom.
/// </summary>
public class StateInfiniFrameWindowFeature(
    IInfiniFrameWindow window,
    ILogger<StateInfiniFrameWindowFeature> logger
) : IStateInfiniFrameWindowFeature {

    /// <inheritdoc cref="IStateInfiniFrameWindowFeature.IsFullScreen" />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsFullScreen => NativeInvoke.InvokeSyncWithValidation<bool>(
        logger,
        window,
        window.ManagedThreadId,
        InfiniFrameNative.GetFullScreen
    );

    /// <inheritdoc cref="IStateInfiniFrameWindowFeature.IsMaximized" />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsMaximized => NativeInvoke.InvokeSyncWithValidation<bool>(
        logger,
        window,
        window.ManagedThreadId,
        InfiniFrameNative.GetMaximized
    );

    /// <inheritdoc cref="IStateInfiniFrameWindowFeature.IsMinimized" />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsMinimized => NativeInvoke.InvokeSyncWithValidation<bool>(
        logger,
        window,
        window.ManagedThreadId,
        InfiniFrameNative.GetMinimized
    );

    /// <inheritdoc cref="IStateInfiniFrameWindowFeature.IsTopMost" />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsTopMost => NativeInvoke.InvokeSyncWithValidation<bool>(
        logger,
        window,
        window.ManagedThreadId,
        InfiniFrameNative.GetTopmost
    );

    /// <inheritdoc cref="IStateInfiniFrameWindowFeature.IsFocused" />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsFocused => NativeInvoke.InvokeSyncWithValidation<bool>(
        logger,
        window,
        window.ManagedThreadId,
        InfiniFrameNative.GetFocused
    );
    /// <inheritdoc cref="IStateInfiniFrameWindowFeature.ZoomFactor" />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int ZoomFactor => NativeInvoke.InvokeSyncWithValidation<int>(
        logger,
        window,
        window.ManagedThreadId,
        InfiniFrameNative.GetZoom
    );

    /// <inheritdoc cref="IStateInfiniFrameWindowFeature.IsZoomEnabled" />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsZoomEnabled => NativeInvoke.InvokeSyncWithValidation<bool>(
        logger,
        window,
        window.ManagedThreadId,
        InfiniFrameNative.GetZoomEnabled
    );


    /// <inheritdoc cref="IStateInfiniFrameWindowFeature.CachedPreFullScreenBounds" />
    public Rectangle CachedPreFullScreenBounds { get; set; } = Rectangle.Empty;
    /// <inheritdoc cref="IStateInfiniFrameWindowFeature.CachedPreMaximizedBounds" />
    public Rectangle CachedPreMaximizedBounds { get; set; } = Rectangle.Empty;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="IStateInfiniFrameWindowFeature.SetMaximized" />
    public void SetMaximized(bool maximized = true) {
        logger.LogDebug(".SetMaximized({Maximized})", maximized);
        if (!window.Features.Decorations.IsChromeless) {
            NativeInvoke.InvokeSyncWithValidation(
                logger,
                window,
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
                window,
                window.ManagedThreadId,
                InfiniFrameNative.SetPosition,
                workArea.Left,
                workArea.Top
            );
            NativeInvoke.InvokeSyncWithValidation(
                logger,
                window,
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
                window,
                window.ManagedThreadId,
                InfiniFrameNative.SetPosition,
                oldRect.Left,
                oldRect.Top
            );
            NativeInvoke.InvokeSyncWithValidation(
                logger,
                window,
                window.ManagedThreadId,
                InfiniFrameNative.SetSize,
                oldRect.Width,
                oldRect.Height
            );
            CachedPreMaximizedBounds = Rectangle.Empty;
            window.Events.OnRestored();
        }
    }

    /// <inheritdoc cref="IStateInfiniFrameWindowFeature.ToggleMaximized" />
    public void ToggleMaximized() {
        logger.LogDebug(".ToggleMaximized()");
        bool maximized = NativeInvoke.InvokeSyncWithValidation<bool>(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.GetMaximized
        );
        if (!window.Features.Decorations.IsChromeless) {
            NativeInvoke.InvokeSyncWithValidation(
                logger,
                window,
                window.ManagedThreadId,
                InfiniFrameNative.SetMaximized,
                !maximized
            );
            return;
        }

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
                window,
                window.ManagedThreadId,
                InfiniFrameNative.SetPosition,
                workArea.X,
                workArea.Y
            );
            NativeInvoke.InvokeSyncWithValidation(
                logger,
                window,
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
                window,
                window.ManagedThreadId,
                InfiniFrameNative.SetPosition,
                oldRect.X,
                oldRect.Y
            );
            NativeInvoke.InvokeSyncWithValidation(
                logger,
                window,
                window.ManagedThreadId,
                InfiniFrameNative.SetSize,
                oldRect.Width,
                oldRect.Height
            );
            CachedPreMaximizedBounds = Rectangle.Empty;
            window.Events.OnRestored();
        }
    }

    /// <inheritdoc cref="IStateInfiniFrameWindowFeature.SetMinimized" />
    public void SetMinimized(bool minimized = true) {
        logger.LogDebug(".SetMinimized({Minimized})", minimized);
        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.SetMinimized,
            minimized
        );
    }

    /// <inheritdoc cref="IStateInfiniFrameWindowFeature.SetFullScreen" />
    public void SetFullScreen(bool fullScreen = true) {
        logger.LogDebug(".SetFullScreen({FullScreen})", fullScreen);
        // AppKit owns the asynchronous fullscreen transition and restores the original
        // frame when leaving fullscreen. Moving or resizing during that animation races
        // the Space transition and can leave the window on the wrong display. Always pass
        // the requested state through because native code also reconciles reversals that
        // arrive while an earlier transition is still in progress.
        if (OperatingSystem.IsMacOS()) {
            NativeInvoke.InvokeSyncWithValidation(
                logger,
                window,
                window.ManagedThreadId,
                InfiniFrameNative.SetFullScreen,
                fullScreen
            );
            return;
        }

        if (IsFullScreen == fullScreen) {
            logger.LogDebug("Window is already of the same fullscreen state of {fullscreen}", fullScreen);
            return;
        }

        if (fullScreen) {
            ImmutableArray<InfiniMonitor> monitors = MonitorsUtility.GetMonitors(window);

            (int left, int top) = NativeInvoke.InvokeSyncWithValidation<int, int>(
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

            CachedPreFullScreenBounds = new Rectangle(left, top, width, height);
            if (!MonitorsUtility.TryGetCurrentMonitor(monitors, CachedPreFullScreenBounds, out InfiniMonitor currentMonitor)) {
                logger.LogError("Failed to get current monitor, defaulting to simple fullscreen call");
                NativeInvoke.InvokeSyncWithValidation(
                    logger,
                    window,
                    window.ManagedThreadId,
                    InfiniFrameNative.SetFullScreen,
                    true
                );
            }
            else {
                Rectangle currentMonitorArea = currentMonitor.MonitorArea;
                NativeInvoke.InvokeSyncWithValidation(
                    logger,
                    window,
                    window.ManagedThreadId,
                    InfiniFrameNative.SetFullScreen,
                    true
                );
                NativeInvoke.InvokeSyncWithValidation(
                    logger,
                    window,
                    window.ManagedThreadId,
                    InfiniFrameNative.SetPosition,
                    currentMonitorArea.X,
                    currentMonitorArea.Y
                );
                NativeInvoke.InvokeSyncWithValidation(
                    logger,
                    window,
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
            window,
            window.ManagedThreadId,
            InfiniFrameNative.SetFullScreen,
            false
        );
        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.SetPosition,
            CachedPreFullScreenBounds.X,
            CachedPreFullScreenBounds.Y
        );
        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.SetSize,
            CachedPreFullScreenBounds.Width,
            CachedPreFullScreenBounds.Height
        );
    }

    /// <inheritdoc cref="IStateInfiniFrameWindowFeature.SetFocused" />
    public void SetFocused() {
        if (window.Features.Lifecycle.IsClosedOrClosing()) return;

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.SetFocused
        );
    }

    /// <inheritdoc cref="IStateInfiniFrameWindowFeature.SetZoomFactor" />
    public void SetZoomFactor(int zoom) {
        if (window.Features.Lifecycle.IsClosedOrClosing()) return;

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.SetZoom,
            zoom
        );

    }

    /// <inheritdoc cref="IStateInfiniFrameWindowFeature.EnableZoom" />
    public void EnableZoom(bool zoomEnabled = true) {
        if (window.Features.Lifecycle.IsClosedOrClosing()) return;

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.SetZoomEnabled,
            zoomEnabled
        );

    }

    /// <inheritdoc cref="IStateInfiniFrameWindowFeature.SetTopMost" />
    public void SetTopMost(bool topMost = true) {
        logger.LogDebug(".SetTopMost({TopMost})", topMost);

        NativeInvoke.InvokeSyncWithValidation(
            logger,
            window,
            window.ManagedThreadId,
            InfiniFrameNative.SetTopmost,
            topMost
        );
    }
}
