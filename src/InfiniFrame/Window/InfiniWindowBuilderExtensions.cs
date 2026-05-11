// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Utilities;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[SuppressMessage("ReSharper", "ConvertToExtensionBlock")]
public static class InfiniWindowBuilderExtensions {
    /// <summary>
    ///     Sets the media autoplay functionality on the browser control at initialization.
    /// </summary>
    /// <param name="builder">The builder of the window</param>
    /// <param name="enable">
    ///     Determines whether media autoplay should be enabled or disabled.
    ///     Pass true to enable media autoplay, or false to disable it.
    /// </param>
    /// <return>
    ///     Returns the modified builder instance to allow for method chaining.
    /// </return>
    public static T SetMediaAutoplayEnabled<T>(this T builder, bool enable) where T : IInfiniFrameWindowBuilder {
        builder.Configuration.MediaAutoplayEnabled = enable;
        return builder;
    }

    /// <summary>
    ///     Sets the user agent on the browser control at initialization.
    /// </summary>
    public static T SetUserAgent<T>(this T builder, string userAgent) where T : IInfiniFrameWindowBuilder {
        builder.Configuration.UserAgent = userAgent;
        return builder;
    }

    /// <summary>
    ///     Sets FileSystemAccessEnabled on the browser control at initialization.
    /// </summary>
    public static T SetFileSystemAccessEnabled<T>(this T builder, bool enable) where T : IInfiniFrameWindowBuilder {
        builder.Configuration.FileSystemAccessEnabled = enable;
        return builder;
    }

    /// <summary>
    ///     Sets WebSecurityEnabled on the browser control at initialization.
    /// </summary>
    public static T SetWebSecurityEnabled<T>(this T builder, bool enable) where T : IInfiniFrameWindowBuilder {
        builder.Configuration.WebSecurityEnabled = enable;
        return builder;
    }

    /// <summary>
    ///     Sets JavascriptClipboardAccessEnabled on the browser control at initialization.
    /// </summary>
    public static T SetJavascriptClipboardAccessEnabled<T>(this T builder, bool enable) where T : IInfiniFrameWindowBuilder {
        builder.Configuration.JavascriptClipboardAccessEnabled = enable;
        return builder;
    }

    /// <summary>
    ///     Sets MediaStreamEnabled on the browser control at initialization.
    /// </summary>
    public static T SetMediaStreamEnabled<T>(this T builder, bool enable) where T : IInfiniFrameWindowBuilder {
        builder.Configuration.MediaStreamEnabled = enable;
        return builder;
    }

    /// <summary>
    ///     Sets SmoothScrollingEnabled on the browser control at initialization.
    /// </summary>
    public static T SetSmoothScrollingEnabled<T>(this T builder, bool enable = true) where T : IInfiniFrameWindowBuilder {
        builder.Configuration.SmoothScrollingEnabled = enable;
        return builder;
    }

    /// <summary>
    ///     Sets IgnoreCertificateErrorsEnabled on the browser control at initialization.
    /// </summary>
    public static T SetIgnoreCertificateErrorsEnabled<T>(this T builder, bool enable = true) where T : IInfiniFrameWindowBuilder {
        builder.Configuration.IgnoreCertificateErrorsEnabled = enable;
        return builder;
    }

    /// <summary>
    ///     Sets NotificationsEnabled on the browser control at initialization.
    /// </summary>
    /// <remarks>
    ///     Only available on Windows.
    /// </remarks>
    /// <exception cref="ApplicationException">
    ///     Thrown if a platform is not Windows.
    /// </exception>
    public static T SetNotificationsEnabled<T>(this T builder, bool enable = true) where T : IInfiniFrameWindowBuilder {
        builder.Configuration.NotificationsEnabled = enable;
        return builder;
    }

    /// <summary>
    ///     Gets or Sets whether the native browser control grants all requests for access to local resources
    ///     such as the user's camera and microphone. By default, this is set to true.
    /// </summary>
    /// <remarks>
    ///     This only works on Windows.
    /// </remarks>
    public static T GrantBrowserPermissions<T>(this T builder, bool enable = true) where T : IInfiniFrameWindowBuilder {
        builder.Configuration.GrantBrowserPermissions = enable;
        return builder;
    }

