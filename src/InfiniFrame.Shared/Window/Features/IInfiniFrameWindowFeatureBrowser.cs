// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWindowFeatureBrowser {
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
    
    IInfiniFrameWindow SetContextMenuEnabled(bool enabled);
    IInfiniFrameWindow Win32SetWebView2Path(string data);
    IInfiniFrameWindow ClearBrowserAutoFill();
}
