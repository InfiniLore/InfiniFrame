// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IBrowserInfiniFrameWindowBuilderFeature : IInfiniFrameWindowBuilderFeature {
    /// <summary>
    ///     Gets whether the context menu is enabled.
    /// </summary>
    bool IsContextMenuEnabled { get; }

    /// <summary>
    ///     Gets whether media autoplay is enabled.
    /// </summary>
    bool IsMediaAutoplayEnabled { get; }

    /// <summary>
    ///     Gets the current user agent string.
    /// </summary>
    string? UserAgent { get; }

    /// <summary>
    ///     Gets whether file system access is enabled.
    /// </summary>
    bool IsFileSystemAccessEnabled { get; }

    /// <summary>
    ///     Gets whether web security is enabled.
    /// </summary>
    bool IsWebSecurityEnabled { get; }

    /// <summary>
    ///     Gets whether JavaScript clipboard access is enabled.
    /// </summary>
    bool IsJavascriptClipboardAccessEnabled { get; }

    /// <summary>
    ///     Gets whether media stream is enabled.
    /// </summary>
    bool IsMediaStreamEnabled { get; }

    /// <summary>
    ///     Gets whether certificate errors are ignored.
    /// </summary>
    bool IsIgnoreCertificateErrorsEnabled { get; }

    /// <summary>
    ///     Gets whether browser permissions are granted.
    /// </summary>
    bool GrantBrowserPermissions { get; }

    /// <summary>
    ///     Gets whether smooth scrolling is enabled.
    /// </summary>
    bool IsSmoothScrollingEnabled { get; }

    /// <summary>
    ///     Gets the browser control initialization parameters.
    /// </summary>
    string? BrowserControlInitParameters { get; }

    /// <summary>
    ///     Gets the temporary files path.
    /// </summary>
    string TemporaryFilesPath { get; }

    /// <summary>
    ///     Gets the fixed-version WebView2 runtime path used on Windows.
    /// </summary>
    string? WebView2RuntimePath { get; }

    /// <summary>
    ///     Enables or disables the context menu.
    /// </summary>
    /// <param name="enabled">Whether the context menu should be enabled.</param>
    void EnableContextMenu(bool enabled);

    /// <summary>
    ///     Enables or disables media autoplay.
    /// </summary>
    /// <param name="enabled">Whether media autoplay should be enabled.</param>
    void EnableMediaAutoplay(bool enabled);

    /// <summary>
    ///     Sets the user agent string.
    /// </summary>
    /// <param name="userAgent">The user agent string to set.</param>
    void SetUserAgent(string? userAgent);

    /// <summary>
    ///     Enables or disables file system access.
    /// </summary>
    /// <param name="enabled">Whether file system access should be enabled.</param>
    void EnableFileSystemAccess(bool enabled);

    /// <summary>
    ///     Enables or disables web security.
    /// </summary>
    /// <param name="enabled">Whether web security should be enabled.</param>
    void EnableWebSecurity(bool enabled);

    /// <summary>
    ///     Enables or disables JavaScript clipboard access.
    /// </summary>
    /// <param name="enabled">Whether JavaScript clipboard access should be enabled.</param>
    void EnableJavascriptClipboardAccess(bool enabled);

    /// <summary>
    ///     Enables or disables media stream.
    /// </summary>
    /// <param name="enabled">Whether media stream should be enabled.</param>
    void EnableMediaStream(bool enabled);

    /// <summary>
    ///     Enables or disables ignoring certificate errors.
    /// </summary>
    /// <param name="enabled">Whether certificate errors should be ignored.</param>
    void EnableIgnoreCertificateErrors(bool enabled);

    /// <summary>
    ///     Enables or disables browser permissions.
    /// </summary>
    /// <param name="enabled">Whether browser permissions should be granted.</param>
    void EnableBrowserPermissions(bool enabled);

    /// <summary>
    ///     Enables or disables smooth scrolling.
    /// </summary>
    /// <param name="enabled">Whether smooth scrolling should be enabled.</param>
    void EnableSmoothScrolling(bool enabled);

    /// <summary>
    ///     Sets the browser control initialization parameters.
    /// </summary>
    /// <param name="parameters">The initialization parameters.</param>
    void SetBrowserControlInitParameters(string? parameters);

    /// <summary>
    ///     Sets the temporary files path.
    /// </summary>
    /// <param name="parameters">The temporary files path.</param>
    void SetTemporaryFilesPath(string parameters);

    /// <summary>
    ///     Sets the fixed-version WebView2 runtime path used when creating the window on Windows.
    /// </summary>
    /// <param name="path">The path to the extracted WebView2 runtime directory.</param>
    void SetWebView2RuntimePath(string path);
}
