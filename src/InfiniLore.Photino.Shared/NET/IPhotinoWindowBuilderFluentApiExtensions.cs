using System.Drawing;

namespace InfiniLore.Photino.NET;

// ReSharper disable once InconsistentNaming
public static class IPhotinoWindowBuilderFluentApiExtensions
{
    public static IPhotinoWindowBuilder SetMediaAutoplayEnabled(this IPhotinoWindowBuilder builder, bool enable)
    {
        builder.MediaAutoplayEnabled = enable;
        return builder;
    }
    
    /// <summary>
    ///     Sets the user agent on the browser control at initialization.
    /// </summary>
    public static IPhotinoWindowBuilder SetUserAgent(this IPhotinoWindowBuilder builder, string userAgent)
    {
        builder.UserAgent = userAgent;
        return builder;
    }
    
    /// <summary>
    ///     Sets FileSystemAccessEnabled on the browser control at initialization.
    /// </summary>
    public static IPhotinoWindowBuilder SetFileSystemAccessEnabled(this IPhotinoWindowBuilder builder, bool enable)
    {
        builder.FileSystemAccessEnabled = enable;
        return builder;
    }
    
    /// <summary>
    ///     Sets WebSecurityEnabled on the browser control at initialization.
    /// </summary>
    public static IPhotinoWindowBuilder SetWebSecurityEnabled(this IPhotinoWindowBuilder builder, bool enable)
    {
        builder.WebSecurityEnabled = enable;
        return builder;
    }
    
    /// <summary>
    ///     Sets JavascriptClipboardAccessEnabled on the browser control at initialization.
    /// </summary>
    public static IPhotinoWindowBuilder SetJavascriptClipboardAccessEnabled(this IPhotinoWindowBuilder builder, bool enable)
    {
        builder.JavascriptClipboardAccessEnabled = enable;
        return builder;
    }
    
    /// <summary>
    ///     Sets MediaStreamEnabled on the browser control at initialization.
    /// </summary>
    public static IPhotinoWindowBuilder SetMediaStreamEnabled(this IPhotinoWindowBuilder builder, bool enable)
    {
        builder.MediaStreamEnabled = enable;
        return builder;
    }
    
    /// <summary>
    ///     Sets SmoothScrollingEnabled on the browser control at initialization.
    /// </summary>
    public static IPhotinoWindowBuilder SetSmoothScrollingEnabled(this IPhotinoWindowBuilder builder, bool enable = true)
    {
        builder.SmoothScrollingEnabled = enable;
        return builder;
    }
    
    /// <summary>
    ///     Sets IgnoreCertificateErrorsEnabled on the browser control at initialization.
    /// </summary>
    public static IPhotinoWindowBuilder SetIgnoreCertificateErrorsEnabled(this IPhotinoWindowBuilder builder, bool enable = true)
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
    public static IPhotinoWindowBuilder SetNotificationsEnabled(this IPhotinoWindowBuilder builder, bool enable = true)
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
    public static IPhotinoWindowBuilder GrantBrowserPermissions(this IPhotinoWindowBuilder builder, bool enable = true)
    {
        builder.GrantBrowserPermissions = enable;
        return builder;
    }
    
    /// <summary>
    ///     Sets IgnoreCertificateErrorsEnabled on the browser control at initialization.
    /// </summary>
    public static IPhotinoWindowBuilder SetHeight(this IPhotinoWindowBuilder builder, int value)
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
    public static IPhotinoWindowBuilder SetIconFile(this IPhotinoWindowBuilder builder, string? iconFilePath)
    {
        if (!IconFileUtilities.IsValidIconFile(iconFilePath)) return builder;
        builder.IconFilePath = iconFilePath;
        return builder;
    }
    
    /// <summary>
    ///     Sets Location on the browser control at initialization.
    /// </summary>
    public static IPhotinoWindowBuilder SetLocation(this IPhotinoWindowBuilder builder, int left, int top)
    {
        builder.Left = left;
        builder.Top = top;
        return builder;
    }
    
    /// <summary>
    ///     Sets Location on the browser control at initialization.
    /// </summary>
    public static IPhotinoWindowBuilder SetLocation(this IPhotinoWindowBuilder builder, Point location)
    {
        builder.Left = location.X;
        builder.Top = location.Y;
        return builder;
    }
    
