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
}