    /// <summary>
    ///     Sets IgnoreCertificateErrorsEnabled on the browser control at initialization.
    /// </summary>
    public static T SetHeight<T>(this T builder, int value) where T : IInfiniFrameWindowBuilder => builder.SetSize(builder.Configuration.Width, value);

    /// <summary>
    ///     Sets IgnoreCertificateErrorsEnabled on the browser control at initialization.
    /// </summary>
    /// <remarks>
    ///     This only works on Windows and Linux.
    /// </remarks>
    /// <value>
    ///     The file path to the icon.
    /// </value>
    public static T SetIconFile<T>(this T builder, string? iconFilePath) where T : IInfiniFrameWindowBuilder {
        if (!IconFileUtility.TryResolveIconFilePath(iconFilePath, out string? resolvedIconFilePath)) return builder;

        builder.Configuration.IconFilePath = resolvedIconFilePath;
        return builder;
    }

    /// <summary>
    ///     Sets Location on the browser control at initialization.
    /// </summary>
    public static T SetLocation<T>(this T builder, int left, int top) where T : IInfiniFrameWindowBuilder {
        builder.Configuration.Left = left;
        builder.Configuration.Top = top;

        builder.Configuration.UseOsDefaultLocation = false;
        builder.Configuration.Centered = false;
        return builder;
    }

    /// <summary>
    ///     Sets Location on the browser control at initialization.
    /// </summary>
    public static T SetLocation<T>(this T builder, Point location) where T : IInfiniFrameWindowBuilder => builder.SetLocation(location.X, location.Y);

    /// <summary>
    ///     Sets Minimized on the browser control at initialization.
    /// </summary>
    public static T SetMinimized<T>(this T builder, bool minimized) where T : IInfiniFrameWindowBuilder {
        builder.Configuration.Minimized = minimized;
        return builder;
    }

    /// <summary>
    ///     Sets Maximized on the browser control at initialization.
    /// </summary>
    public static T SetMaximized<T>(this T builder, bool maximized) where T : IInfiniFrameWindowBuilder {
        builder.Configuration.Maximized = maximized;
        return builder;
    }

    /// <summary>
    ///     Sets MaxWidth on the browser control at initialization.
    /// </summary>
    public static T SetMaxWidth<T>(this T builder, int value) where T : IInfiniFrameWindowBuilder {
        builder.Configuration.MaxWidth = value;
        return builder;
    }

    /// <summary>
    ///     Sets MaxHeight on the browser control at initialization.
    /// </summary>
    public static T SetMaxHeight<T>(this T builder, int value) where T : IInfiniFrameWindowBuilder {
        builder.Configuration.MaxHeight = value;
        return builder;
    }

    /// <summary>
    ///     Sets MinWidth on the browser control at initialization.
    /// </summary>
    public static T SetMinWidth<T>(this T builder, int value) where T : IInfiniFrameWindowBuilder {
        builder.Configuration.MinWidth = Math.Max(0, value);
        return builder;
    }

    /// <summary>
    ///     Sets MinHeight on the browser control at initialization.
    /// </summary>
    public static T SetMinHeight<T>(this T builder, int value) where T : IInfiniFrameWindowBuilder {
        builder.Configuration.MinHeight = Math.Max(0, value);
        return builder;
    }

    public static T SetMinSize<T>(this T builder, int width, int height) where T : IInfiniFrameWindowBuilder {
        builder.Configuration.MinHeight = Math.Max(0, height);
        builder.Configuration.MinWidth = Math.Max(0, width);

        return builder;
    }

    public static T SetMinSize<T>(this T builder, Size minSize) where T : IInfiniFrameWindowBuilder => builder.SetMinSize(minSize.Width, minSize.Height);

    /// <summary>
    ///     Sets FullScreen on the browser control at initialization.
    /// </summary>
    public static T SetFullScreen<T>(this T builder, bool fullscreen) where T : IInfiniFrameWindowBuilder {
        builder.Configuration.FullScreen = fullscreen;
        return builder;
    }

    /// <summary>
    ///     Sets Resizable on the browser control at initialization.
    /// </summary>
    public static T SetResizable<T>(this T builder, bool resizable) where T : IInfiniFrameWindowBuilder {
        builder.Configuration.Resizable = resizable;
        return builder;
    }

    /// <summary>
    ///     Sets Width on the browser control at initialization.
    /// </summary>
    public static T SetWidth<T>(this T builder, int value) where T : IInfiniFrameWindowBuilder => builder.SetSize(value, builder.Configuration.Height);