    /// <summary>
    ///     Sets Minimized on the browser control at initialization.
    /// </summary>
    public static IPhotinoWindowBuilder SetMinimized(this IPhotinoWindowBuilder builder, bool minimized)
    {
        builder.Minimized = minimized;
        return builder;
    }
    
    /// <summary>
    ///     Sets Maximized on the browser control at initialization.
    /// </summary>
    public static IPhotinoWindowBuilder SetMaximized(this IPhotinoWindowBuilder builder, bool maximized)
    {
        builder.Maximized = maximized;
        return builder;
    }
    
    /// <summary>
    ///     Sets MaxWidth on the browser control at initialization.
    /// </summary>
    public static IPhotinoWindowBuilder SetMaxWidth(this IPhotinoWindowBuilder builder, int value)
    {
        builder.MaxWidth = value;
        return builder;
    }
    
    /// <summary>
    ///     Sets MaxHeight on the browser control at initialization.
    /// </summary>
    public static IPhotinoWindowBuilder SetMaxHeight(this IPhotinoWindowBuilder builder, int value)
    {
        builder.MaxHeight = value;
        return builder;
    }
    
    /// <summary>
    ///     Sets MinWidth on the browser control at initialization.
    /// </summary>
    public static IPhotinoWindowBuilder SetMinWidth(this IPhotinoWindowBuilder builder, int value)
    {
        builder.MinWidth = Math.Max(0, value);
        return builder;
    }
    
    /// <summary>
    ///     Sets MinHeight on the browser control at initialization.
    /// </summary>
    public static IPhotinoWindowBuilder SetMinHeight(this IPhotinoWindowBuilder builder, int value)
    {
        builder.MinHeight = Math.Max(0, value);
        return builder;
    }
    
    /// <summary>
    ///     Sets FullScreen on the browser control at initialization.
    /// </summary>
    public static IPhotinoWindowBuilder SetFullScreen(this IPhotinoWindowBuilder builder, bool fullscreen)
    {
        builder.FullScreen = fullscreen;
        return builder;
    }
    
    /// <summary>
    ///     Sets Resizable on the browser control at initialization.
    /// </summary>
    public static IPhotinoWindowBuilder SetResizable(this IPhotinoWindowBuilder builder, bool resizable)
    {
        builder.Resizable = resizable;
        return builder;
    }
    
    /// <summary>
    ///     Sets Width on the browser control at initialization.
    /// </summary>
    public static IPhotinoWindowBuilder SetWidth(this IPhotinoWindowBuilder builder, int value)
    {
        builder.Width = Math.Max(0, value);
        return builder;
    }
    
    /// <summary>
    ///     Sets Size on the browser control at initialization.
    /// </summary>
    public static IPhotinoWindowBuilder SetSize(this IPhotinoWindowBuilder builder, int width, int height)
    {
        builder.Width = width;
        builder.Height = height;
        return builder;
    }
    
    /// <summary>
    ///     Sets Size on the browser control at initialization.
    /// </summary>
    public static IPhotinoWindowBuilder SetSize(this IPhotinoWindowBuilder builder, Size size)
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
    public static IPhotinoWindowBuilder SetBrowserControlInitParameters(this IPhotinoWindowBuilder builder, string? parameters)
    {
        builder.BrowserControlInitParameters = parameters;
        return builder;
    }
    
    /// <summary>
    ///     Sets TemporaryFilesPath on the browser control at initialization.
    /// </summary>
    public static IPhotinoWindowBuilder SetTemporaryFilesPath(this IPhotinoWindowBuilder builder, string? path)
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
    public static IPhotinoWindowBuilder SetNotificationRegistrationId(this IPhotinoWindowBuilder builder, string? id)
    {
        builder.NotificationRegistrationId = id;
        return builder;
    }
    
    /// <summary>
    ///     Sets Title on the browser control at initialization.
    /// </summary>
    public static IPhotinoWindowBuilder SetTitle(this IPhotinoWindowBuilder builder, string? title)
    {
        builder.Title = title;
        return builder;
    }

    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    /// <summary>
    ///     Sets SetCentered on the browser control at initialization.
    /// </summary>
    public static IPhotinoWindowBuilder Center(this IPhotinoWindowBuilder builder, bool enable = true)
    {
        builder.Centered = enable;
        return builder;
    }
}
