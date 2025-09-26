using System.Runtime.InteropServices;

namespace InfiniLore.Photino.NET;
using InfiniLore.Photino.NET.Utilities;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using System.Drawing;

public static class PhotinoWindowExtensions {
    #region Load
    /// <summary>
    ///     Loads specified <see cref="Uri" /> into the browser control.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="PhotinoWindow" /> instance.
    /// </returns>
    /// <remarks>
    ///     Load() or LoadString() must be called before a native window is initialized.
    /// </remarks>
    /// <param name="window">Photino window instance</param>
    /// <param name="uri">A Uri pointing to the file or the URL to load.</param>
    public static T Load<T>(this T window, Uri uri) where T : class, IPhotinoWindow {
        window.Logger.LogDebug(".Load({uri})", uri);
        window.Invoke(() => PhotinoNative.NavigateToUrl(window.InstanceHandle, uri.ToString()));
        return window;
    }

    /// <summary>
    ///     Loads a specified path into the browser control.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <remarks>
    ///     Load() or LoadString() must be called before a native window is initialized.
    /// </remarks>
    /// <param name="window">Photino window instance</param>
    /// <param name="path">A path pointing to the resource to load.</param>
    public static T Load<T>(this T window, string path) where T : class, IPhotinoWindow {
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
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <remarks>
    ///     Used to load HTML into the browser control directly.
    ///     Load() or LoadString() must be called before a native window is initialized.
    /// </remarks>
    /// <param name="window">Photino window instance</param>
    /// <param name="content">Raw content (such as HTML)</param>
    public static T LoadRawString<T>(this T window, string content) where T : class, IPhotinoWindow {
        string shortContent = content.Length > 50 ? string.Concat(content.AsSpan(0, 47), "...") : content;
        window.Logger.LogDebug(".LoadRawString({Content})", shortContent);
        window.Invoke(() => PhotinoNative.NavigateToString(window.InstanceHandle, content));
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
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    public static T Center<T>(this T window) where T : class, IPhotinoWindow {
        window.Logger.LogDebug(".Center()");
        window.Invoke(() => PhotinoNative.Center(window.InstanceHandle));
        return window;
    }

    // ReSharper disable once RedundantArgumentDefaultValue
    public static T CenterOnCurrentMonitor<T>(this T window) where T : class, IPhotinoWindow
        => CenterOnMonitor(window, -1);
    
    public static T CenterOnMonitor<T>(this T window, int monitorIndex = -1) where T : class, IPhotinoWindow {
        ImmutableArray<Monitor> monitors = InvokeUtilities.InvokeAndReturn(window, MonitorsUtility.GetMonitors);

        if (monitorIndex <= -1 ) {
            window.Invoke(() => {
                PhotinoNative.GetRectangle(window.InstanceHandle, out Rectangle rectangle);
                
                // TODO think about proper unhappy flow here
                if (!MonitorsUtility.TryGetCurrentMonitor(monitors, rectangle, out var monitor)) return;
                Rectangle area = monitor.MonitorArea;
                
                var newLocation = new Point(area.X + area.Width / 2 - rectangle.Width / 2, area.Y + area.Height / 2 - rectangle.Height / 2);
                PhotinoNative.SetPosition(window.InstanceHandle, newLocation.X, newLocation.Y);
            });
        }
        
        if (monitorIndex < 0 || monitorIndex >= monitors.Length) {
            window.Logger.LogWarning("Monitor index {MonitorIndex} is out of range. Available monitors: {Monitors}", monitorIndex, monitors.Length);
            return window;
        }

        window.Invoke(() => {
            PhotinoNative.GetSize(window.InstanceHandle, out Size size);
            Rectangle area = monitors[monitorIndex].MonitorArea;
            
            var newLocation = new Point(area.X + area.Width / 2 - size.Width / 2, area.Y + area.Height / 2 - size.Height / 2);
            PhotinoNative.SetPosition(window.InstanceHandle, newLocation.X, newLocation.Y);
        });
        
        return window;  
    }
    #endregion

    #region MoveTo
    /// <summary>
    ///     Moves the native window to the specified location on the screen in pixels using a Point.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <param name="window">Photino window instance</param>
    /// <param name="location">Position as <see cref="Point" /></param>
    /// <param name="allowOutsideWorkArea">Whether the window can go off-screen (work area)</param>
    public static T MoveTo<T>(this T window, Point location, bool allowOutsideWorkArea = false) where T : class, IPhotinoWindow {
        window.Logger.LogDebug(".MoveTo({location}, {allowOutsideWorkArea})", location, allowOutsideWorkArea);
        window.Logger.LogDebug("Current location: {Location}", window.Location);
        window.Logger.LogDebug("New location: {NewLocation}", location);

        // If the window is outside the work area,
        // recalculate the position and continue.
        //When a window isn't initialized yet, cannot determine screen size.
        if (!allowOutsideWorkArea && window.InstanceHandle != IntPtr.Zero) {
            int horizontalWindowEdge = location.X + window.Width;
            int verticalWindowEdge = location.Y + window.Height;

            int horizontalWorkAreaEdge = window.MainMonitor.WorkArea.Width;
            int verticalWorkAreaEdge = window.MainMonitor.WorkArea.Height;

            bool isOutsideHorizontalWorkArea = horizontalWindowEdge > horizontalWorkAreaEdge;
            bool isOutsideVerticalWorkArea = verticalWindowEdge > verticalWorkAreaEdge;

            var locationInsideWorkArea = new Point(
                isOutsideHorizontalWorkArea ? horizontalWorkAreaEdge - window.Width : location.X,
                isOutsideVerticalWorkArea ? verticalWorkAreaEdge - window.Height : location.Y
            );

            location = locationInsideWorkArea;
        }

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
        if (!OperatingSystem.IsMacOSVersionAtLeast(23)) {
            Size workArea = window.MainMonitor.WorkArea.Size;
            location.Y = location.Y >= 0
                ? location.Y - workArea.Height
                : location.Y;
        }

        // TODO patch this
        window.SetLocation(location);

        return window;
    }

    /// <summary>
    ///     Moves the native window to the specified location on the screen in pixels
    ///     using <see cref="IPhotinoWindow.Left" /> (X) and <see cref="IPhotinoWindow.Top" /> (Y) properties.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <param name="window">Photino window instance</param>
    /// <param name="left">Position from left in pixels</param>
    /// <param name="top">Position from top in pixels</param>
    /// <param name="allowOutsideWorkArea">Whether the window can go off-screen (work area)</param>
    public static T MoveTo<T>(this T window, int left, int top, bool allowOutsideWorkArea = false) where T : class, IPhotinoWindow {
        window.Logger.LogDebug(".MoveTo({left}, {top}, {allowOutsideWorkArea})", left, top, allowOutsideWorkArea);
        return MoveTo(window, new Point(left, top), allowOutsideWorkArea);
    }

    public static T MoveTo<T>(this T window, double left, double top, bool allowOutsideWorkArea = false) where T : class, IPhotinoWindow {
        return MoveTo(window, (int)left, (int)top, allowOutsideWorkArea);
    }
    #endregion

    #region Offset
    /// <summary>
    ///     Moves the native window relative to its current location on the screen in pixels
    ///     using <see cref="IPhotinoWindow.Left" /> (X) and <see cref="IPhotinoWindow.Top" /> (Y) properties.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <param name="window">Photino window instance</param>
    /// <param name="left">Relative offset from left in pixels</param>
    /// <param name="top">Relative offset from top in pixels</param>
    public static T Offset<T>(this T window, int left, int top) where T : class, IPhotinoWindow {
        window.Logger.LogDebug(".Offset({left}, {top})", left, top);
        window.Invoke(() => {
            PhotinoNative.GetPosition(window.InstanceHandle, out int oldLeft, out int oldTop);
            PhotinoNative.SetPosition(window.InstanceHandle, oldLeft + left, oldTop + top);
        });
        return window;
    }
    
    /// <summary>
    ///     Moves the native window relative to its current location on the screen
    ///     using a <see cref="Point" />.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <param name="window">Photino window instance</param>
    /// <param name="offset">Relative offset</param>
    public static T Offset<T>(this T window, Point offset) where T : class, IPhotinoWindow {
        return Offset(window, offset.X, offset.Y);
    }
    
    public static T Offset<T>(this T window, double left, double top) where T : class, IPhotinoWindow {
        return Offset(window, (int)left, (int)top);
    }
    #endregion

    #region SetTransparent
    /// <summary>
    ///     When true, the native window can be displayed with a transparent background.
    ///     Chromeless must be set to true. HTML document's body background must have alpha-based value.
    ///     By default, this is set to false.
    /// </summary>
    public static T SetTransparent<T>(this T window, bool enabled) where T : class, IPhotinoWindow {
        window.Logger.LogDebug(".SetTransparent({Enabled})", enabled);

        if (OperatingSystem.IsWindows()) {
            window.Logger.LogWarning("Transparent can only be set on Windows before the native window is instantiated.");
            return window;
        }

        window.Logger.LogDebug("Invoking PhotinoNative.SetTransparentEnabled({value})", enabled);
        window.Invoke(() => PhotinoNative.SetTransparentEnabled(window.InstanceHandle, enabled));
        return window;
    }
    #endregion

    #region SetContextMenuEnabled
    /// <summary>
    ///     When true, the user can access the browser control's context menu.
    ///     By default, this is set to true.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <param name="window">Photino window instance</param>
    /// <param name="enabled">Whether the context menu should be available</param>
    public static T SetContextMenuEnabled<T>(this T window, bool enabled) where T : class, IPhotinoWindow {
        window.Logger.LogDebug(".SetContextMenuEnabled({Enabled})", enabled);

        window.Invoke(() => {
            PhotinoNative.GetContextMenuEnabled(window.InstanceHandle, out bool isEnabled);
            if (isEnabled == enabled) return;

            PhotinoNative.SetContextMenuEnabled(window.InstanceHandle, enabled);
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
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <param name="window">Photino window instance</param>
    /// <param name="enabled">Whether developer tools should be available</param>
    public static T SetDevToolsEnabled<T>(this T window, bool enabled) where T : class, IPhotinoWindow {
        window.Logger.LogDebug(".SetDevTools({Enabled})", enabled);

        window.Invoke(() => {
            PhotinoNative.GetDevToolsEnabled(window.InstanceHandle, out bool isEnabled);
            if (isEnabled == enabled) return;

            PhotinoNative.SetDevToolsEnabled(window.InstanceHandle, enabled);
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
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <param name="window">Photino window instance</param>
    /// <param name="fullScreen">Whether the window should be fullscreen</param>
    public static T SetFullScreen<T>(this T window, bool fullScreen) where T : class, IPhotinoWindow {
        window.Logger.LogDebug(".SetFullScreen({FullScreen})", fullScreen);

        if (window.FullScreen == fullScreen) {
            window.Logger.LogDebug("Window is already of the same fullscreen state of {fullscreen}", fullScreen);
            return window;
        }

        if (fullScreen) {
            window.Invoke(() => {
                ImmutableArray<Monitor> monitors = MonitorsUtility.GetMonitors(window.InstanceHandle);
                PhotinoNative.GetPosition(window.InstanceHandle, out int left, out int top);
                PhotinoNative.GetSize(window.InstanceHandle, out int width, out int height);

                window.CachedPreFullScreenBounds = new Rectangle(left, top, width, height);
                if (!MonitorsUtility.TryGetCurrentMonitor(monitors, window.CachedPreFullScreenBounds, out Monitor currentMonitor)) {
                    window.Logger.LogError("Failed to get current monitor, defaulting to simple fullscreen call");
                    PhotinoNative.SetFullScreen(window.InstanceHandle, true);
                    return;
                }
                Rectangle currentMonitorArea = currentMonitor.MonitorArea;

                PhotinoNative.SetFullScreen(window.InstanceHandle, true);
                PhotinoNative.SetPosition(window.InstanceHandle, currentMonitorArea.X, currentMonitorArea.Y);
                PhotinoNative.SetSize(window.InstanceHandle, currentMonitorArea.Width, currentMonitorArea.Height);
            });

            return window;
        }

        // Set Fullscreen to false => Restore to previous state
        window.Invoke(() => {
            PhotinoNative.SetFullScreen(window.InstanceHandle, false);
            PhotinoNative.SetPosition(window.InstanceHandle, window.CachedPreFullScreenBounds.X, window.CachedPreFullScreenBounds.Y);
            PhotinoNative.SetSize(window.InstanceHandle, window.CachedPreFullScreenBounds.Width, window.CachedPreFullScreenBounds.Height);
        });

        return window;
    }
    #endregion

    #region SetHeight
    /// <summary>
    ///     Sets the native window <see cref="IPhotinoWindow.Height" /> in pixels.
    ///     Default is 0.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <param name="window">Photino window instance</param>
    /// <param name="height">Height in pixels</param>
    public static T SetHeight<T>(this T window, int height) where T : class, IPhotinoWindow {
        window.Logger.LogDebug(".SetHeight({Height})", height);

        window.Invoke(() => {
            PhotinoNative.GetSize(window.InstanceHandle, out int width, out _);
            PhotinoNative.SetSize(window.InstanceHandle, width, height);
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
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <exception cref="System.ArgumentException">Icon file: {value} does not exist.</exception>
    /// <param name="window"></param>
    /// <param name="iconFilePath">The file path to the icon.</param>
    public static T SetIconFile<T>(this T window, string iconFilePath) where T : class, IPhotinoWindow {
        window.Logger.LogDebug(".SetIconFile({IconFile})", iconFilePath);

        if (window.IconFilePath == iconFilePath) {
            window.Logger.LogDebug("Icon file is already set to {IconFile}, skipping assignment", iconFilePath);
            return window;
        }

        if (!IconFileUtilities.IsValidIconFile(iconFilePath)) {
            window.Logger.LogWarning("Icon file {IconFile} does not exist or is an invalid file path.", iconFilePath);
            return window;
        }
        
        if (window is PhotinoWindow fullWindow) fullWindow.IconFilePath = iconFilePath;
        window.Invoke(() => PhotinoNative.SetIconFile(window.InstanceHandle, iconFilePath));
        return window;
    }
    #endregion

    #region SetLeft
    /// <summary>
    ///     Sets the native window to a new <see cref="IPhotinoWindow.Left" /> (X) coordinate in pixels.
    ///     Default is 0.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <param name="window"></param>
    /// <param name="left">Position in pixels from the left (X).</param>
    public static T SetLeft<T>(this T window, int left) where T : class, IPhotinoWindow {
        window.Logger.LogDebug(".SetLeft({Left})", left);

        window.Invoke(() => {
            PhotinoNative.GetPosition(window.InstanceHandle, out int oldLeft, out int top);
            if (left == oldLeft) return;

            PhotinoNative.SetPosition(window.InstanceHandle, left, top);
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
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <param name="window"></param>
    /// <param name="resizable">Whether the window is resizable</param>
    public static T SetResizable<T>(this T window, bool resizable) where T : class, IPhotinoWindow{
        window.Logger.LogDebug(".SetResizable({Resizable})", resizable);
        window.Invoke(() => PhotinoNative.SetResizable(window.InstanceHandle, resizable));
        return window;
    }
    #endregion

    #region SetSize
    /// <summary>
    ///     Sets the native window Size. This represents the <see cref="IPhotinoWindow.Width" /> and the
    ///     <see cref="IPhotinoWindow.Height" /> of the window in pixels.
    ///     The default Size is 0,0.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <param name="window"></param>
    /// <param name="size">Width &amp; Height</param>
    public static T SetSize<T>(this T window, Size size) where T : class, IPhotinoWindow {
        window.Logger.LogDebug(".SetSize({Size})", size);
        window.Invoke(() => PhotinoNative.SetSize(window.InstanceHandle, size.Width, size.Height));
        return window;
    }

    /// <summary>
    ///     Sets the native window Size. This represents the <see cref="IPhotinoWindow.Width" /> and the
    ///     <see cref="IPhotinoWindow.Height" /> of the window in pixels.
    ///     The default Size is 0,0.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <param name="window"></param>
    /// <param name="width">Width in pixels</param>
    /// <param name="height">Height in pixels</param>
    public static T SetSize<T>(this T window, int width, int height) where T : class, IPhotinoWindow {
        window.Logger.LogDebug(".SetSize({Width}, {Height})", width, height);

        window.Invoke(() => PhotinoNative.SetSize(window.InstanceHandle, width, height));
        return window;
    }
    #endregion
    
    #region SetLocation
    /// <summary>
    ///     Sets the native window <see cref="IPhotinoWindow.Left" /> (X) and <see cref="IPhotinoWindow.Top" /> coordinates (Y)
    ///     in pixels.
    ///     Default is 0,0 that means the window will be aligned to the top-left edge of the screen.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <param name="window"></param>
    /// <param name="location">Location as a <see cref="Point" /></param>
    public static T SetLocation<T>(this T window, Point location) where T : class, IPhotinoWindow {
        window.Logger.LogDebug(".SetLocation({Location})", location);
        window.Invoke(() => {
            PhotinoNative.GetPosition(window.InstanceHandle, out int left, out int top);
            if (left == location.X && top == location.Y) return;

            PhotinoNative.SetPosition(window.InstanceHandle, location.X, location.Y);
        });

        return window;
    }
    #endregion

    /// <summary>
    ///     Sets whether the native window is maximized.
    ///     Default is false.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <param name="window"></param>
    /// <param name="maximized">Whether the window should be maximized.</param>
    public static T SetMaximized<T>(this T window, bool maximized) where T : class, IPhotinoWindow {
        window.Logger.LogDebug(".SetMaximized({Maximized})", maximized);
        window.Invoke(() => PhotinoNative.SetMaximized(window.InstanceHandle, maximized));
        return window;
    }

    ///<summary>Native window maximum Width and Height in pixels.</summary>
    public static T SetMaxSize<T>(this T window, int maxWidth, int maxHeight) where T : class, IPhotinoWindow {
        window.Logger.LogDebug(".SetMaxSize({MaxWidth}, {MaxHeight})", maxWidth, maxHeight);
        window.Invoke(() => PhotinoNative.SetMaxSize(window.InstanceHandle, maxWidth, maxHeight));
        return window;
    }

    ///<summary>Native window maximum Height in pixels.</summary>
    public static T SetMaxHeight<T>(this T window, int maxHeight) where T : class, IPhotinoWindow {
        window.Logger.LogDebug(".SetMaxHeight({MaxHeight})", maxHeight);
        window.MaxHeight = maxHeight;
        return window;
    }

    ///<summary>Native window maximum Width in pixels.</summary>
    public static T SetMaxWidth<T>(this T window, int maxWidth) where T : class, IPhotinoWindow {
        window.Logger.LogDebug(".SetMaxWidth({MaxWidth})", maxWidth);
        window.MaxWidth = maxWidth;
        return window;
    }

    /// <summary>
    ///     Sets whether the native window is minimized (hidden).
    ///     Default is false.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <param name="window"></param>
    /// <param name="minimized">Whether the window should be minimized.</param>
    public static T SetMinimized<T>(this T window, bool minimized) where T : class, IPhotinoWindow {
        window.Logger.LogDebug(".SetMinimized({Minimized})", minimized);
        window.Invoke(() => PhotinoNative.SetMinimized(window.InstanceHandle, minimized));
        return window;
    }

    ///<summary>Native window maximum Width and Height in pixels.</summary>
    public static T SetMinSize<T>(this T window, int minWidth, int minHeight) where T : class, IPhotinoWindow {
        window.Logger.LogDebug(".SetMinSize({MinWidth}, {MinHeight})", minWidth, minHeight);
        window.Invoke(() => PhotinoNative.SetMinSize(window.InstanceHandle, minWidth, minHeight));
        return window;
    }

    ///<summary>Native window maximum Height in pixels.</summary>
    public static T SetMinHeight<T>(this T window, int minHeight) where T : class, IPhotinoWindow {
        window.Logger.LogDebug(".SetMinHeight({MinHeight})", minHeight);
        SetMinSize(window, window.MinWidth, minHeight);
        return window;
    }

    ///<summary>Native window maximum Width in pixels.</summary>
    public static T SetMinWidth<T>(this T window, int minWidth) where T : class, IPhotinoWindow {
        window.Logger.LogDebug(".SetMinWidth({MinWidth})", minWidth);
        SetMinSize(window, minWidth, window.MinHeight);
        return window;
    }

    /// <summary>
    ///     Sets the native window <see cref="IPhotinoWindow.Title" />.
    ///     Default is "Photino".
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <param name="window"></param>
    /// <param name="title">Window title</param>
    public static T SetTitle<T>(this T window, string? title) where T : class, IPhotinoWindow {
        window.Logger.LogDebug(".SetTitle({Title})", title);

        window.Invoke(() => {
            IntPtr ptr = PhotinoNative.GetTitle(window.InstanceHandle);
            string? oldTitle = Marshal.PtrToStringAuto(ptr);
            if (title == oldTitle) return;

            if (OperatingSystem.IsLinux() && title?.Length > 31) title = title[..31];// Due to Linux/Gtk platform limitations, the window title has to be no more than 31 chars
            PhotinoNative.SetTitle(window.InstanceHandle, title ?? string.Empty);
        });

        return window;
    }

    /// <summary>
    ///     Sets the native window <see cref="IPhotinoWindow.Top" /> (Y) coordinate in pixels.
    ///     Default is 0.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <param name="window"></param>
    /// <param name="top">Position in pixels from the top (Y).</param>
    public static T SetTop<T>(this T window, int top) where T : class, IPhotinoWindow {
        window.Logger.LogDebug(".SetTop({Top})", top);
        window.Invoke(() => {
            PhotinoNative.GetPosition(window.InstanceHandle, out int left, out int oldTop);
            if (top == oldTop) return;

            PhotinoNative.SetPosition(window.InstanceHandle, left, top);
        });

        return window;
    }

    /// <summary>
    ///     Sets whether the native window is always at the top of the z-order.
    ///     Default is false.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <param name="window"></param>
    /// <param name="topMost">Whether the window is at the top</param>
    public static T SetTopMost<T>(this T window, bool topMost) where T : class, IPhotinoWindow {
        window.Logger.LogDebug(".SetTopMost({TopMost})", topMost);
        window.Invoke(() => PhotinoNative.SetTopmost(window.InstanceHandle, topMost));
        return window;
    }

    /// <summary>
    ///     Sets the native window width in pixels.
    ///     Default is 0.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <param name="window"></param>
    /// <param name="width">Width in pixels</param>
    public static T SetWidth<T>(this T window, int width) where T : class, IPhotinoWindow {
        window.Logger.LogDebug(".SetWidth({Width})", width);

        window.Invoke(() => {
            PhotinoNative.GetSize(window.InstanceHandle, out _, out int height);
            PhotinoNative.SetSize(window.InstanceHandle, width, height);
        });

        return window;
    }

    /// <summary>
    ///     Sets the native browser control <see cref="IPhotinoWindow.Zoom" />.
    ///     Default is 100.
    /// </summary>
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <param name="window"></param>
    /// <param name="zoom">Zoomlevel as integer</param>
    /// <example>100 = 100%, 50 = 50%</example>
    public static T SetZoom<T>(this T window, int zoom) where T : class, IPhotinoWindow {
        window.Logger.LogDebug(".SetZoom({Zoom})", zoom);
        window.Invoke(() => PhotinoNative.SetZoom(window.InstanceHandle, zoom));
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
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <seealso href="https://docs.microsoft.com/en-us/microsoft-edge/webview2/concepts/distribution" />
    /// <param name="window"></param>
    /// <param name="data">Runtime path for WebView2</param>
    public static T Win32SetWebView2Path<T>(this T window, string data) where T : class, IPhotinoWindow {
        if (OperatingSystem.IsWindows())
            window.Invoke(() => PhotinoNative.SetWebView2RuntimePath_win32(PhotinoWindow.NativeType, data));
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
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    public static T ClearBrowserAutoFill<T>(this T window) where T : class, IPhotinoWindow {
        if (OperatingSystem.IsWindows())
            window.Invoke(() => PhotinoNative.ClearBrowserAutoFill(window.InstanceHandle));
        else
            window.Logger.LogWarning("ClearBrowserAutoFill is only supported on the Windows platform");

        return window;
    }
    
    public static T Resize<T>(this T window, int widthOffset, int heightOffset, ResizeOrigin origin) where T : class, IPhotinoWindow {
        window.Invoke(() => {
            PhotinoNative.GetSize(window.InstanceHandle, out int width, out int height);
            PhotinoNative.GetPosition(window.InstanceHandle, out int x, out int y);

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
                    width += widthOffset;
                    y += heightOffset;
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
            
            PhotinoNative.SetSize(window.InstanceHandle, width, height);
            PhotinoNative.SetPosition(window.InstanceHandle, x, y);
            
        });
        return window;
    }
}
