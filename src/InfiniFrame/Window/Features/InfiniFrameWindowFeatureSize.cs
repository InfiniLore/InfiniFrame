// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge;
using InfiniFrame.Utilities;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowFeatureSize(
    IInfiniFrameWindow window,
    ILogger<InfiniFrameWindowFeatureSize> logger
) : IInfiniFrameWindowFeatureSize {
    public IInfiniFrameWindow SetSize(int width, int height) {
        logger.LogDebug(".SetSize({Width}, {Height})", width, height);

        window.Invoke(() => InfiniFrameNative.SetSize(window.InstanceHandle, width, height));
        return window;
    }
    
    public IInfiniFrameWindow SetSize(Size size)
        => SetSize(size.Width, size.Height);
    
    public IInfiniFrameWindow SetHeight(int height) {
        logger.LogDebug(".SetHeight({Height})", height);

        window.Invoke(() => {
            InfiniFrameNative.GetSize(window.InstanceHandle, out int width, out _);
            InfiniFrameNative.SetSize(window.InstanceHandle, width, height);
        });

        return window;
    }
    
    public IInfiniFrameWindow SetMaximized(bool maximized) {
        logger.LogDebug(".SetMaximized({Maximized})", maximized);
        window.Invoke(() => {
            if (!window.Chromeless) {
                InfiniFrameNative.SetMaximized(window.InstanceHandle, maximized);
                return;
            }

            if (!MonitorsUtility.TryGetCurrentWindowAndMonitor(window, out Rectangle windowRect, out InfiniMonitor monitor)) {
                logger.LogWarning("Monitor {Monitor} not found", monitor);
                return;
            }

            Rectangle workArea = monitor.WorkArea;
            if (maximized) {
                window.CachedPreMaximizedBounds = windowRect;
                InfiniFrameNative.SetPosition(window.InstanceHandle, workArea.Left, workArea.Top);
                InfiniFrameNative.SetSize(window.InstanceHandle, workArea.Width, workArea.Height);
                window.Events.OnMaximized();
            }

            else if (window.CachedPreMaximizedBounds != Rectangle.Empty) {
                Rectangle oldRect = window.CachedPreMaximizedBounds;
                InfiniFrameNative.SetPosition(window.InstanceHandle, oldRect.Left, oldRect.Top);
                InfiniFrameNative.SetSize(window.InstanceHandle, oldRect.Width, oldRect.Height);
                window.CachedPreMaximizedBounds = Rectangle.Empty;
                window.Events.OnRestored();
            }
        });
        return window;
    }
    
    public IInfiniFrameWindow ToggleMaximized() {
        logger.LogDebug(".ToggleMaximized()");
        window.Invoke(() => {
            InfiniFrameNative.GetMaximized(window.InstanceHandle, out bool maximized);
            if (!window.Chromeless) {
                InfiniFrameNative.SetMaximized(window.InstanceHandle, !maximized);
                return;
            }

            // TODO test on other OS?
            // If the window is chromeless then we need to manually register the maximize size else it will just fullscreen
            if (!MonitorsUtility.TryGetCurrentWindowAndMonitor(window, out Rectangle windowRect, out InfiniMonitor monitor)) {
                logger.LogWarning("Monitor {Monitor} not found", monitor);
                return;
            }

            Rectangle workArea = monitor.WorkArea;
            if (window.CachedPreMaximizedBounds == Rectangle.Empty) {
                window.CachedPreMaximizedBounds = windowRect;
                InfiniFrameNative.SetPosition(window.InstanceHandle, workArea.Left, workArea.Top);
                InfiniFrameNative.SetSize(window.InstanceHandle, workArea.Width, workArea.Height);
                window.Events.OnMaximized();
            }
            else {
                Rectangle oldRect = window.CachedPreMaximizedBounds;
                InfiniFrameNative.SetPosition(window.InstanceHandle, oldRect.Left, oldRect.Top);
                InfiniFrameNative.SetSize(window.InstanceHandle, oldRect.Width, oldRect.Height);
                window.CachedPreMaximizedBounds = Rectangle.Empty;
                window.Events.OnRestored();
            }
        });
        return window;
    }
    
    public IInfiniFrameWindow SetMaxSize(int maxWidth, int maxHeight) {
        logger.LogDebug(".SetMaxSize({MaxWidth}, {MaxHeight})", maxWidth, maxHeight);
        window.Invoke(() => InfiniFrameNative.SetMaxSize(window.InstanceHandle, maxWidth, maxHeight));
        return window;
    }
    
    public IInfiniFrameWindow SetMaxSize(Size size)
        => SetMaxSize(size.Width, size.Height);
    
    public IInfiniFrameWindow SetMaxHeight(int maxHeight)
        => SetMaxSize(window.MaxWidth, maxHeight);
    
    public IInfiniFrameWindow SetMaxWidth(int maxWidth)
        => SetMaxSize(maxWidth, window.MaxHeight);
    
    public IInfiniFrameWindow SetMinimized(bool minimized) {
        logger.LogDebug(".SetMinimized({Minimized})", minimized);
        window.Invoke(() => InfiniFrameNative.SetMinimized(window.InstanceHandle, minimized));
        return window;
    }
    
    public IInfiniFrameWindow SetMinSize(int minWidth, int minHeight) {
        logger.LogDebug(".SetMinSize({MinWidth}, {MinHeight})", minWidth, minHeight);
        window.Invoke(() => InfiniFrameNative.SetMinSize(window.InstanceHandle, minWidth, minHeight));
        return window;
    }
    
    public IInfiniFrameWindow SetMinSize(Size size)
        => SetMinSize(size.Width, size.Height);
    
    public IInfiniFrameWindow SetMinHeight(int minHeight)
        => SetMinSize(window.MinWidth, minHeight);
    
    public IInfiniFrameWindow SetMinWidth(int minWidth)
        => SetMinSize(minWidth, window.MinHeight);
    
    public IInfiniFrameWindow SetFullScreen(bool fullScreen) {
        logger.LogDebug(".SetFullScreen({FullScreen})", fullScreen);
        if (window.FullScreen == fullScreen) {
            logger.LogDebug("Window is already of the same fullscreen state of {fullscreen}", fullScreen);
            return window;
        }

        if (fullScreen) {
            window.Invoke(()
                => {
                MonitorsUtility.GetMonitors(window.InstanceHandle, out ImmutableArray<InfiniMonitor> monitors);
                InfiniFrameNative.GetPosition(window.InstanceHandle, out int left, out int top);
                InfiniFrameNative.GetSize(window.InstanceHandle, out int width, out int height);

                window.CachedPreFullScreenBounds = new Rectangle(left, top, width, height);
                if (!MonitorsUtility.TryGetCurrentMonitor(monitors, window.CachedPreFullScreenBounds, out InfiniMonitor currentMonitor)) {
                    logger.LogError("Failed to get current monitor, defaulting to simple fullscreen call");
                    InfiniFrameNative.SetFullScreen(window.InstanceHandle, true);
                    return;
                }

                Rectangle currentMonitorArea = currentMonitor.MonitorArea;

                InfiniFrameNative.SetFullScreen(window.InstanceHandle, true);
                InfiniFrameNative.SetPosition(window.InstanceHandle, currentMonitorArea.X, currentMonitorArea.Y);
                InfiniFrameNative.SetSize(window.InstanceHandle, currentMonitorArea.Width, currentMonitorArea.Height);
            });

            return window;
        }

        // Set Fullscreen to false => Restore to previous state
        window.Invoke(() => {
            InfiniFrameNative.SetFullScreen(window.InstanceHandle, false);
            InfiniFrameNative.SetPosition(window.InstanceHandle, window.CachedPreFullScreenBounds.X, window.CachedPreFullScreenBounds.Y);
            InfiniFrameNative.SetSize(window.InstanceHandle, window.CachedPreFullScreenBounds.Width, window.CachedPreFullScreenBounds.Height);
        });

        return window;
    }
    
    public IInfiniFrameWindow SetWidth(int width) {
        logger.LogDebug(".SetWidth({Width})", width);

        window.Invoke(() => {
            InfiniFrameNative.GetSize(window.InstanceHandle, out _, out int height);
            InfiniFrameNative.SetSize(window.InstanceHandle, width, height);
        });

        return window;
    }
    
    public IInfiniFrameWindow Resize(int widthOffset, int heightOffset, ResizeOrigin origin) {
        window.Invoke(() => {
            InfiniFrameNative.GetSize(window.InstanceHandle, out int width, out int height);
            InfiniFrameNative.GetPosition(window.InstanceHandle, out int originalX, out int originalY);

            int x = originalX;
            int y = originalY;
            switch (origin) {
                case ResizeOrigin.TopLeft: {
                    x += widthOffset;
                    y += heightOffset;
                    width -= widthOffset;
                    height -= heightOffset;
                    break;
                }

                case ResizeOrigin.Top: {
                    y += heightOffset;
                    height -= heightOffset;
                    break;
                }

                case ResizeOrigin.TopRight: {
                    y += heightOffset;
                    width += widthOffset;
                    height -= heightOffset;
                    break;
                }

                case ResizeOrigin.Right: {
                    width += widthOffset;
                    break;
                }

                case ResizeOrigin.BottomRight: {
                    width += widthOffset;
                    height += heightOffset;
                    break;
                }

                case ResizeOrigin.Bottom: {
                    height += heightOffset;
                    break;
                }

                case ResizeOrigin.BottomLeft: {
                    x += widthOffset;
                    width -= widthOffset;
                    height += heightOffset;
                    break;
                }

                case ResizeOrigin.Left: {
                    x += widthOffset;
                    width -= widthOffset;
                    break;
                }

                default: throw new ArgumentOutOfRangeException(nameof(origin), origin, null);
            }

            // Clamping between min and max size
            Size max = window.MaxSize;
            Size min = window.MinSize;

            if (width >= max.Width) {
                width = max.Width;
                x = originalX;
            }

            if (height >= max.Height) {
                height = max.Height;
                y = originalY;
            }

            if (width <= min.Width) {
                width = min.Width;
                x = originalX;
            }

            if (height <= min.Height) {
                height = min.Height;
                y = originalY;
            }

            InfiniFrameNative.SetSize(window.InstanceHandle, width, height);
            InfiniFrameNative.SetPosition(window.InstanceHandle, x, y);

        });
        return window;
    }

}