    /// <summary>
    ///     Sets Size on the browser control at initialization.
    /// </summary>
    public static T SetSize<T>(this T builder, int width, int height) where T : IInfiniFrameWindowBuilder {
        builder.Configuration.Width = Math.Max(0, width);
        builder.Configuration.Height = Math.Max(0, height);

        builder.Configuration.UseOsDefaultSize = false;
        builder.Configuration.Centered = false;
        return builder;
    }

    /// <summary>
    ///     Sets Size on the browser control at initialization.
    /// </summary>
    public static T SetSize<T>(this T builder, Size size) where T : IInfiniFrameWindowBuilder => builder.SetSize(size.Width, size.Height);

    /// <summary>
    ///     Sets BrowserControlInitParameters on the browser control at initialization.
    ///     <remarks>
    ///         WINDOWS: WebView2 specific string. Space separated.
    ///         https://peter.sh/experiments/chromium-command-line-switches/
    ///         https://learn.microsoft.com/en-us/dotnet/api/microsoft.web.webview2.core.corewebview2environmentoptions.additionalbrowserarguments?view=webview2-dotnet-1.0.1938.49
    ///         viewFallbackFrom=webview2-dotnet-1.0.1901.177view%3Dwebview2-1.0.1901.177
    ///         https://www.chromium.org/developers/how-tos/run-chromium-with-flags/
    ///         LINUX: Webkit2Gtk specific string. Enter parameter names and values as JSON string.
    ///         e.g. { "set_enable_encrypted_media": true }
    ///         https://webkitgtk.org/reference/webkit2gtk/2.5.1/WebKitSettings.html
    ///         https://lazka.github.io/pgi-docs/WebKit2-4.0/classes/Settings.html
    ///         MAC: Webkit specific string. Enter parameter names and values as JSON string.
    ///         e.g. { "minimumFontSize": 8 }
    ///         https://developer.apple.com/documentation/webkit/wkwebviewconfiguration?language=objc
    ///         https://developer.apple.com/documentation/webkit/wkpreferences?language=objc
    ///     </remarks>
    /// </summary>
    public static T SetBrowserControlInitParameters<T>(this T builder, string? parameters) where T : IInfiniFrameWindowBuilder {
        builder.Configuration.BrowserControlInitParameters = parameters;
        return builder;
    }

    /// <summary>
    ///     Sets TemporaryFilesPath on the browser control at initialization.
    /// </summary>
    public static T SetTemporaryFilesPath<T>(this T builder, string? path) where T : IInfiniFrameWindowBuilder {
        builder.Configuration.TemporaryFilesPath = path;
        return builder;
    }

    /// <summary>
    ///     Sets NotificationRegistrationId on the browser control at initialization.
    /// </summary>
    /// <remarks>
    ///     Only available on Windows.
    /// </remarks>
    /// <exception cref="ApplicationException">
    ///     Thrown if a platform is not Windows.
    /// </exception>
    public static T SetNotificationRegistrationId<T>(this T builder, string? id) where T : IInfiniFrameWindowBuilder {
        builder.Configuration.NotificationRegistrationId = id;
        return builder;
    }

    /// <summary>
    ///     Sets Title on the browser control at initialization.
    /// </summary>
    public static T SetTitle<T>(this T builder, string? title) where T : IInfiniFrameWindowBuilder {
        builder.Configuration.Title = title;
        return builder;
    }

    /// <summary>
    ///     Sets TopMost on the browser control at initialization.
    /// </summary>
    public static T SetTopMost<T>(this T builder, bool topmost) where T : IInfiniFrameWindowBuilder {
        builder.Configuration.TopMost = topmost;
        return builder;
    }

    /// <summary>
    ///     Sets UseOsDefaultLocation on the browser control at initialization.
    /// </summary>
    public static T SetUseOsDefaultLocation<T>(this T builder, bool useOsDefaultLocation) where T : IInfiniFrameWindowBuilder {
        builder.Configuration.UseOsDefaultLocation = useOsDefaultLocation;
        return builder;
    }

    /// <summary>
    ///     Sets UseOsDefaultSize on the browser control at initialization.
    /// </summary>
    public static T SetUseOsDefaultSize<T>(this T builder, bool useOsDefaultSize) where T : IInfiniFrameWindowBuilder {
        builder.Configuration.UseOsDefaultSize = useOsDefaultSize;
        return builder;
    }

