// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Native;
using InfiniFrame.Utilities;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class InfiniWindowExtensions {
    #region Load
    /// <param name="window">Photino window instance</param>
    extension<T>(T window) where T : class, IInfiniFrameWindow {
        /// <summary>
        ///     Loads specified <see cref="Uri" /> into the browser control.
        /// </summary>
        /// <returns>
        ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
        /// </returns>
        /// <remarks>
        ///     Load() or LoadString() must be called before a native window is initialized.
        /// </remarks>
        /// <param name="uri">A Uri pointing to the file or the URL to load.</param>
        public T Load(Uri uri) {
            window.Logger.LogDebug(".Load({uri})", uri);
            window.Invoke(() => InfiniFrameNative.NavigateToUrl(window.InstanceHandle, uri.ToString()));
            return window;
        }

        /// <summary>
        ///     Loads a specified path into the browser control.
        /// </summary>
        /// <returns>
        ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
        /// </returns>
        /// <remarks>
        ///     Load() or LoadString() must be called before a native window is initialized.
        /// </remarks>
        /// <param name="path">A path pointing to the resource to load.</param>
        public T Load(string path) {
            window.Logger.LogDebug(".Load({Path})", path);

            // TODO patch this
            // ––––––––––––––––––––––
            // SECURITY RISK!
            // This needs validation!
            // ––––––––––––––––––––––
            // Open a web URL string path
            if (path.Contains("http://") || path.Contains("https://"))
                return Load(window, new Uri(path));

            // Open a file resource string path
            string absolutePath = Path.GetFullPath(path);

            // For a bundled app it can be necessary to consider
            // the app context base directory. Check there too.
            if (File.Exists(absolutePath)) return Load(window, new Uri(absolutePath, UriKind.Absolute));

            absolutePath = $"{AppContext.BaseDirectory}/{path}";

            if (File.Exists(absolutePath)) return Load(window, new Uri(absolutePath, UriKind.Absolute));

            window.Logger.LogWarning("File not found: {Path}", absolutePath);
            return window;
        }

        /// <summary>
        ///     Loads a raw string into the browser control.
        /// </summary>
        /// <returns>
        ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
        /// </returns>
        /// <remarks>
        ///     Used to load HTML into the browser control directly.
        ///     Load() or LoadString() must be called before a native window is initialized.
        /// </remarks>
        /// <param name="content">Raw content (such as HTML)</param>
        public T LoadRawString(string content) {
            string shortContent = content.Length > 50 ? string.Concat(content.AsSpan(0, 47), "...") : content;
            window.Logger.LogDebug(".LoadRawString({Content})", shortContent);
            window.Invoke(() => InfiniFrameNative.NavigateToString(window.InstanceHandle, content));
            return window;
        }
        #endregion

        #region Center
        /// <summary>
        ///     Centers the native window on the primary display.
        /// </summary>
        /// <remarks>
        ///     If called prior to window initialization, overrides Left (X) and Top (Y) properties.
        /// </remarks>
        /// <returns>
        ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
        /// </returns>
        public T Center() {
            window.Logger.LogDebug(".Center()");
            window.Invoke(() => InfiniFrameNative.Center(window.InstanceHandle));
            return window;
        }

        /// <summary>
        /// Centers the current window on the monitor where it is currently displayed.
        /// </summary>
        /// <returns>
        /// Returns the current <see cref="IInfiniFrameWindow" /> instance.
        /// </returns>
        /// <remarks>
        /// If the window spans multiple monitors, it will be centered on the monitor containing the largest portion of the window.
        /// </remarks>
        public T CenterOnCurrentMonitor()
            => CenterOnMonitor(window, -1);

        /// <summary>
        /// Centers the specified <see cref="IInfiniFrameWindow" /> on a monitor.
        /// </summary>
        /// <param name="monitorIndex">
        /// The index of the monitor on which the window should be centered.
        /// Use -1 for the current monitor, or specify a zero-based index for a specific monitor.
        /// </param>
        /// <returns>
        /// Returns the current <see cref="IInfiniFrameWindow" /> instance.
        /// </returns>
        public T CenterOnMonitor(int monitorIndex) {
            if (monitorIndex <= -1) {
                window.Invoke(() => {
                    ImmutableArray<Monitor> monitors = MonitorsUtility.GetMonitors(window);
                    InfiniFrameNative.GetWindowRectangle(window.InstanceHandle, out Rectangle rectangle);

                    // TODO think about proper unhappy flow here
                    if (!MonitorsUtility.TryGetCurrentMonitor(monitors, rectangle, out Monitor monitor)) return;

                    Rectangle area = monitor.MonitorArea;

                    var newLocation = new Point(area.X + area.Width / 2 - rectangle.Width / 2, area.Y + area.Height / 2 - rectangle.Height / 2);
                    InfiniFrameNative.SetPosition(window.InstanceHandle, newLocation.X, newLocation.Y);
                });
            }

            window.Invoke(() => {
                ImmutableArray<Monitor> monitors = MonitorsUtility.GetMonitors(window);

                if (monitorIndex < 0 || monitorIndex >= monitors.Length) {
                    window.Logger.LogWarning("Monitor index {MonitorIndex} is out of range. Available monitors: {Monitors}", monitorIndex, monitors.Length);
                    return;
                }

                InfiniFrameNative.GetSize(window.InstanceHandle, out Size size);
                Rectangle area = monitors[monitorIndex].MonitorArea;

                var newLocation = new Point(area.X + area.Width / 2 - size.Width / 2, area.Y + area.Height / 2 - size.Height / 2);
                InfiniFrameNative.SetPosition(window.InstanceHandle, newLocation.X, newLocation.Y);
            });

            return window;
        }
        #endregion

        #region MoveWithinCurrentMonitorArea
        /// <summary>
        ///     Moves the native window to the specified location on the screen in pixels using a Point.
        /// </summary>
        /// <returns>
        ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
        /// </returns>
        /// <param name="left">Position from left in pixels</param>
        /// <param name="top">Position from top in pixels</param>
        public T MoveWithinCurrentMonitorArea(int left, int top) {
            window.Invoke(() => {
                MonitorsUtility.TryGetCurrentWindowAndMonitor(window, out Rectangle windowRect, out Monitor monitor);
                int horizontalWindowEdge = left + windowRect.Width;
                int verticalWindowEdge = top + windowRect.Height;

                int leftBound = monitor.WorkArea.X;
                int topBound = monitor.WorkArea.Y;
                int rightBound = monitor.WorkArea.X + monitor.WorkArea.Width;
                int bottomBound = monitor.WorkArea.Y + monitor.WorkArea.Height;

                left = horizontalWindowEdge > rightBound
                    ? Math.Max(rightBound - window.Width, leftBound)
                    : Math.Max(left, leftBound);
                top = verticalWindowEdge > bottomBound
                    ? Math.Max(bottomBound - window.Height, topBound)
                    : Math.Max(top, topBound);

                // Bug:
                // For some reason the vertical position is not handled correctly.
                // Whenever a positive value is set, the window appears at the
                // very bottom of the screen, and the only visible thing is the
                // application window title bar. As a workaround we make a
                // negative value out of the vertical position to "pull" the window up.
                // Note:
                // This behavior seems to be a macOS thing. In the Photino.Native
                // project files it is commented to be expected behavior for macOS.
                // There is some code trying to mitigate this problem, but it might
                // not work as expected. Further investigation is necessary.
                // Update:
                // This behavior seems to have changed with macOS Sonoma.
                // Therefore, we determine the version of macOS and only apply the
                // workaround for older versions.
                if (OperatingSystem.IsMacOS() && OperatingSystem.IsMacOSVersionAtLeast(23)) {
                    Size workArea = window.MainMonitor.WorkArea.Size;
                    top = top >= 0
                        ? top - workArea.Height
                        : top;
                }

                InfiniFrameNative.SetPosition(window.InstanceHandle, left, top);
            });
            return window;
        }

        /// <summary>
        ///     Moves the native window to the specified location on the screen in pixels
        ///     using <see cref="IInfiniFrameWindow.Left" /> (X) and <see cref="IInfiniFrameWindow.Top" /> (Y) properties.
        /// </summary>
        /// <returns>
        ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
        /// </returns>
        /// <param name="location">Position as <see cref="Point" /></param>
        public T MoveWithinCurrentMonitorArea(Point location)
            => MoveWithinCurrentMonitorArea(window, location.X, location.Y);

        /// <summary>
        /// Moves the native window to the specified location on the screen using a double value.
        /// </summary>
        /// <param name="left">Left Position</param>
        /// <param name="top">Top Position</param>
        /// <returns>
        ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
        /// </returns>
        public T MoveWithinCurrentMonitorArea(double left, double top)
            => MoveWithinCurrentMonitorArea(window, (int)left, (int)top);
        #endregion

        #region Offset
        /// <summary>
        ///     Moves the native window relative to its current location on the screen in pixels
        ///     using <see cref="IInfiniFrameWindow.Left" /> (X) and <see cref="IInfiniFrameWindow.Top" /> (Y) properties.
        /// </summary>
        /// <returns>
        ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
        /// </returns>
        /// <param name="left">Relative offset from left in pixels</param>
        /// <param name="top">Relative offset from top in pixels</param>
        public T Offset(int left, int top) {
            window.Logger.LogDebug(".Offset({left}, {top})", left, top);
            window.Invoke(() => {
                InfiniFrameNative.GetPosition(window.InstanceHandle, out int oldLeft, out int oldTop);
                InfiniFrameNative.SetPosition(window.InstanceHandle, oldLeft + left, oldTop + top);
            });
            return window;
        }

        /// <summary>
        ///     Moves the native window relative to its current location on the screen
        ///     using a <see cref="Point" />.
        /// </summary>
        /// <returns>
        ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
        /// </returns>
        /// <param name="offset">Relative offset</param>
        public T Offset(Point offset)
            => Offset(window, offset.X, offset.Y);

        /// <summary>
        /// Moves the native window relative to its current location on the screen in pixels
        /// using <see cref="IInfiniFrameWindow.Left" /> (X) and <see cref="IInfiniFrameWindow.Top" /> (Y) properties.
        /// </summary>
        /// <returns>
        /// Returns the current <see cref="IInfiniFrameWindow" /> instance.
        /// </returns>
        /// <param name="left">Relative offset from the left in pixels.</param>
        /// <param name="top">Relative offset from the top in pixels.</param>
        public T Offset(double left, double top)
            => Offset(window, (int)left, (int)top);
        #endregion

        #region SetTransparent
        /// <summary>
        ///     When true, the native window can be displayed with a transparent background.
        ///     Chromeless must be set to true. HTML document's body background must have alpha-based value.
        ///     By default, this is set to false.
        /// </summary>
        public T SetTransparent(bool enabled) {
            window.Logger.LogDebug(".SetTransparent({Enabled})", enabled);

            if (OperatingSystem.IsWindows()) {
                window.Logger.LogWarning("Transparent can only be set on Windows before the native window is instantiated.");
                return window;
            }

            window.Logger.LogDebug("Invoking PhotinoNative.SetTransparentEnabled({value})", enabled);
            window.Invoke(() => InfiniFrameNative.SetTransparentEnabled(window.InstanceHandle, enabled));
            return window;
        }
        #endregion

        #region SetContextMenuEnabled
        /// <summary>
        ///     When true, the user can access the browser control's context menu.
        ///     By default, this is set to true.
        /// </summary>
        /// <returns>
        ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
        /// </returns>
        /// <param name="enabled">Whether the context menu should be available</param>
        public T SetContextMenuEnabled(bool enabled) {
            window.Logger.LogDebug(".SetContextMenuEnabled({Enabled})", enabled);

            window.Invoke(() => {
                InfiniFrameNative.GetContextMenuEnabled(window.InstanceHandle, out bool isEnabled);
                if (isEnabled == enabled) return;

                InfiniFrameNative.SetContextMenuEnabled(window.InstanceHandle, enabled);
            });

            return window;
        }
        #endregion

        #region SetDevToolsEnabled
        /// <summary>
        ///     When true, the user can access the browser control's developer tools.
        ///     By default, this is set to true.
        /// </summary>
        /// <returns>
        ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
        /// </returns>
        /// <param name="enabled">Whether developer tools should be available</param>
        public T SetDevToolsEnabled(bool enabled) {
            window.Logger.LogDebug(".SetDevTools({Enabled})", enabled);

            window.Invoke(() => {
                InfiniFrameNative.GetDevToolsEnabled(window.InstanceHandle, out bool isEnabled);
                if (isEnabled == enabled) return;

                InfiniFrameNative.SetDevToolsEnabled(window.InstanceHandle, enabled);
            });

            return window;
        }
        #endregion

        #region SetFullscreen
        /// <summary>
        ///     When set to true, the native window will cover the entire screen, similar to kiosk mode.
        ///     By default, this is set to false.
        /// </summary>
        /// <returns>
        ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
        /// </returns>
        /// <param name="fullScreen">Whether the window should be fullscreen</param>
        public T SetFullScreen(bool fullScreen) {
            window.Logger.LogDebug(".SetFullScreen({FullScreen})", fullScreen);
            if (window.FullScreen == fullScreen) {
                window.Logger.LogDebug("Window is already of the same fullscreen state of {fullscreen}", fullScreen);
                return window;
            }

            if (fullScreen) {
                window.Invoke(() => {
                    ImmutableArray<Monitor> monitors = MonitorsUtility.GetMonitors(window);
                    InfiniFrameNative.GetPosition(window.InstanceHandle, out int left, out int top);
                    InfiniFrameNative.GetSize(window.InstanceHandle, out int width, out int height);

                    window.CachedPreFullScreenBounds = new Rectangle(left, top, width, height);
                    if (!MonitorsUtility.TryGetCurrentMonitor(monitors, window.CachedPreFullScreenBounds, out Monitor currentMonitor)) {
                        window.Logger.LogError("Failed to get current monitor, defaulting to simple fullscreen call");
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
        #endregion

        #region SetHeight
        /// <summary>
        ///     Sets the native window <see cref="IInfiniFrameWindow.Height" /> in pixels.
        ///     Default is 0.
        /// </summary>
        /// <returns>
        ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
        /// </returns>
        /// <param name="height">Height in pixels</param>
        public T SetHeight(int height) {
            window.Logger.LogDebug(".SetHeight({Height})", height);

            window.Invoke(() => {
                InfiniFrameNative.GetSize(window.InstanceHandle, out int width, out _);
                InfiniFrameNative.SetSize(window.InstanceHandle, width, height);
            });

            return window;
        }
    
    #endregion

    #region SetIcon
        /// <summary>
        ///     Sets the icon file for the native window title bar.
        ///     The file must be located on the local machine and cannot be a URL. The default is none.
        /// </summary>
        /// <remarks>
        ///     This only works on Windows and Linux.
        /// </remarks>
        /// <returns>
        ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
        /// </returns>
        /// <exception cref="System.ArgumentException">Icon file: {value} does not exist.</exception>
        /// <param name="iconFilePath">The file path to the icon.</param>
        public T SetIconFile(string iconFilePath) {
            window.Logger.LogDebug(".SetIconFile({IconFile})", iconFilePath);

            if (window.IconFilePath == iconFilePath) {
                window.Logger.LogDebug("Icon file is already set to {IconFile}, skipping assignment", iconFilePath);
                return window;
            }

            if (!IconFileUtilities.IsValidIconFile(iconFilePath)) {
                window.Logger.LogWarning("Icon file {IconFile} does not exist or is an invalid file path.", iconFilePath);
                return window;
            }

            window.Invoke(() => InfiniFrameNative.SetIconFile(window.InstanceHandle, iconFilePath));
            return window;
        }
    
    #endregion

    #region SetLeft
        /// <summary>
        ///     Sets the native window to a new <see cref="IInfiniFrameWindow.Left" /> (X) coordinate in pixels.
        ///     Default is 0.
        /// </summary>
        /// <returns>
        ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
        /// </returns>
        /// <param name="left">Position in pixels from the left (X).</param>
        public T SetLeft(int left) {
            window.Logger.LogDebug(".SetLeft({Left})", left);

            window.Invoke(() => {
                InfiniFrameNative.GetPosition(window.InstanceHandle, out int oldLeft, out int top);
                if (left == oldLeft) return;

                InfiniFrameNative.SetPosition(window.InstanceHandle, left, top);
            });

            return window;
        }
    
    #endregion

    #region SetResizable
        /// <summary>
        ///     Sets whether the user can resize the native window.
        ///     Default is true.
        /// </summary>
        /// <returns>
        ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
        /// </returns>
        /// <param name="resizable">Whether the window is resizable</param>
        public T SetResizable(bool resizable) {
            window.Logger.LogDebug(".SetResizable({Resizable})", resizable);
            window.Invoke(() => InfiniFrameNative.SetResizable(window.InstanceHandle, resizable));
            return window;
        }
    
    #endregion

    #region SetSize
        /// <summary>
        ///     Sets the native window Size. This represents the <see cref="IInfiniFrameWindow.Width" /> and the
        ///     <see cref="IInfiniFrameWindow.Height" /> of the window in pixels.
        ///     The default Size is 0,0.
        /// </summary>
        /// <returns>
        ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
        /// </returns>
        /// <param name="width">Width in pixels</param>
        /// <param name="height">Height in pixels</param>
        public T SetSize(int width, int height) {
            window.Logger.LogDebug(".SetSize({Width}, {Height})", width, height);

            window.Invoke(() => InfiniFrameNative.SetSize(window.InstanceHandle, width, height));
            return window;
        }
        /// <summary>
        ///     Sets the native window Size. This represents the <see cref="IInfiniFrameWindow.Width" /> and the
        ///     <see cref="IInfiniFrameWindow.Height" /> of the window in pixels.
        ///     The default Size is 0,0.
        /// </summary>
        /// <returns>
        ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
        /// </returns>
        /// <param name="size">Width &amp; Height</param>
        public T SetSize(Size size) 
            => SetSize(window, size.Width, size.Height);
    #endregion

    #region SetLocation
    /// <summary>
    /// Sets the location of the window to the specified coordinates.
    /// </summary>
    /// <param name="left">The horizontal position (in pixels) from the left edge of the screen.</param>
    /// <param name="top">The vertical position (in pixels) from the top edge of the screen.</param>
    /// <returns>
    /// Returns the current <see cref="IInfiniFrameWindow" /> instance.
    /// </returns>
    public T SetLocation(int left, int top) {
            window.Logger.LogDebug(".SetLocation({left}, {right})", left, top);
            window.Invoke(() => {
                InfiniFrameNative.GetPosition(window.InstanceHandle, out int oldLeft, out int oldTop);
                if (oldLeft == left && oldTop == top) return;

                InfiniFrameNative.SetPosition(window.InstanceHandle, left, top);
            });

            return window;
        }
        
        /// <summary>
        ///     Sets the native window <see cref="IInfiniFrameWindow.Left" /> (X) and <see cref="IInfiniFrameWindow.Top" /> coordinates (Y)
        ///     in pixels.
        ///     Default is 0,0 that means the window will be aligned to the top-left edge of the screen.
        /// </summary>
        /// <returns>
        ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
        /// </returns>
        /// <param name="location">Location as a <see cref="Point" /></param>
        public T SetLocation(Point location) 
            => SetLocation(window, location.X, location.Y);
    
    #endregion

        /// <summary>
        ///     Sets whether the native window is maximized.
        ///     Default is false.
        /// </summary>
        /// <returns>
        ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
        /// </returns>
        /// <param name="maximized">Whether the window should be maximized.</param>
        public T SetMaximized(bool maximized) {
            window.Logger.LogDebug(".SetMaximized({Maximized})", maximized);
            window.Invoke(() => {
                if (!window.Chromeless) {
                    InfiniFrameNative.SetMaximized(window.InstanceHandle, maximized);
                    return;
                }

                if (!MonitorsUtility.TryGetCurrentWindowAndMonitor(window, out Rectangle windowRect, out Monitor monitor)) {
                    window.Logger.LogWarning("Monitor {Monitor} not found", monitor);
                    return;
                }

                Rectangle workArea = monitor.WorkArea;
                if (maximized) {
                    window.CachedPreMaximizedBounds = windowRect;
                    InfiniFrameNative.SetPosition(window.InstanceHandle, workArea.Left, workArea.Top);
                    InfiniFrameNative.SetSize(window.InstanceHandle, workArea.Width, workArea.Height);
                    window.Events.OnMaximized();
                }
                else if (!maximized && window.CachedPreMaximizedBounds != Rectangle.Empty) {
                    Rectangle oldRect = window.CachedPreMaximizedBounds;
                    InfiniFrameNative.SetPosition(window.InstanceHandle, oldRect.Left, oldRect.Top);
                    InfiniFrameNative.SetSize(window.InstanceHandle, oldRect.Width, oldRect.Height);
                    window.CachedPreMaximizedBounds = Rectangle.Empty;
                    window.Events.OnRestored();
                }
            });
            return window;
        }

        /// <summary>
        /// Toggles the maximized state of the current window.
        /// </summary>
        /// <returns>
        /// Returns the current <see cref="IInfiniFrameWindow" /> instance after performing the toggle action.
        /// </returns>
        /// <remarks>
        /// If the window is chromeless, the method adjusts the window size and position manually to emulate maximized behavior.
        /// Otherwise, the native maximized state is toggled.
        /// </remarks>
        public T ToggleMaximized() {
            window.Logger.LogDebug(".ToggleMaximized()");
            window.Invoke(() => {
                InfiniFrameNative.GetMaximized(window.InstanceHandle, out bool maximized);
                if (!window.Chromeless) {
                    InfiniFrameNative.SetMaximized(window.InstanceHandle, !maximized);
                    return;
                }

                // TODO test on other OS?
                // If the window is chromeless then we need to manually register the maximize size else it will just fullscreen
                if (!MonitorsUtility.TryGetCurrentWindowAndMonitor(window, out Rectangle windowRect, out Monitor monitor)) {
                    window.Logger.LogWarning("Monitor {Monitor} not found", monitor);
                    return;
                }

                Rectangle workArea = monitor.WorkArea;
                if (window.CachedPreMaximizedBounds == Rectangle.Empty) {
                    window.CachedPreMaximizedBounds = windowRect;
                    InfiniFrameNative.SetPosition(window.InstanceHandle, workArea.Left, workArea.Top);
                    InfiniFrameNative.SetSize(window.InstanceHandle, workArea.Width, workArea.Height);
                    // window.Events.OnMaximized();
                }
                else {
                    Rectangle oldRect = window.CachedPreMaximizedBounds;
                    InfiniFrameNative.SetPosition(window.InstanceHandle, oldRect.Left, oldRect.Top);
                    InfiniFrameNative.SetSize(window.InstanceHandle, oldRect.Width, oldRect.Height);
                    window.CachedPreMaximizedBounds = Rectangle.Empty;
                    // window.Events.OnRestored();
                }
            });
            return window;
        }
        
        /// <summary>
        /// Sets the maximum width and height for a native window in pixels.
        /// </summary>
        /// <param name="maxWidth">The maximum width of the window in pixels.</param>
        /// <param name="maxHeight">The maximum height of the window in pixels.</param>
        /// <returns>
        /// Returns the current <see cref="IInfiniFrameWindow" /> instance.
        /// </returns>
        public T SetMaxSize(int maxWidth, int maxHeight) {
            window.Logger.LogDebug(".SetMaxSize({MaxWidth}, {MaxHeight})", maxWidth, maxHeight);

            window.MaxWidth = maxWidth;
            window.MaxHeight = maxHeight;

            window.Invoke(() => InfiniFrameNative.SetMaxSize(window.InstanceHandle, maxWidth, maxHeight));
            return window;
        }
        
        /// <summary>
        /// Sets the maximum size constraints for the window.
        /// </summary>
        /// <param name="size">The maximum width and height of the window in pixels.</param>
        /// <returns>
        /// Returns the current <see cref="IInfiniFrameWindow" /> instance.
        /// </returns>
        public T SetMaxSize(Size size)
            => SetMaxSize(window, size.Width, size.Height);
        
        /// <summary>
        /// Sets the maximum height of the native window in pixels.
        /// </summary>
        /// <param name="maxHeight">The maximum height in pixels to apply to the window.</param>
        /// <returns>
        /// Returns the current <see cref="IInfiniFrameWindow" /> instance.
        /// </returns>
        public T SetMaxHeight(int maxHeight)
            => SetMaxSize(window, window.MaxWidth, maxHeight);
        
        /// <summary>
        /// Sets the maximum width for the native window.
        /// </summary>
        /// <param name="maxWidth">The maximum width in pixels.</param>
        /// <returns>
        /// Returns the current <see cref="IInfiniFrameWindow" /> instance with the updated maximum width.
        /// </returns>
        public T SetMaxWidth(int maxWidth)
            => SetMaxSize(window, maxWidth, window.MaxHeight);
        
        /// <summary>
        ///     Sets whether the native window is minimized (hidden).
        ///     Default is false.
        /// </summary>
        /// <returns>
        ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
        /// </returns>
        /// <param name="minimized">Whether the window should be minimized.</param>
        public T SetMinimized(bool minimized) {
            window.Logger.LogDebug(".SetMinimized({Minimized})", minimized);
            window.Invoke(() => InfiniFrameNative.SetMinimized(window.InstanceHandle, minimized));
            return window;
        }

        /// <summary>
        /// Sets the minimum width and height for a native window.
        /// </summary>
        /// <param name="minWidth">The minimum width, in pixels, the window can be resized to.</param>
        /// <param name="minHeight">The minimum height, in pixels, the window can be resized to.</param>
        /// <returns>
        /// Returns the current <see cref="IInfiniFrameWindow" /> instance.
        /// </returns>
        public T SetMinSize(int minWidth, int minHeight) {
            window.Logger.LogDebug(".SetMinSize({MinWidth}, {MinHeight})", minWidth, minHeight);

            window.MinHeight = minHeight;
            window.MinWidth = minWidth;

            window.Invoke(() => InfiniFrameNative.SetMinSize(window.InstanceHandle, minWidth, minHeight));
            return window;
        }
        
        /// <summary>
        /// Sets the minimum size for the window.
        /// </summary>
        /// <param name="size">The minimum width and height of the window in pixels.</param>
        /// <returns>
        /// Returns the current <see cref="IInfiniFrameWindow" /> instance.
        /// </returns>
        public T SetMinSize(Size size)
            => SetMinSize(window, size.Width, size.Height);
        
        /// <summary>
        /// Sets the minimum height of the window.
        /// </summary>
        /// <param name="minHeight">The minimum height, in pixels, to set for the window.</param>
        /// <returns>
        /// Returns the current <see cref="IInfiniFrameWindow" /> instance.
        /// </returns>
        public T SetMinHeight(int minHeight) 
            => SetMinSize(window, window.MinWidth, minHeight);
        
        /// <summary>
        /// Sets the minimum width of the native window.
        /// </summary>
        /// <param name="minWidth">The minimum width in pixels to set for the window.</param>
        /// <returns>
        /// Returns the current <see cref="IInfiniFrameWindow" /> instance for method chaining.
        /// </returns>
        public T SetMinWidth(int minWidth)
            => SetMinSize(window, minWidth, window.MinHeight);
        
        /// <summary>
        ///     Sets the native window <see cref="IInfiniFrameWindow.Title" />.
        ///     Default is "Photino".
        /// </summary>
        /// <returns>
        ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
        /// </returns>
        /// <param name="title">Window title</param>
        public T SetTitle(string? title) {
            window.Logger.LogDebug(".SetTitle({Title})", title);

            window.Invoke(() => {
                IntPtr ptr = InfiniFrameNative.GetTitle(window.InstanceHandle);
                string? oldTitle = Marshal.PtrToStringAuto(ptr);
                if (title == oldTitle) return;

                if (OperatingSystem.IsLinux() && title?.Length > 31) title = title[..31];// Due to Linux/Gtk platform limitations, the window title has to be no more than 31 chars
                InfiniFrameNative.SetTitle(window.InstanceHandle, title ?? string.Empty);
            });

            return window;
        }
        
        /// <summary>
        ///     Sets the native window <see cref="IInfiniFrameWindow.Top" /> (Y) coordinate in pixels.
        ///     Default is 0.
        /// </summary>
        /// <returns>
        ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
        /// </returns>
        /// <param name="top">Position in pixels from the top (Y).</param>
        public T SetTop(int top) {
            window.Logger.LogDebug(".SetTop({Top})", top);
            window.Invoke(() => {
                InfiniFrameNative.GetPosition(window.InstanceHandle, out int left, out int oldTop);
                if (top == oldTop) return;

                InfiniFrameNative.SetPosition(window.InstanceHandle, left, top);
            });

            return window;
        }
        
        /// <summary>
        ///     Sets whether the native window is always at the top of the z-order.
        ///     Default is false.
        /// </summary>
        /// <returns>
        ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
        /// </returns>
        /// <param name="topMost">Whether the window is at the top</param>
        public T SetTopMost(bool topMost) {
            window.Logger.LogDebug(".SetTopMost({TopMost})", topMost);
            window.Invoke(() => InfiniFrameNative.SetTopmost(window.InstanceHandle, topMost));
            return window;
        }
        
        /// <summary>
        ///     Sets the native window width in pixels.
        ///     Default is 0.
        /// </summary>
        /// <returns>
        ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
        /// </returns>
        /// <param name="width">Width in pixels</param>
        public T SetWidth(int width) {
            window.Logger.LogDebug(".SetWidth({Width})", width);

            window.Invoke(() => {
                InfiniFrameNative.GetSize(window.InstanceHandle, out _, out int height);
                InfiniFrameNative.SetSize(window.InstanceHandle, width, height);
            });

            return window;
        }
        
        /// <summary>
        ///     Sets the native browser control <see cref="IInfiniFrameWindow.Zoom" />.
        ///     Default is 100.
        /// </summary>
        /// <returns>
        ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
        /// </returns>
        /// <param name="zoom">Zoomlevel as integer</param>
        /// <example>100 = 100%, 50 = 50%</example>
        public T SetZoom(int zoom) {
            window.Logger.LogDebug(".SetZoom({Zoom})", zoom);
            window.Invoke(() => InfiniFrameNative.SetZoom(window.InstanceHandle, zoom));
            return window;
        }
        
        /// <summary>
        ///     Set the runtime path for WebView2 so that developers can use Photino on Windows using the "Fixed Version"
        ///     deployment
        ///     module of the WebView2 runtime.
        /// </summary>
        /// <remarks>
        ///     This only works on Windows.
        /// </remarks>
        /// <returns>
        ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
        /// </returns>
        /// <seealso href="https://docs.microsoft.com/en-us/microsoft-edge/webview2/concepts/distribution" />
        /// <param name="data">Runtime path for WebView2</param>
        public T Win32SetWebView2Path(string data) {
            if (OperatingSystem.IsWindows())
                window.Invoke(() => InfiniFrameNative.SetWebView2RuntimePath_win32(window.NativeType, data));
            else
                window.Logger.LogDebug("Win32SetWebView2Path is only supported on the Windows platform");

            return window;
        }
        
        /// <summary>
        ///     Clears the autofill data in the browser control.
        /// </summary>
        /// <remarks>
        ///     This method is only supported on the Windows platform.
        /// </remarks>
        /// <returns>
        ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
        /// </returns>
        public T ClearBrowserAutoFill() {
            if (OperatingSystem.IsWindows())
                window.Invoke(() => InfiniFrameNative.ClearBrowserAutoFill(window.InstanceHandle));
            else
                window.Logger.LogWarning("ClearBrowserAutoFill is only supported on the Windows platform");

            return window;
        }

        /// <summary>
        /// Resizes the current window relative to its existing dimensions and specified origin.
        /// </summary>
        /// <param name="widthOffset">The amount by which to adjust the window's width.</param>
        /// <param name="heightOffset">The amount by which to adjust the window's height.</param>
        /// <param name="origin">The origin point from which resizing operations are measured.</param>
        /// <returns>
        /// Returns the current <see cref="IInfiniFrameWindow" /> instance.
        /// </returns>
        public T Resize(int widthOffset, int heightOffset, ResizeOrigin origin) {
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

        /// <summary>
        /// Enables or disables the zoom functionality for the specified <see cref="IInfiniFrameWindow" /> instance.
        /// </summary>
        /// <param name="zoomEnabled">A boolean value indicating whether zoom functionality should be enabled or disabled.</param>
        /// <returns>
        /// Returns the current <see cref="IInfiniFrameWindow" /> instance.
        /// </returns>
        public T SetZoomEnabled(bool zoomEnabled) {
            window.Invoke(() => InfiniFrameNative.SetZoomEnabled(window.InstanceHandle, zoomEnabled));
            return window;
        }

        /// <summary>
        /// Sets the specified window as focused.
        /// </summary>
        /// <returns>
        /// Returns the current <see cref="IInfiniFrameWindow" /> instance.
        /// </returns>
        /// <remarks>
        /// This method invokes the native function to set focus on the window instance.
        /// </remarks>
        public T SetFocused() {
            window.Invoke(() => InfiniFrameNative.SetFocused(window.InstanceHandle));
            return window;
        }
    }
}
