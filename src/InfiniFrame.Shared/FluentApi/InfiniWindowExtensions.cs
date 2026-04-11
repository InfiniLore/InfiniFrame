// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Native;
using InfiniFrame.Utilities;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[SuppressMessage("ReSharper", "ConvertToExtensionBlock")]
public static class InfiniWindowExtensions {
    /// <summary>
    ///     Loads specified <see cref="Uri" /> into the browser control.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
    /// </returns>
    /// <param name="uri">A Uri pointing to the file or the URL to load.</param>
    /// <param name="window">InfiniFrame window instance</param>
    public static T Load<T>(this T window, Uri uri) where T : class, IInfiniFrameWindow {
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
    /// <param name="path">A path pointing to the resource to load.</param>
    /// <param name="window">InfiniFrame window instance</param>
    public static T Load<T>(this T window, string path) where T : class, IInfiniFrameWindow {
        window.Logger.LogDebug(".Load({Path})", path);

        // TODO patch this
        // ––––––––––––––––––––––
        // SECURITY RISK!
        // This needs validation!
        // ––––––––––––––––––––––
        // Open a web URL string path
        if (path.Contains("http://") || path.Contains("https://"))
            return window.Load(new Uri(path));

        // Open a file resource string path
        string absolutePath = Path.GetFullPath(path);

        // For a bundled app it can be necessary to consider
        // the app context base directory. Check there too.
        if (File.Exists(absolutePath)) return window.Load(new Uri(absolutePath, UriKind.Absolute));

        absolutePath = $"{AppContext.BaseDirectory}/{path}";

        if (File.Exists(absolutePath)) return window.Load(new Uri(absolutePath, UriKind.Absolute));

        if (window.TryResolveStaticAssetUri(path, out Uri staticAssetUri))
            return window.Load(staticAssetUri);

        window.Logger.LogWarning("File not found: {Path}", absolutePath);
        return window;
    }

    /// <summary>
    ///     Loads a raw string into the browser control.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
    /// </returns>
    /// <param name="content">Raw content (such as HTML)</param>
    /// <param name="window">InfiniFrame window instance</param>
    public static T LoadRawString<T>(this T window, string content) where T : class, IInfiniFrameWindow {
        string shortContent = content.Length > 50 ? string.Concat(content.AsSpan(0, 47), "...") : content;
        window.Logger.LogDebug(".LoadRawString({Content})", shortContent);
        window.Invoke(() => InfiniFrameNative.NavigateToString(window.InstanceHandle, content));
        return window;
    }

    /// <summary>
    ///     Centers the native window on the primary display.
    /// </summary>
    /// <param name="window">InfiniFrame window instance</param>
    /// <returns>
    ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
    /// </returns>
    public static T Center<T>(this T window) where T : class, IInfiniFrameWindow {
        window.Logger.LogDebug(".Center()");
        window.Invoke(() => InfiniFrameNative.Center(window.InstanceHandle));
        return window;
    }

    /// <summary>
    /// Centers the current window on the monitor where it is currently displayed.
    /// </summary>
    /// <param name="window">InfiniFrame window instance</param>
    /// <returns>
    /// Returns the current <see cref="IInfiniFrameWindow" /> instance.
    /// </returns>
    public static T CenterOnCurrentMonitor<T>(this T window) where T : class, IInfiniFrameWindow {
        window.Invoke(() => {
            ImmutableArray<InfiniMonitor> monitors = MonitorsUtility.GetMonitors(window);
            InfiniFrameNative.GetWindowRectangle(window.InstanceHandle, out Rectangle rectangle);

            // TODO think about proper unhappy flow here
            if (!MonitorsUtility.TryGetCurrentMonitor(monitors, rectangle, out InfiniMonitor monitor)) return;

            Rectangle area = monitor.MonitorArea;

            var newLocation = new Point(area.X + area.Width / 2 - rectangle.Width / 2, area.Y + area.Height / 2 - rectangle.Height / 2);
            InfiniFrameNative.SetPosition(window.InstanceHandle, newLocation.X, newLocation.Y);
        });

        return window;
    }

    /// <summary>
    /// Centers the specified <see cref="IInfiniFrameWindow" /> on a monitor.
    /// </summary>
    /// <param name="monitorIndex">
    /// The index of the monitor on which the window should be centered.
    /// </param>
    /// <param name="window">InfiniFrame window instance</param>
    /// <returns>
    /// Returns the current <see cref="IInfiniFrameWindow" /> instance.
    /// </returns>
    public static T CenterOnMonitor<T>(this T window, int monitorIndex) where T : class, IInfiniFrameWindow {
        window.Invoke(() => {
            ImmutableArray<InfiniMonitor> monitors = MonitorsUtility.GetMonitors(window);

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

    /// <summary>
    ///     Moves the native window to the specified location on the screen in pixels using a Point.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
    /// </returns>
    /// <param name="left">Position from left in pixels</param>
    /// <param name="top">Position from top in pixels</param>
    /// <param name="window">InfiniFrame window instance</param>
    public static T MoveWithinCurrentMonitorArea<T>(this T window, int left, int top) where T : class, IInfiniFrameWindow {
        window.Invoke(() => {
            MonitorsUtility.TryGetCurrentWindowAndMonitor(window, out Rectangle windowRect, out InfiniMonitor monitor);
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
            // This behavior seems to be a macOS thing. In the InfiniFrame.Native
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
    ///     using <see cref="IHasInfiniFrameProperties.Left" /> (X) and <see cref="IHasInfiniFrameProperties.Top" /> (Y) properties.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
    /// </returns>
    /// <param name="location">Position as <see cref="Point" /></param>
    /// <param name="window">InfiniFrame window instance</param>
    public static T MoveWithinCurrentMonitorArea<T>(this T window, Point location) where T : class, IInfiniFrameWindow
        => window.MoveWithinCurrentMonitorArea(location.X, location.Y);

    /// <summary>
    /// Moves the native window to the specified location on the screen using a double value.
    /// </summary>
    /// <param name="left">Left Position</param>
    /// <param name="top">Top Position</param>
    /// <param name="window">InfiniFrame window instance</param>
    /// <returns>
    ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
    /// </returns>
    public static T MoveWithinCurrentMonitorArea<T>(this T window, double left, double top) where T : class, IInfiniFrameWindow
        => window.MoveWithinCurrentMonitorArea((int)left, (int)top);

    /// <summary>
    ///     Moves the native window relative to its current location on the screen in pixels
    ///     using <see cref="IHasInfiniFrameProperties.Left" /> (X) and <see cref="IHasInfiniFrameProperties.Top" /> (Y) properties.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
    /// </returns>
    /// <param name="left">Relative offset from left in pixels</param>
    /// <param name="top">Relative offset from top in pixels</param>
    /// <param name="window">InfiniFrame window instance</param>
    public static T Offset<T>(this T window, int left, int top) where T : class, IInfiniFrameWindow {
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
    /// <param name="window">InfiniFrame window instance</param>
    public static T Offset<T>(this T window, Point offset) where T : class, IInfiniFrameWindow
        => window.Offset(offset.X, offset.Y);

    /// <summary>
    /// Moves the native window relative to its current location on the screen in pixels
    /// using <see cref="IHasInfiniFrameProperties.Left" /> (X) and <see cref="IHasInfiniFrameProperties.Top" /> (Y) properties.
    /// </summary>
    /// <returns>
    /// Returns the current <see cref="IInfiniFrameWindow" /> instance.
    /// </returns>
    /// <param name="left">Relative offset from the left in pixels.</param>
    /// <param name="top">Relative offset from the top in pixels.</param>
    /// <param name="window">InfiniFrame window instance</param>
    public static T Offset<T>(this T window, double left, double top) where T : class, IInfiniFrameWindow
        => window.Offset((int)left, (int)top);

    /// <summary>
    ///     When true, the native window can be displayed with a transparent background.
    ///     Chromeless must be set to true. HTML document's body background must have alpha-based value.
    ///     By default, this is set to false.
    /// </summary>
    /// <param name="window">InfiniFrame window instance</param>
    /// <param name="enabled">Enables a transparent background for the native window</param>
    public static T SetTransparent<T>(this T window, bool enabled) where T : class, IInfiniFrameWindow {
        window.Logger.LogDebug(".SetTransparent({Enabled})", enabled);

        if (OperatingSystem.IsWindows()) {
            window.Logger.LogWarning("Transparent can only be set on Windows before the native window is instantiated.");
            return window;
        }

        window.Logger.LogDebug("Invoking InfiniFrameNative.SetTransparentEnabled({value})", enabled);
        window.Invoke(() => InfiniFrameNative.SetTransparentEnabled(window.InstanceHandle, enabled));
        return window;
    }

    /// <summary>
    ///     When true, the user can access the browser control's context menu.
    ///     By default, this is set to true.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
    /// </returns>
    /// <param name="enabled">Whether the context menu should be available</param>
    /// <param name="window">InfiniFrame window instance</param>
    public static T SetContextMenuEnabled<T>(this T window, bool enabled) where T : class, IInfiniFrameWindow {
        window.Logger.LogDebug(".SetContextMenuEnabled({Enabled})", enabled);

        window.Invoke(() => {
            InfiniFrameNative.GetContextMenuEnabled(window.InstanceHandle, out bool isEnabled);
            if (isEnabled == enabled) return;

            InfiniFrameNative.SetContextMenuEnabled(window.InstanceHandle, enabled);
        });

        return window;
    }

    /// <summary>
    ///     When true, the user can access the browser control's developer tools.
    ///     By default, this is set to true.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
    /// </returns>
    /// <param name="enabled">Whether developer tools should be available</param>
    /// <param name="window">InfiniFrame window instance</param>
    public static T SetDevToolsEnabled<T>(this T window, bool enabled) where T : class, IInfiniFrameWindow {
        window.Logger.LogDebug(".SetDevTools({Enabled})", enabled);

        window.Invoke(() => {
            InfiniFrameNative.GetDevToolsEnabled(window.InstanceHandle, out bool isEnabled);
            if (isEnabled == enabled) return;

            InfiniFrameNative.SetDevToolsEnabled(window.InstanceHandle, enabled);
        });

        return window;
    }

    /// <summary>
    ///     When set to true, the native window will cover the entire screen, similar to kiosk mode.
    ///     By default, this is set to false.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
    /// </returns>
    /// <param name="fullScreen">Whether the window should be fullscreen</param>
    /// <param name="window">InfiniFrame window instance</param>
    public static T SetFullScreen<T>(this T window, bool fullScreen) where T : class, IInfiniFrameWindow {
        window.Logger.LogDebug(".SetFullScreen({FullScreen})", fullScreen);
        if (window.FullScreen == fullScreen) {
            window.Logger.LogDebug("Window is already of the same fullscreen state of {fullscreen}", fullScreen);
            return window;
        }

        if (fullScreen) {
            window.Invoke(()
                => {
                ImmutableArray<InfiniMonitor> monitors = MonitorsUtility.GetMonitors(window);
                InfiniFrameNative.GetPosition(window.InstanceHandle, out int left, out int top);
                InfiniFrameNative.GetSize(window.InstanceHandle, out int width, out int height);

                window.CachedPreFullScreenBounds = new Rectangle(left, top, width, height);
                if (!MonitorsUtility.TryGetCurrentMonitor(monitors, window.CachedPreFullScreenBounds, out InfiniMonitor currentMonitor)) {
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

    /// <summary>
    ///     Sets the native window <see cref="IHasInfiniFrameProperties.Height" /> in pixels.
    ///     Default is 0.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
    /// </returns>
    /// <param name="height">Height in pixels</param>
    /// <param name="window">InfiniFrame window instance</param>
    public static T SetHeight<T>(this T window, int height) where T : class, IInfiniFrameWindow {
        window.Logger.LogDebug(".SetHeight({Height})", height);

        window.Invoke(() => {
            InfiniFrameNative.GetSize(window.InstanceHandle, out int width, out _);
            InfiniFrameNative.SetSize(window.InstanceHandle, width, height);
        });

        return window;
    }

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
    /// <param name="window">InfiniFrame window instance</param>
    public static T SetIconFile<T>(this T window, string iconFilePath) where T : class, IInfiniFrameWindow {
        window.Logger.LogDebug(".SetIconFile({IconFile})", iconFilePath);

        if (!IconFileUtilities.TryResolveIconFilePath(iconFilePath, out string? resolvedIconFilePath)) {
            window.Logger.LogWarning("Icon file {IconFile} does not exist or is an invalid file path.", iconFilePath);
            return window;
        }

        if (window.IconFilePath == resolvedIconFilePath) {
            window.Logger.LogDebug("Icon file is already set to {IconFile}, skipping assignment", resolvedIconFilePath);
            return window;
        }

        window.Invoke(() => InfiniFrameNative.SetIconFile(window.InstanceHandle, resolvedIconFilePath));
        return window;
    }

    /// <summary>
    ///     Sets the native window to a new <see cref="IHasInfiniFrameProperties.Left" /> (X) coordinate in pixels.
    ///     Default is 0.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
    /// </returns>
    /// <param name="left">Position in pixels from the left (X).</param>
    /// <param name="window">InfiniFrame window instance</param>
    public static T SetLeft<T>(this T window, int left) where T : class, IInfiniFrameWindow {
        window.Logger.LogDebug(".SetLeft({Left})", left);

        window.Invoke(() => {
            InfiniFrameNative.GetPosition(window.InstanceHandle, out int oldLeft, out int top);
            if (left == oldLeft) return;

            InfiniFrameNative.SetPosition(window.InstanceHandle, left, top);
        });

        return window;
    }

    /// <summary>
    ///     Sets whether the user can resize the native window.
    ///     Default is true.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
    /// </returns>
    /// <param name="resizable">Whether the window is resizable</param>
    /// <param name="window">InfiniFrame window instance</param>
    public static T SetResizable<T>(this T window, bool resizable) where T : class, IInfiniFrameWindow {
        window.Logger.LogDebug(".SetResizable({Resizable})", resizable);
        window.Invoke(() => InfiniFrameNative.SetResizable(window.InstanceHandle, resizable));
        return window;
    }

    /// <summary>
    ///     Sets the native window Size. This represents the <see cref="IHasInfiniFrameProperties.Width" /> and the
    ///     <see cref="IHasInfiniFrameProperties.Height" /> of the window in pixels.
    ///     The default Size is 0,0.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
    /// </returns>
    /// <param name="width">Width in pixels</param>
    /// <param name="height">Height in pixels</param>
    /// <param name="window">InfiniFrame window instance</param>
    public static T SetSize<T>(this T window, int width, int height) where T : class, IInfiniFrameWindow {
        window.Logger.LogDebug(".SetSize({Width}, {Height})", width, height);

        window.Invoke(() => InfiniFrameNative.SetSize(window.InstanceHandle, width, height));
        return window;
    }

    /// <summary>
    ///     Sets the native window Size. This represents the <see cref="IHasInfiniFrameProperties.Width" /> and the
    ///     <see cref="IHasInfiniFrameProperties.Height" /> of the window in pixels.
    ///     The default Size is 0,0.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
    /// </returns>
    /// <param name="size">Width &amp; Height</param>
    /// <param name="window">InfiniFrame window instance</param>
    public static T SetSize<T>(this T window, Size size) where T : class, IInfiniFrameWindow
        => window.SetSize(size.Width, size.Height);

    /// <summary>
    /// Sets the location of the window to the specified coordinates.
    /// </summary>
    /// <param name="left">The horizontal position (in pixels) from the left edge of the screen.</param>
    /// <param name="top">The vertical position (in pixels) from the top edge of the screen.</param>
    /// <param name="window">InfiniFrame window instance</param>
    /// <returns>
    /// Returns the current <see cref="IInfiniFrameWindow" /> instance.
    /// </returns>
    public static T SetLocation<T>(this T window, int left, int top) where T : class, IInfiniFrameWindow {
        window.Logger.LogDebug(".SetLocation({left}, {right})", left, top);
        window.Invoke(() => {
            InfiniFrameNative.GetPosition(window.InstanceHandle, out int oldLeft, out int oldTop);
            if (oldLeft == left && oldTop == top) return;

            InfiniFrameNative.SetPosition(window.InstanceHandle, left, top);
        });

        return window;
    }

    /// <summary>
    ///     Sets the native window <see cref="IHasInfiniFrameProperties.Left" /> (X) and <see cref="IHasInfiniFrameProperties.Top" /> coordinates (Y)
    ///     in pixels.
    ///     Default is 0,0 that means the window will be aligned to the top-left edge of the screen.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
    /// </returns>
    /// <param name="location">Location as a <see cref="Point" /></param>
    /// <param name="window">InfiniFrame window instance</param>
    public static T SetLocation<T>(this T window, Point location) where T : class, IInfiniFrameWindow
        => window.SetLocation(location.X, location.Y);

    /// <summary>
    ///     Sets whether the native window is maximized.
    ///     Default is false.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
    /// </returns>
    /// <param name="maximized">Whether the window should be maximized.</param>
    /// <param name="window">InfiniFrame window instance</param>
    public static T SetMaximized<T>(this T window, bool maximized) where T : class, IInfiniFrameWindow {
        window.Logger.LogDebug(".SetMaximized({Maximized})", maximized);
        window.Invoke(() => {
            if (!window.Chromeless) {
                InfiniFrameNative.SetMaximized(window.InstanceHandle, maximized);
                return;
            }

            if (!MonitorsUtility.TryGetCurrentWindowAndMonitor(window, out Rectangle windowRect, out InfiniMonitor monitor)) {
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

    /// <summary>
    /// Toggles the maximized state of the current window.
    /// </summary>
    /// <param name="window">InfiniFrame window instance</param>
    /// <returns>
    /// Returns the current <see cref="IInfiniFrameWindow" /> instance after performing the toggle action.
    /// </returns>
    /// <remarks>
    /// If the window is chromeless, the method adjusts the window size and position manually to emulate maximized behavior.
    /// Otherwise, the native maximized state is toggled.
    /// </remarks>
    public static T ToggleMaximized<T>(this T window) where T : class, IInfiniFrameWindow {
        window.Logger.LogDebug(".ToggleMaximized()");
        window.Invoke(() => {
            InfiniFrameNative.GetMaximized(window.InstanceHandle, out bool maximized);
            if (!window.Chromeless) {
                InfiniFrameNative.SetMaximized(window.InstanceHandle, !maximized);
                return;
            }

            // TODO test on other OS?
            // If the window is chromeless then we need to manually register the maximize size else it will just fullscreen
            if (!MonitorsUtility.TryGetCurrentWindowAndMonitor(window, out Rectangle windowRect, out InfiniMonitor monitor)) {
                window.Logger.LogWarning("Monitor {Monitor} not found", monitor);
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

    /// <summary>
    /// Sets the maximum width and height for a native window in pixels.
    /// </summary>
    /// <param name="maxWidth">The maximum width of the window in pixels.</param>
    /// <param name="maxHeight">The maximum height of the window in pixels.</param>
    /// <param name="window">InfiniFrame window instance</param>
    /// <returns>
    /// Returns the current <see cref="IInfiniFrameWindow" /> instance.
    /// </returns>
    public static T SetMaxSize<T>(this T window, int maxWidth, int maxHeight) where T : class, IInfiniFrameWindow {
        window.Logger.LogDebug(".SetMaxSize({MaxWidth}, {MaxHeight})", maxWidth, maxHeight);
        window.Invoke(() => InfiniFrameNative.SetMaxSize(window.InstanceHandle, maxWidth, maxHeight));
        return window;
    }

    /// <summary>
    /// Sets the maximum size constraints for the window.
    /// </summary>
    /// <param name="size">The maximum width and height of the window in pixels.</param>
    /// <param name="window">InfiniFrame window instance</param>
    /// <returns>
    /// Returns the current <see cref="IInfiniFrameWindow" /> instance.
    /// </returns>
    public static T SetMaxSize<T>(this T window, Size size) where T : class, IInfiniFrameWindow
        => window.SetMaxSize(size.Width, size.Height);

    /// <summary>
    /// Sets the maximum height of the native window in pixels.
    /// </summary>
    /// <param name="maxHeight">The maximum height in pixels to apply to the window.</param>
    /// <param name="window">InfiniFrame window instance</param>
    /// <returns>
    /// Returns the current <see cref="IInfiniFrameWindow" /> instance.
    /// </returns>
    public static T SetMaxHeight<T>(this T window, int maxHeight) where T : class, IInfiniFrameWindow
        => window.SetMaxSize(window.MaxWidth, maxHeight);

    /// <summary>
    /// Sets the maximum width for the native window.
    /// </summary>
    /// <param name="maxWidth">The maximum width in pixels.</param>
    /// <param name="window">InfiniFrame window instance</param>
    /// <returns>
    /// Returns the current <see cref="IInfiniFrameWindow" /> instance with the updated maximum width.
    /// </returns>
    public static T SetMaxWidth<T>(this T window, int maxWidth) where T : class, IInfiniFrameWindow
        => window.SetMaxSize(maxWidth, window.MaxHeight);

    /// <summary>
    ///     Sets whether the native window is minimized (hidden).
    ///     Default is false.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
    /// </returns>
    /// <param name="minimized">Whether the window should be minimized.</param>
    /// <param name="window">InfiniFrame window instance</param>
    public static T SetMinimized<T>(this T window, bool minimized) where T : class, IInfiniFrameWindow {
        window.Logger.LogDebug(".SetMinimized({Minimized})", minimized);
        window.Invoke(() => InfiniFrameNative.SetMinimized(window.InstanceHandle, minimized));
        return window;
    }

    /// <summary>
    /// Sets the minimum width and height for a native window.
    /// </summary>
    /// <param name="minWidth">The minimum width, in pixels, the window can be resized to.</param>
    /// <param name="minHeight">The minimum height, in pixels, the window can be resized to.</param>
    /// <param name="window">InfiniFrame window instance</param>
    /// <returns>
    /// Returns the current <see cref="IInfiniFrameWindow" /> instance.
    /// </returns>
    public static T SetMinSize<T>(this T window, int minWidth, int minHeight) where T : class, IInfiniFrameWindow {
        window.Logger.LogDebug(".SetMinSize({MinWidth}, {MinHeight})", minWidth, minHeight);
        window.Invoke(() => InfiniFrameNative.SetMinSize(window.InstanceHandle, minWidth, minHeight));
        return window;
    }

    /// <summary>
    /// Sets the minimum size for the window.
    /// </summary>
    /// <param name="size">The minimum width and height of the window in pixels.</param>
    /// <param name="window">InfiniFrame window instance</param>
    /// <returns>
    /// Returns the current <see cref="IInfiniFrameWindow" /> instance.
    /// </returns>
    public static T SetMinSize<T>(this T window, Size size) where T : class, IInfiniFrameWindow
        => window.SetMinSize(size.Width, size.Height);

    /// <summary>
    /// Sets the minimum height of the window.
    /// </summary>
    /// <param name="minHeight">The minimum height, in pixels, to set for the window.</param>
    /// <param name="window">InfiniFrame window instance</param>
    /// <returns>
    /// Returns the current <see cref="IInfiniFrameWindow" /> instance.
    /// </returns>
    public static T SetMinHeight<T>(this T window, int minHeight) where T : class, IInfiniFrameWindow
        => window.SetMinSize(window.MinWidth, minHeight);

    /// <summary>
    /// Sets the minimum width of the native window.
    /// </summary>
    /// <param name="minWidth">The minimum width in pixels to set for the window.</param>
    /// <param name="window">InfiniFrame window instance</param>
    /// <returns>
    /// Returns the current <see cref="IInfiniFrameWindow" /> instance for method chaining.
    /// </returns>
    public static T SetMinWidth<T>(this T window, int minWidth) where T : class, IInfiniFrameWindow
        => window.SetMinSize(minWidth, window.MinHeight);

    /// <summary>
    ///     Sets the native window <see cref="IHasInfiniFrameProperties.Title" />.
    ///     Default is "InfiniFrame".
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
    /// </returns>
    /// <param name="title">Window title</param>
    /// <param name="window">InfiniFrame window instance</param>
    public static T SetTitle<T>(this T window, string? title) where T : class, IInfiniFrameWindow {
        window.Logger.LogDebug(".SetTitle({Title})", title);

        window.Invoke(() => {
            IntPtr ptr = InfiniFrameNative.GetTitle(window.InstanceHandle);
            string? oldTitle = InfiniFrameNative.PtrToNativeString(ptr);
            if (title == oldTitle) return;

            if (OperatingSystem.IsLinux() && title?.Length > 31) title = title[..31];// Due to Linux/Gtk platform limitations, the window title has to be no more than 31 chars
            InfiniFrameNative.SetTitle(window.InstanceHandle, title ?? string.Empty);
        });

        return window;
    }

    /// <summary>
    ///     Sets the native window <see cref="IHasInfiniFrameProperties.Top" /> (Y) coordinate in pixels.
    ///     Default is 0.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
    /// </returns>
    /// <param name="top">Position in pixels from the top (Y).</param>
    /// <param name="window">InfiniFrame window instance</param>
    public static T SetTop<T>(this T window, int top) where T : class, IInfiniFrameWindow {
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
    /// <param name="window">InfiniFrame window instance</param>
    public static T SetTopMost<T>(this T window, bool topMost) where T : class, IInfiniFrameWindow {
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
    /// <param name="window">InfiniFrame window instance</param>
    public static T SetWidth<T>(this T window, int width) where T : class, IInfiniFrameWindow {
        window.Logger.LogDebug(".SetWidth({Width})", width);

        window.Invoke(() => {
            InfiniFrameNative.GetSize(window.InstanceHandle, out _, out int height);
            InfiniFrameNative.SetSize(window.InstanceHandle, width, height);
        });

        return window;
    }

    /// <summary>
    ///     Sets the native browser control <see cref="IHasInfiniFrameProperties.Zoom" />.
    ///     Default is 100.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
    /// </returns>
    /// <param name="zoom">Zoomlevel as integer</param>
    /// <param name="window">InfiniFrame window instance</param>
    /// <example>100 = 100%, 50 = 50%</example>
    public static T SetZoom<T>(this T window, int zoom) where T : class, IInfiniFrameWindow {
        window.Logger.LogDebug(".SetZoom({Zoom})", zoom);
        window.Invoke(() => InfiniFrameNative.SetZoom(window.InstanceHandle, zoom));
        return window;
    }

    /// <summary>
    ///     Set the runtime path for WebView2 so that developers can use InfiniFrame on Windows using the "Fixed Version"
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
    /// <param name="window">InfiniFrame window instance</param>
    public static T Win32SetWebView2Path<T>(this T window, string data) where T : class, IInfiniFrameWindow {
        if (OperatingSystem.IsWindows())
            window.Invoke(()
                => InfiniFrameNative.SetWebView2RuntimePath_win32(window.NativeType, data));
        else
            window.Logger.LogDebug("Win32SetWebView2Path is only supported on the Windows platform");

        return window;
    }

    /// <summary>
    ///     Clears the autofill data in the browser control.
    /// </summary>
    /// <param name="window">InfiniFrame window instance</param>
    /// <remarks>
    ///     This method is only supported on the Windows platform.
    /// </remarks>
    /// <returns>
    ///     Returns the current <see cref="IInfiniFrameWindow" /> instance.
    /// </returns>
    public static T ClearBrowserAutoFill<T>(this T window) where T : class, IInfiniFrameWindow {
        if (OperatingSystem.IsWindows())
            window.Invoke(()
                => InfiniFrameNative.ClearBrowserAutoFill(window.InstanceHandle));
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
    /// <param name="window">InfiniFrame window instance</param>
    /// <returns>
    /// Returns the current <see cref="IInfiniFrameWindow" /> instance.
    /// </returns>
    public static T Resize<T>(this T window, int widthOffset, int heightOffset, ResizeOrigin origin) where T : class, IInfiniFrameWindow {
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
    /// <param name="window">InfiniFrame window instance</param>
    /// <returns>
    /// Returns the current <see cref="IInfiniFrameWindow" /> instance.
    /// </returns>
    public static T SetZoomEnabled<T>(this T window, bool zoomEnabled) where T : class, IInfiniFrameWindow {
        window.Invoke(() => InfiniFrameNative.SetZoomEnabled(window.InstanceHandle, zoomEnabled));
        return window;
    }

    /// <summary>
    /// Sets the specified window as focused.
    /// </summary>
    /// <param name="window">InfiniFrame window instance</param>
    /// <returns>
    /// Returns the current <see cref="IInfiniFrameWindow" /> instance.
    /// </returns>
    /// <remarks>
    /// This method invokes the native function to set focus on the window instance.
    /// </remarks>
    public static T SetFocused<T>(this T window) where T : class, IInfiniFrameWindow {
        window.Invoke(() => InfiniFrameNative.SetFocused(window.InstanceHandle));
        return window;
    }
}