    /// <summary>
    ///     Sets the zoom level for the browser control associated with the builder.
    /// </summary>
    /// <param name="builder">The builder of the window</param>
    /// <param name="zoom">
    ///     The desired zoom level. Positive values increase the zoom, negative values decrease it, and 0 resets to the default
    ///     zoom level.
    /// </param>
    /// <return>
    ///     Returns the modified builder instance to allow for method chaining.
    /// </return>
    public static T SetZoom<T>(this T builder, int zoom) where T : IInfiniFrameWindowBuilder {
        builder.Configuration.Zoom = zoom;
        return builder;
    }

    /// <summary>
    ///     Sets the initial URL to be loaded in the browser control upon initialization.
    /// </summary>
    /// <param name="builder">The builder of the window</param>
    /// <param name="url">
    ///     Specifies the URL to set as the starting page.
    ///     Pass a string value representing the desired URL, or null to leave it unset.
    /// </param>
    /// <return>
    ///     Returns the modified builder instance to allow for method chaining.
    /// </return>
    public static T SetStartUrl<T>(this T builder, string? url) where T : IInfiniFrameWindowBuilder {
        builder.Configuration.StartUrl = url;
        return builder;
    }

    /// <summary>
    ///     Sets the starting URL for the browser control in the window.
    /// </summary>
    /// <param name="builder">The builder of the window</param>
    /// <param name="url">
    ///     The URL to be set as the starting location for the browser control.
    ///     Pass a string representing a valid URL or null to leave it unset.
    /// </param>
    /// <return>
    ///     Returns the modified builder instance to enable method chaining.
    /// </return>
    public static T SetStartUrl<T>(this T builder, Uri? url) where T : IInfiniFrameWindowBuilder {
        builder.Configuration.StartUrl = url?.ToString();
        return builder;
    }

    /// <summary>
    ///     Configures the starting string for the browser window initialization.
    /// </summary>
    /// <param name="builder">The builder of the window</param>
    /// <param name="startString">
    ///     The string to be used as the starting configuration. This can be null or a specific value
    ///     that modifies the behavior or appearance of the window during initialization.
    /// </param>
    /// <return>
    ///     Returns the modified builder instance to allow for method chaining.
    /// </return>
    public static T SetStartString<T>(this T builder, string? startString) where T : IInfiniFrameWindowBuilder {
        builder.Configuration.StartString = startString;
        return builder;
    }

    /// <summary>
    ///     Configures whether the browser window should be displayed without any borders or system UI chrome.
    /// </summary>
    /// <param name="builder">The builder of the window</param>
    /// <param name="chromeless">
    ///     Pass true to enable a chromeless (borderless) mode for the browser window, or false to disable it.
    /// </param>
    /// <return>
    ///     Returns the modified builder instance to allow for method chaining.
    /// </return>
    public static T SetChromeless<T>(this T builder, bool chromeless) where T : IInfiniFrameWindowBuilder {
        builder.Configuration.Chromeless = chromeless;

        if (!OperatingSystem.IsWindows()) return builder;

        // Overrides the os defaults for you, as it does not work together on windows with chromeless
        builder.Configuration.UseOsDefaultLocation = !chromeless && builder.Configuration.UseOsDefaultLocation;
        builder.Configuration.UseOsDefaultSize = !chromeless && builder.Configuration.UseOsDefaultSize;
        builder.Configuration.Resizable = !chromeless && builder.Configuration.Resizable;

        return builder;
    }

    /// <summary>
    ///     Configures the browser control to support transparency.
    /// </summary>
    /// <param name="builder">The builder of the window</param>
    /// <param name="transparent">
    ///     Specifies whether the browser control should be transparent.
    ///     Pass true to enable transparency, or false to disable it.
    /// </param>
    /// <return>
    ///     Returns the modified builder instance to allow for method chaining.
    /// </return>
    public static T SetTransparent<T>(this T builder, bool transparent) where T : IInfiniFrameWindowBuilder {
        builder.Configuration.Transparent = transparent;
        return builder;
    }

    /// <summary>
    ///     Sets SetCentered on the browser control at initialization.
    /// </summary>
    public static T Center<T>(this T builder, bool enable = true) where T : IInfiniFrameWindowBuilder {
        builder.Configuration.Centered = enable;
        return builder;
    }

