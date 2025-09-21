namespace InfiniLore.Photino.NET;
using InfiniLore.Photino.NET.Utilities;
using Microsoft.Extensions.Logging;
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
    private static T Load<T>(this T window, Uri uri) where T : IPhotinoWindow {
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
    public static T Load<T>(this T window, string path) where T : IPhotinoWindow {
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
    public static T LoadRawString<T>(this T window, string content) where T : IPhotinoWindow {
        string shortContent = content.Length > 50 ? string.Concat(content.AsSpan(0, 50), "...") : content;
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
    public static T Center<T>(this T window) where T : IPhotinoWindow {
        window.Logger.LogDebug(".Center()");
        window.Invoke(() => PhotinoNative.Center(window.InstanceHandle));
        return window;
    }
    
    // TODO create a CenterOnMonitor method
    
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
    public static T MoveTo<T>(this T window, Point location, bool allowOutsideWorkArea = false) where T : IPhotinoWindow {
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
        if (PlatformUtilities.IsMacOsPlatform && PlatformUtilities.MacOsVersion?.Major < 23) {
            Size workArea = window.MainMonitor.WorkArea.Size;
            location.Y = location.Y >= 0
                ? location.Y - workArea.Height
                : location.Y;
        }

        SetLocation(location);

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
    public static T MoveTo<T>(this T window, int left, int top, bool allowOutsideWorkArea = false) where T : IPhotinoWindow {
        window.Logger.LogDebug(".MoveTo({left}, {top}, {allowOutsideWorkArea})", left, top, allowOutsideWorkArea);
        return MoveTo(window,new Point(left, top), allowOutsideWorkArea);
    }
    #endregion 
}
