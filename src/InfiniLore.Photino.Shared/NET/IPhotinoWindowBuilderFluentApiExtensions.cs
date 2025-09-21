using System.Drawing;

namespace InfiniLore.Photino.NET;

// ReSharper disable once InconsistentNaming
public static class IPhotinoWindowBuilderFluentApiExtensions
{
    public static T SetMediaAutoplayEnabled<T>(this T builder, bool enable) where T : IPhotinoWindowBuilder
    {
        builder.MediaAutoplayEnabled = enable;
        return builder;
    }
    
    /// <summary>
    ///     Sets the user agent on the browser control at initialization.
    /// </summary>
    public static T SetUserAgent<T>(this T builder, string userAgent) where T : IPhotinoWindowBuilder
    {
        builder.UserAgent = userAgent;
        return builder;
    }
    
    /// <summary>
    ///     Sets FileSystemAccessEnabled on the browser control at initialization.
    /// </summary>
    public static T SetFileSystemAccessEnabled<T>(this T builder, bool enable) where T : IPhotinoWindowBuilder
    {
        builder.FileSystemAccessEnabled = enable;
        return builder;
    }
    
    /// <summary>
    ///     Sets WebSecurityEnabled on the browser control at initialization.
    /// </summary>
    public static T SetWebSecurityEnabled<T>(this T builder, bool enable) where T : IPhotinoWindowBuilder
    {
        builder.WebSecurityEnabled = enable;
        return builder;
    }
    
    /// <summary>
    ///     Sets JavascriptClipboardAccessEnabled on the browser control at initialization.
    /// </summary>
    public static T SetJavascriptClipboardAccessEnabled<T>(this T builder, bool enable) where T : IPhotinoWindowBuilder
    {
        builder.JavascriptClipboardAccessEnabled = enable;
        return builder;
    }
    
    /// <summary>
    ///     Sets MediaStreamEnabled on the browser control at initialization.
    /// </summary>
    public static T SetMediaStreamEnabled<T>(this T builder, bool enable) where T : IPhotinoWindowBuilder
    {
        builder.MediaStreamEnabled = enable;
        return builder;
    }
    
    /// <summary>
    ///     Sets SmoothScrollingEnabled on the browser control at initialization.
    /// </summary>
    public static T SetSmoothScrollingEnabled<T>(this T builder, bool enable = true) where T : IPhotinoWindowBuilder
    {
        builder.SmoothScrollingEnabled = enable;
        return builder;
    }
    
