namespace InfiniLore.Photino.NET;

public static class PhotinoWindowBuilderFluentApiExtensions
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
    public static IPhotinoWindowBuilder SetIconFile(this IPhotinoWindowBuilder builder, string? iconFilePath)
    {
        if (!IconFileUtilities.IsValidIconFile(iconFilePath)) return builder;
        builder.IconFilePath = iconFilePath;
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
