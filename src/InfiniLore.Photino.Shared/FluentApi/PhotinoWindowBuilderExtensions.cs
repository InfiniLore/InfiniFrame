using System.Drawing;

// ReSharper disable once CheckNamespace
namespace InfiniLore.Photino.NET;
using InfiniLore.Photino.Utilities;

public static class PhotinoWindowBuilderExtensions {
    public static T SetMediaAutoplayEnabled<T>(this T builder, bool enable) where T : IPhotinoWindowBuilder {
        builder.Configuration.MediaAutoplayEnabled = enable;
        return builder;
    }

    /// <summary>
    ///     Sets the user agent on the browser control at initialization.
    /// </summary>
    public static T SetUserAgent<T>(this T builder, string userAgent) where T : IPhotinoWindowBuilder {
        builder.Configuration.UserAgent = userAgent;
        return builder;
    }

    /// <summary>
    ///     Sets FileSystemAccessEnabled on the browser control at initialization.
    /// </summary>
    public static T SetFileSystemAccessEnabled<T>(this T builder, bool enable) where T : IPhotinoWindowBuilder {
        builder.Configuration.FileSystemAccessEnabled = enable;
        return builder;
    }

    /// <summary>
    ///     Sets WebSecurityEnabled on the browser control at initialization.
    /// </summary>
    public static T SetWebSecurityEnabled<T>(this T builder, bool enable) where T : IPhotinoWindowBuilder {
        builder.Configuration.WebSecurityEnabled = enable;
        return builder;
    }

    /// <summary>
    ///     Sets JavascriptClipboardAccessEnabled on the browser control at initialization.
    /// </summary>
    public static T SetJavascriptClipboardAccessEnabled<T>(this T builder, bool enable) where T : IPhotinoWindowBuilder {
        builder.Configuration.JavascriptClipboardAccessEnabled = enable;
        return builder;
    }

    /// <summary>
    ///     Sets MediaStreamEnabled on the browser control at initialization.
    /// </summary>
    public static T SetMediaStreamEnabled<T>(this T builder, bool enable) where T : IPhotinoWindowBuilder {
        builder.Configuration.MediaStreamEnabled = enable;
        return builder;
    }

    /// <summary>
    ///     Sets SmoothScrollingEnabled on the browser control at initialization.
    /// </summary>
    public static T SetSmoothScrollingEnabled<T>(this T builder, bool enable = true) where T : IPhotinoWindowBuilder {
        builder.Configuration.SmoothScrollingEnabled = enable;
        return builder;
    }

    /// <summary>
    ///     Sets IgnoreCertificateErrorsEnabled on the browser control at initialization.
    /// </summary>
    public static T SetIgnoreCertificateErrorsEnabled<T>(this T builder, bool enable = true) where T : IPhotinoWindowBuilder {
        builder.Configuration.IgnoreCertificateErrorsEnabled = enable;
        return builder;
    }