    /// <summary>
    ///     Sets IgnoreCertificateErrorsEnabled on the browser control at initialization.
    /// </summary>
    public static T SetIgnoreCertificateErrorsEnabled<T>(this T builder, bool enable = true) where T : IPhotinoWindowBuilder
    {
        builder.IgnoreCertificateErrorsEnabled = enable;
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
    public static T SetNotificationsEnabled<T>(this T builder, bool enable = true) where T : IPhotinoWindowBuilder
    {
        builder.NotificationsEnabled = enable;
        return builder;
    }
    
    /// <summary>
    ///     Gets or Sets whether the native browser control grants all requests for access to local resources
    ///     such as the user's camera and microphone. By default, this is set to true.
    /// </summary>
    /// <remarks>
    ///     This only works on Windows.
    /// </remarks>
    public static T GrantBrowserPermissions<T>(this T builder, bool enable = true) where T : IPhotinoWindowBuilder
    {
        builder.GrantBrowserPermissions = enable;
        return builder;
    }
    
    /// <summary>
    ///     Sets IgnoreCertificateErrorsEnabled on the browser control at initialization.
    /// </summary>
    public static T SetHeight<T>(this T builder, int value) where T : IPhotinoWindowBuilder
    {
        builder.Height = Math.Max(0, value);
        return builder;
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
    public static T SetIconFile<T>(this T builder, string? iconFilePath) where T : IPhotinoWindowBuilder
    {
        if (!IconFileUtilities.IsValidIconFile(iconFilePath)) return builder;
        builder.IconFilePath = iconFilePath;
        return builder;
    }
    
    /// <summary>
    ///     Sets Location on the browser control at initialization.
    /// </summary>
    public static T SetLocation<T>(this T builder, int left, int top) where T : IPhotinoWindowBuilder
    {
        builder.Left = left;
        builder.Top = top;
        return builder;
    }
    
    /// <summary>
    ///     Sets Location on the browser control at initialization.
    /// </summary>
    public static T SetLocation<T>(this T builder, Point location) where T : IPhotinoWindowBuilder
    {
        builder.Left = location.X;
        builder.Top = location.Y;
        return builder;
    }
    
    /// <summary>
    ///     Sets Minimized on the browser control at initialization.
    /// </summary>
    public static T SetMinimized<T>(this T builder, bool minimized) where T : IPhotinoWindowBuilder
    {
        builder.Minimized = minimized;
        return builder;
    }
    
    /// <summary>
    ///     Sets Maximized on the browser control at initialization.
    /// </summary>
    public static T SetMaximized<T>(this T builder, bool maximized) where T : IPhotinoWindowBuilder
    {
        builder.Maximized = maximized;
        return builder;
    }
    
    /// <summary>
    ///     Sets MaxWidth on the browser control at initialization.
    /// </summary>
    public static T SetMaxWidth<T>(this T builder, int value) where T : IPhotinoWindowBuilder
    {
        builder.MaxWidth = value;
        return builder;
    }
    
    /// <summary>
    ///     Sets MaxHeight on the browser control at initialization.
    /// </summary>
    public static T SetMaxHeight<T>(this T builder, int value) where T : IPhotinoWindowBuilder
    {
        builder.MaxHeight = value;
        return builder;
    }
    
    /// <summary>
    ///     Sets MinWidth on the browser control at initialization.
    /// </summary>
    public static T SetMinWidth<T>(this T builder, int value) where T : IPhotinoWindowBuilder
    {
        builder.MinWidth = Math.Max(0, value);
        return builder;
    }
    
    /// <summary>
    ///     Sets MinHeight on the browser control at initialization.
    /// </summary>
    public static T SetMinHeight<T>(this T builder, int value) where T : IPhotinoWindowBuilder
    {
        builder.MinHeight = Math.Max(0, value);
        return builder;
    }
    
    /// <summary>
    ///     Sets FullScreen on the browser control at initialization.
    /// </summary>
    public static T SetFullScreen<T>(this T builder, bool fullscreen) where T : IPhotinoWindowBuilder
    {
        builder.FullScreen = fullscreen;
        return builder;
    }
    
    /// <summary>
    ///     Sets Resizable on the browser control at initialization.
    /// </summary>
    public static T SetResizable<T>(this T builder, bool resizable) where T : IPhotinoWindowBuilder
    {
        builder.Resizable = resizable;
        return builder;
    }
    
    /// <summary>
    ///     Sets Width on the browser control at initialization.
    /// </summary>
    public static T SetWidth<T>(this T builder, int value) where T : IPhotinoWindowBuilder
    {
        builder.Width = Math.Max(0, value);
        return builder;
    }
    
    /// <summary>
    ///     Sets Size on the browser control at initialization.
    /// </summary>
    public static T SetSize<T>(this T builder, int width, int height) where T : IPhotinoWindowBuilder
    {
        builder.Width = width;
        builder.Height = height;
        return builder;
    }
    
    /// <summary>
    ///     Sets Size on the browser control at initialization.
    /// </summary>
    public static T SetSize<T>(this T builder, Size size) where T : IPhotinoWindowBuilder
    {
        builder.Width = size.Width;
        builder.Height = size.Height;
        return builder;
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
    public static T SetBrowserControlInitParameters<T>(this T builder, string? parameters) where T : IPhotinoWindowBuilder
    {
        builder.BrowserControlInitParameters = parameters;
        return builder;
    }
    
    /// <summary>
    ///     Sets TemporaryFilesPath on the browser control at initialization.
    /// </summary>
    public static T SetTemporaryFilesPath<T>(this T builder, string? path) where T : IPhotinoWindowBuilder
    {
        builder.TemporaryFilesPath = path;
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
    public static T SetNotificationRegistrationId<T>(this T builder, string? id) where T : IPhotinoWindowBuilder
    {
        builder.NotificationRegistrationId = id;
        return builder;
    }
    
    /// <summary>
    ///     Sets Title on the browser control at initialization.
    /// </summary>
    public static T SetTitle<T>(this T builder, string? title) where T : IPhotinoWindowBuilder
    {
        builder.Title = title;
        return builder;
    }
    
    /// <summary>
    ///     Sets TopMost on the browser control at initialization.
    /// </summary>
    public static T SetTopMost<T>(this T builder, bool resizable) where T : IPhotinoWindowBuilder
    {
        builder.TopMost = resizable;
        return builder;
    }
    
    /// <summary>
    ///     Sets UseOsDefaultLocation on the browser control at initialization.
    /// </summary>
    public static T SetUseOsDefaultLocation<T>(this T builder, bool useOsDefaultLocation) where T : IPhotinoWindowBuilder
    {
        builder.UseOsDefaultLocation = useOsDefaultLocation;
        return builder;
    }
    
    /// <summary>
    ///     Sets UseOsDefaultSize on the browser control at initialization.
    /// </summary>
    public static T SetUseOsDefaultSize<T>(this T builder, bool useOsDefaultSize) where T : IPhotinoWindowBuilder
    {
        builder.UseOsDefaultSize = useOsDefaultSize;
        return builder;
    }
    
    
    
    
    
    
    
    
    /// <summary>
    ///     Sets SetCentered on the browser control at initialization.
    /// </summary>
    public static T Center<T>(this T builder, bool enable = true) where T : IPhotinoWindowBuilder
    {
        builder.Centered = enable;
        return builder;
    }
}
