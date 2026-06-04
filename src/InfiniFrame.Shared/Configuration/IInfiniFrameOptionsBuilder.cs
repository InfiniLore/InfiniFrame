// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameOptionsBuilder {
    #region Native Parameters
    bool Centered { get; set; }
    bool Chromeless { get; set; }
    bool Transparent { get; set; }
    bool ContextMenuEnabled { get; set; }
    bool DevToolsEnabled { get; set; }
    bool MediaAutoplayEnabled { get; set; }
    string? UserAgent { get; set; }
    bool FileSystemAccessEnabled { get; set; }
    bool WebSecurityEnabled { get; set; }
    bool JavascriptClipboardAccessEnabled { get; set; }
    bool MediaStreamEnabled { get; set; }
    bool SmoothScrollingEnabled { get; set; }
    bool IgnoreCertificateErrorsEnabled { get; set; }
    bool NotificationsEnabled { get; set; }
    bool FullScreen { get; set; }
    bool GrantBrowserPermissions { get; set; }
    int Height { get; set; }
    string? IconFilePath { get; set; }
    int Left { get; set; }
    int Top { get; set; }
    bool Maximized { get; set; }
    int MaxWidth { get; set; }
    int MaxHeight { get; set; }
    int MinWidth { get; set; }
    int MinHeight { get; set; }
    bool Minimized { get; set; }
    bool Resizable { get; set; }
    int Width { get; set; }
    string? BrowserControlInitParameters { get; set; }
    string? StartUrl { get; set; }
    string? StartString { get; set; }
    string? TemporaryFilesPath { get; set; }
    string? NotificationRegistrationId { get; set; }
    string? Title { get; set; }
    bool TopMost { get; set; }
    bool UseOsDefaultLocation { get; set; }
    bool UseOsDefaultSize { get; set; }
    List<string> CustomSchemeNames { get; set; }
    int Zoom { get; set; }
    bool ZoomEnabled { get; set; }
    int? RemoteDebuggingPort { get; set; }
    #endregion
    
    #region C# Options
    bool LimitLinuxWindowTitleLength { get; set; }
    IInfiniFrameWindow? ParentWindow { get; set; }
    IEnumerable<IInfiniFrameWindow> ChildWindows { get; set; }
    #endregion
    
    InfiniFrameNativeParameters ToNativeParameters();
}
