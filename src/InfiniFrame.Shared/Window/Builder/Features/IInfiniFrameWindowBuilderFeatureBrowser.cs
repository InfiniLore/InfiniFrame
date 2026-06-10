// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowBuilderFeatureBrowser {
    bool IsContextMenuEnabled { get; }
    bool IsMediaAutoplayEnabled { get; }
    string? UserAgent { get; }
    bool IsFileSystemAccessEnabled { get; }
    bool IsWebSecurityEnabled { get; }
    bool IsJavascriptClipboardAccessEnabled { get; }
    bool IsMediaStreamEnabled { get; }
    bool IsIgnoreCertificateErrorsEnabled { get; }
    bool GrantBrowserPermissions { get; }
    bool IsSmoothScrollingEnabled { get; }
    string? BrowserControlInitParameters { get; }
    string TemporaryFilesPath { get; }

    void EnableContextMenu(bool enabled);
    void EnableMediaAutoplay(bool enabled);
    void SetUserAgent(string? userAgent);
    void EnableFileSystemAccess(bool enabled);
    void EnableWebSecurity(bool enabled);
    void EnableJavascriptClipboardAccess(bool enabled);
    void EnableMediaStream(bool enabled);
    void EnableIgnoreCertificateErrors(bool enabled);
    void EnableBrowserPermissions(bool enabled);
    void EnableSmoothScrolling(bool enabled);
    void SetBrowserControlInitParameters(string? parameters);
    void SetTemporaryFilesPath(string parameters);
}
