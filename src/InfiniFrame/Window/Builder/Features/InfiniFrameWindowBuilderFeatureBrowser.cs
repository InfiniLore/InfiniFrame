// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowBuilderFeatureBrowser : IInfiniFrameWindowBuilderFeatureBrowser {
    public bool IsContextMenuEnabled { get; private set; } = true;
    public bool IsMediaAutoplayEnabled { get; private set; } = true;
    public string? UserAgent { get; private set; } = "InfiniFrame WebView";
    public bool IsFileSystemAccessEnabled { get; private set; } = true;
    public bool IsWebSecurityEnabled { get; private set; } = true;
    public bool IsJavascriptClipboardAccessEnabled { get; private set; } = true;
    public bool IsMediaStreamEnabled { get; private set; } = true;
    public bool IsIgnoreCertificateErrorsEnabled { get; private set; } = true;
    public bool GrantBrowserPermissions { get; private set; } = true;
    public bool IsSmoothScrollingEnabled { get; private set; } = true;
    public string? BrowserControlInitParameters { get; private set; }
    
    public string TemporaryFilesPath { get; private set; } = Path.Join(
        Path.GetTempPath(),
        "infiniframe",
        Environment.ProcessId.ToString()
    );

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public void EnableContextMenu(bool enabled) {
        IsContextMenuEnabled = enabled;
    }
    
    public void EnableMediaAutoplay(bool enabled) {
        IsMediaAutoplayEnabled = enabled;
    }
    
    public void SetUserAgent(string? userAgent) {
        UserAgent = userAgent;
    }
    
    public void EnableFileSystemAccess(bool enabled) {
        IsFileSystemAccessEnabled = enabled;
    }
    
    public void EnableWebSecurity(bool enabled) {
        IsWebSecurityEnabled = enabled;
    }

    public void EnableJavascriptClipboardAccess(bool enabled) {
        IsJavascriptClipboardAccessEnabled = enabled;
    }
    
    public void EnableMediaStream(bool enabled) {
        IsMediaStreamEnabled = enabled;
    }
    
    public void EnableIgnoreCertificateErrors(bool enabled) {
        IsIgnoreCertificateErrorsEnabled = enabled;
    }
    
    public void EnableBrowserPermissions(bool enabled) {
        GrantBrowserPermissions = enabled;
    }
    
    public void EnableSmoothScrolling(bool enabled) {
        IsSmoothScrollingEnabled = enabled;
    }
    
    public void SetBrowserControlInitParameters(string? parameters) {
        BrowserControlInitParameters = parameters;
    }

    public void SetTemporaryFilesPath(string path) {
        TemporaryFilesPath = path;
    }
    
    public void ApplyToNativeParameters(ref InfiniFrameNativeParameters parameters) {
        parameters.ContextMenuEnabled = IsContextMenuEnabled;
        parameters.MediaAutoplayEnabled = IsMediaAutoplayEnabled;
        parameters.UserAgent = UserAgent;
        parameters.FileSystemAccessEnabled = IsFileSystemAccessEnabled;
        parameters.WebSecurityEnabled = IsWebSecurityEnabled;
        parameters.JavascriptClipboardAccessEnabled = IsJavascriptClipboardAccessEnabled;
        parameters.MediaStreamEnabled = IsMediaStreamEnabled;
        parameters.IgnoreCertificateErrorsEnabled = IsIgnoreCertificateErrorsEnabled;
        parameters.GrantBrowserPermissions = GrantBrowserPermissions;
        parameters.SmoothScrollingEnabled = IsSmoothScrollingEnabled;
        parameters.BrowserControlInitParameters = BrowserControlInitParameters;
    }
}