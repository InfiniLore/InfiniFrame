// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IBrowserInfiniFrameWindowFeature {
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
    ///     Enables or disables the context menu.
    /// </summary>
    /// <param name="enabled">Whether the context menu should be enabled.</param>
    void EnableContextMenu(bool enabled = true);

    /// <summary>
    ///     Enables or disables media autoplay.
    /// </summary>
    /// <param name="enabled">Whether media autoplay should be enabled.</param>
    void EnableMediaAutoplay(bool enabled = true);

    /// <summary>
    ///     Sets the user agent string.
    /// </summary>
    /// <param name="userAgent">The user agent string to set.</param>
    void SetUserAgent(string? userAgent);

    /// <summary>
    ///     Sets the WebView2 runtime path on Windows.
    /// </summary>
    /// <param name="data">The WebView2 runtime path.</param>
    void Win32SetWebView2Path(string data);

    /// <summary>
    ///     Clears the browser auto-fill data.
    /// </summary>
    void ClearBrowserAutoFill();
}