    // TODO verify that this is only on windows and else throws an error
    /// <summary>
    ///     Sets NotificationsEnabled on the browser control at initialization.
    /// </summary>
    /// <remarks>
    ///     Only available on Windows.
    /// </remarks>
    /// <exception cref="ApplicationException">
    ///     Thrown if a platform is not Windows.
    /// </exception>
    public static T SetNotificationsEnabled<T>(this T builder, bool enable = true) where T : IPhotinoWindowBuilder {
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
    public static T GrantBrowserPermissions<T>(this T builder, bool enable = true) where T : IPhotinoWindowBuilder {
        builder.Configuration.GrantBrowserPermissions = enable;
        return builder;
    }

    /// <summary>
    ///     Sets IgnoreCertificateErrorsEnabled on the browser control at initialization.
    /// </summary>
    public static T SetHeight<T>(this T builder, int value) where T : IPhotinoWindowBuilder {
        return SetSize(builder, builder.Configuration.Width, value);
    }

    /// <summary>
    ///     Sets IgnoreCertificateErrorsEnabled on the browser control at initialization.
    /// </summary>
    /// <remarks>
    ///     This only works on Windows and Linux.
    /// </remarks>
    /// <value>
    ///     The file path to the icon.
    /// </value>
    public static T SetIconFile<T>(this T builder, string? iconFilePath) where T : IPhotinoWindowBuilder {
        if (!IconFileUtilities.IsValidIconFile(iconFilePath)) return builder;

        builder.Configuration.IconFilePath = iconFilePath;
        return builder;
    }

    /// <summary>
    ///     Sets Location on the browser control at initialization.
    /// </summary>
    public static T SetLocation<T>(this T builder, int left, int top) where T : IPhotinoWindowBuilder {
        builder.Configuration.Left = left;
        builder.Configuration.Top = top;
        
        builder.Configuration.UseOsDefaultLocation = false;
        builder.Configuration.Centered = false;
        return builder;
    }

    /// <summary>
    ///     Sets Location on the browser control at initialization.
    /// </summary>
    public static T SetLocation<T>(this T builder, Point location) where T : IPhotinoWindowBuilder {
        return SetLocation(builder, location.X, location.Y);
    }

    /// <summary>
    ///     Sets Minimized on the browser control at initialization.
    /// </summary>
    public static T SetMinimized<T>(this T builder, bool minimized) where T : IPhotinoWindowBuilder {
        builder.Configuration.Minimized = minimized;
        return builder;
    }

    /// <summary>
    ///     Sets Maximized on the browser control at initialization.
    /// </summary>
    public static T SetMaximized<T>(this T builder, bool maximized) where T : IPhotinoWindowBuilder {
        builder.Configuration.Maximized = maximized;
        return builder;
    }

    /// <summary>
    ///     Sets MaxWidth on the browser control at initialization.
    /// </summary>
    public static T SetMaxWidth<T>(this T builder, int value) where T : IPhotinoWindowBuilder {
        builder.Configuration.MaxWidth = value;
        return builder;
    }

    /// <summary>
    ///     Sets MaxHeight on the browser control at initialization.
    /// </summary>
    public static T SetMaxHeight<T>(this T builder, int value) where T : IPhotinoWindowBuilder {
        builder.Configuration.MaxHeight = value;
        return builder;
    }
    
    public static T SetMaxSize<T>(this T builder, Size maxSize) where T : class, IPhotinoWindowBuilder {
        builder.Configuration.MaxHeight = maxSize.Height;
        builder.Configuration.MaxWidth = maxSize.Width;
        
        return builder;
    }

    /// <summary>
    ///     Sets MinWidth on the browser control at initialization.
    /// </summary>
    public static T SetMinWidth<T>(this T builder, int value) where T : IPhotinoWindowBuilder {
        builder.Configuration.MinWidth = Math.Max(0, value);
        return builder;
    }

    /// <summary>
    ///     Sets MinHeight on the browser control at initialization.
    /// </summary>
    public static T SetMinHeight<T>(this T builder, int value) where T : IPhotinoWindowBuilder {
        builder.Configuration.MinHeight = Math.Max(0, value);
        return builder;
    }
    
    public static T SetMinSize<T>(this T builder, Size minSize) where T : class, IPhotinoWindowBuilder {
        builder.Configuration.MinHeight = minSize.Height;
        builder.Configuration.MinWidth = minSize.Width;
        
        return builder;
    }

    /// <summary>
    ///     Sets FullScreen on the browser control at initialization.
    /// </summary>
    public static T SetFullScreen<T>(this T builder, bool fullscreen) where T : IPhotinoWindowBuilder {
        builder.Configuration.FullScreen = fullscreen;
        return builder;
    }

    /// <summary>
    ///     Sets Resizable on the browser control at initialization.
    /// </summary>
    public static T SetResizable<T>(this T builder, bool resizable) where T : IPhotinoWindowBuilder {
        builder.Configuration.Resizable = resizable;
        return builder;
    }

    /// <summary>
    ///     Sets Width on the browser control at initialization.
    /// </summary>
    public static T SetWidth<T>(this T builder, int value) where T : IPhotinoWindowBuilder {
        return SetSize(builder, value, builder.Configuration.Height);
    }

    /// <summary>
    ///     Sets Size on the browser control at initialization.
    /// </summary>
    public static T SetSize<T>(this T builder, int width, int height) where T : IPhotinoWindowBuilder {
        builder.Configuration.Width = Math.Max(0, width);
        builder.Configuration.Height = Math.Max(0, height);
        
        builder.Configuration.UseOsDefaultSize = false;
        builder.Configuration.Centered = false;
        return builder;
    }

    /// <summary>
    ///     Sets Size on the browser control at initialization.
    /// </summary>
    public static T SetSize<T>(this T builder, Size size) where T : IPhotinoWindowBuilder {
        return SetSize(builder, size.Width, size.Height);
    }

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
    public static T SetBrowserControlInitParameters<T>(this T builder, string? parameters) where T : IPhotinoWindowBuilder {
        builder.Configuration.BrowserControlInitParameters = parameters;
        return builder;
    }

    /// <summary>
    ///     Sets TemporaryFilesPath on the browser control at initialization.
    /// </summary>
    public static T SetTemporaryFilesPath<T>(this T builder, string? path) where T : IPhotinoWindowBuilder {
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
    public static T SetNotificationRegistrationId<T>(this T builder, string? id) where T : IPhotinoWindowBuilder {
        builder.Configuration.NotificationRegistrationId = id;
        return builder;
    }

    /// <summary>
    ///     Sets Title on the browser control at initialization.
    /// </summary>
    public static T SetTitle<T>(this T builder, string? title) where T : IPhotinoWindowBuilder {
        builder.Configuration.Title = title;
        return builder;
    }

    /// <summary>
    ///     Sets TopMost on the browser control at initialization.
    /// </summary>
    public static T SetTopMost<T>(this T builder, bool resizable) where T : IPhotinoWindowBuilder {
        builder.Configuration.TopMost = resizable;
        return builder;
    }

    /// <summary>
    ///     Sets UseOsDefaultLocation on the browser control at initialization.
    /// </summary>
    public static T SetUseOsDefaultLocation<T>(this T builder, bool useOsDefaultLocation) where T : IPhotinoWindowBuilder {
        builder.Configuration.UseOsDefaultLocation = useOsDefaultLocation;
        return builder;
    }

    /// <summary>
    ///     Sets UseOsDefaultSize on the browser control at initialization.
    /// </summary>
    public static T SetUseOsDefaultSize<T>(this T builder, bool useOsDefaultSize) where T : IPhotinoWindowBuilder {
        builder.Configuration.UseOsDefaultSize = useOsDefaultSize;
        return builder;
    }

    public static T SetZoom<T>(this T builder, int zoom) where T : IPhotinoWindowBuilder {
        builder.Configuration.Zoom = zoom;
        return builder;
    }

    public static T SetStartUrl<T>(this T builder, string? url) where T : IPhotinoWindowBuilder {
        builder.Configuration.StartUrl = url;
        return builder;
    }

    public static T SetStartUrl<T>(this T builder, Uri? url) where T : IPhotinoWindowBuilder {
        builder.Configuration.StartUrl = url?.ToString();
        return builder;
    }

    public static T SetStartString<T>(this T builder, string? startString) where T : IPhotinoWindowBuilder {
        builder.Configuration.StartString = startString;
        return builder;
    }

    public static T SetChromeless<T>(this T builder, bool chromeless) where T : IPhotinoWindowBuilder {
        builder.Configuration.Chromeless = chromeless;

        if (!OperatingSystem.IsWindows()) return builder;

        // Overrides the os defaults for you, as it does not work together on windows with chromeless
        builder.Configuration.UseOsDefaultLocation = !chromeless && builder.Configuration.UseOsDefaultLocation;
        builder.Configuration.UseOsDefaultSize = !chromeless && builder.Configuration.UseOsDefaultSize;
        builder.Configuration.Resizable = !chromeless && builder.Configuration.Resizable;

        return builder;
    }

    public static T SetTransparent<T>(this T builder, bool transparent) where T : IPhotinoWindowBuilder {
        builder.Configuration.Transparent = transparent;
        return builder;
    }
    
    /// <summary>
    ///     Sets SetCentered on the browser control at initialization.
    /// </summary>
    public static T Center<T>(this T builder, bool enable = true) where T : IPhotinoWindowBuilder {
        builder.Configuration.Centered = enable;
        return builder;
    }

    /// <summary>
    ///     Registers user-defined custom schemes (other than 'http', 'https' and 'file') and handler methods to receive
    ///     callbacks
    ///     when the native browser control encounters them.
    /// </summary>
    /// <remarks>
    ///     Only 16 custom schemes can be registered before initialization. Additional handlers can be added after
    ///     initialization.
    /// </remarks>
    /// <returns>
    ///     Returns the current <see cref="IPhotinoWindow" /> instance.
    /// </returns>
    /// <param name="builder"></param>
    /// <param name="scheme">The custom scheme</param>
    /// <param name="handler">
    ///     <see cref="EventHandler" />
    /// </param>
    /// <exception cref="ArgumentException">Thrown if no scheme or handler was provided</exception>
    /// <exception cref="ApplicationException">Thrown if more than 16 custom schemes were set</exception>
    public static T RegisterCustomSchemeHandler<T>(this T builder, string scheme, NetCustomSchemeDelegate handler) where T : IPhotinoWindowBuilder {
        if (string.IsNullOrWhiteSpace(scheme)) throw new ArgumentException("A scheme must be provided. (for example 'app' or 'custom'");
        if (handler is null) throw new ArgumentException("A handler (method) with a signature matching NetCustomSchemeDelegate must be supplied.");

        scheme = scheme.ToLower();

        if (builder.CustomSchemeHandlers.Count > 15 && !builder.CustomSchemeHandlers.ContainsKey(scheme)) throw new ApplicationException("No more than 16 custom schemes can be set prior to initialization. Additional handlers can be added after initialization.");
        builder.CustomSchemeHandlers.TryAdd(scheme, null);
        builder.CustomSchemeHandlers[scheme] = handler;

        return builder;
    }
    
    public static T SetLeft<T>(this T builder, int left) where T : IPhotinoWindowBuilder {
        builder.Configuration.Left = left;
        
        builder.Configuration.UseOsDefaultLocation = false;
        builder.Configuration.Centered = false;
        return builder;
    }
    
    public static T SetTop<T>(this T builder, int top) where T : IPhotinoWindowBuilder {
        builder.Configuration.Top = top;
        
        builder.Configuration.UseOsDefaultLocation = false;
        builder.Configuration.Centered = false;
        return builder;
    }
    
    public static T SetContextMenuEnabled<T>(this T builder, bool enabled) where T : IPhotinoWindowBuilder {
        builder.Configuration.ContextMenuEnabled = enabled;
        return builder;
    }
    
    public static T SetDevToolsEnabled<T>(this T builder, bool enabled) where T : IPhotinoWindowBuilder {
        builder.Configuration.DevToolsEnabled = enabled;
        return builder;
    }
}