    /// <summary>
    ///     Sets the left position of the window in screen coordinates.
    /// </summary>
    /// <param name="builder">The builder of the window</param>
    /// <param name="left">
    ///     The desired left position of the window. This value represents the distance in pixels
    ///     from the left edge of the screen to the left edge of the window.
    /// </param>
    /// <return>
    ///     Returns the modified builder instance to allow for method chaining.
    /// </return>
    public static T SetLeft<T>(this T builder, int left) where T : IInfiniFrameWindowBuilder {
        builder.Configuration.Left = left;

        builder.Configuration.UseOsDefaultLocation = false;
        builder.Configuration.Centered = false;
        return builder;
    }

    /// <summary>
    ///     Sets the top position of the window on screen in pixels.
    /// </summary>
    /// <param name="builder">The builder of the window</param>
    /// <param name="top">
    ///     The top position in pixels where the window should be placed.
    /// </param>
    /// <return>
    ///     Returns the modified builder instance to allow for method chaining.
    /// </return>
    public static T SetTop<T>(this T builder, int top) where T : IInfiniFrameWindowBuilder {
        builder.Configuration.Top = top;

        builder.Configuration.UseOsDefaultLocation = false;
        builder.Configuration.Centered = false;
        return builder;
    }

    /// <summary>
    ///     Enables or disables the context menu functionality for the browser control.
    /// </summary>
    /// <param name="builder">The builder of the window</param>
    /// <param name="enabled">
    ///     Specifies whether the context menu should be enabled or disabled. Pass true to enable the context menu, or false to
    ///     disable it.
    /// </param>
    /// <return>
    ///     Returns the modified builder instance to support method chaining.
    /// </return>
    public static T SetContextMenuEnabled<T>(this T builder, bool enabled) where T : IInfiniFrameWindowBuilder {
        builder.Configuration.ContextMenuEnabled = enabled;
        return builder;
    }

    /// <summary>
    ///     Enables or disables the DevTools functionality for the browser control.
    /// </summary>
    /// <param name="builder">The builder of the window</param>
    /// <param name="enabled">
    ///     Indicates whether the DevTools should be enabled. Pass true to enable DevTools, or false to disable them.
    /// </param>
    /// <return>
    ///     Returns the modified builder instance to allow for method chaining.
    /// </return>
    public static T SetDevToolsEnabled<T>(this T builder, bool enabled) where T : IInfiniFrameWindowBuilder {
        builder.Configuration.DevToolsEnabled = enabled;
        return builder;
    }

    /// <summary>
    ///     Sets the maximum size of the window by specifying the width and height.
    /// </summary>
    /// <param name="builder">The builder of the window</param>
    /// <param name="width">
    ///     The maximum width of the window, in pixels.
    /// </param>
    /// <param name="height">
    ///     The maximum height of the window, in pixels.
    /// </param>
    /// <return>
    ///     Returns the modified builder instance to allow for method chaining.
    /// </return>
    public static T SetMaxSize<T>(this T builder, int width, int height) where T : IInfiniFrameWindowBuilder {
        builder.Configuration.MaxWidth = width;
        builder.Configuration.MaxHeight = height;

        return builder;
    }

    /// <summary>
    ///     Sets the maximum dimensions for the window.
    /// </summary>
    /// <param name="builder">The builder of the window</param>
    /// <param name="size">
    ///     The maximum allowable width and height of the window, in pixels. Pass a positive non-zero value.
    /// </param>
    /// <return>
    ///     Returns the modified builder instance to allow for method chaining.
    /// </return>
    public static T SetMaxSize<T>(this T builder, Size size) where T : IInfiniFrameWindowBuilder => builder.SetMaxSize(size.Width, size.Height);

    /// <summary>
    ///     Enables or disables the zoom functionality in the browser window.
    /// </summary>
    /// <param name="builder">The builder of the window</param>
    /// <param name="zoomEnabled">
    ///     A boolean value indicating whether zoom functionality should be enabled.
    ///     Pass true to enable zooming functionality, or false to disable it.
    /// </param>
    /// <return>
    ///     Returns the modified builder instance to allow for method chaining.
    /// </return>
    public static T SetZoomEnabled<T>(this T builder, bool zoomEnabled) where T : IInfiniFrameWindowBuilder {
        builder.Configuration.ZoomEnabled = zoomEnabled;
        return builder;
    }
}
