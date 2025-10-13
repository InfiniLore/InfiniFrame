namespace InfiniLore.Photino.NET;
public interface IPhotinoWindowBase {
    bool Chromeless { get; }
    bool Transparent { get; }
    bool ContextMenuEnabled { get; }
    bool DevToolsEnabled { get; }
    bool FileSystemAccessEnabled { get; }
    bool WebSecurityEnabled { get; }
    bool JavascriptClipboardAccessEnabled { get; }
    bool MediaStreamEnabled { get; }
    bool SmoothScrollingEnabled { get; }
    bool NotificationsEnabled { get; }
    bool IgnoreCertificateErrorsEnabled { get; }
    bool FullScreen { get; }
    bool GrantBrowserPermissions { get; }
    int Height { get; }
    string? IconFilePath { get; internal set; }
    int Left { get; }
    int Top { get; }
    bool Maximized { get; }
    int MaxWidth { get; set; }
    int MaxHeight { get; set; }
    int MinWidth { get; set; }
    int MinHeight { get; set; }
    bool Minimized { get; }
    bool Resizable { get; }
    int Width { get; }
    string? BrowserControlInitParameters { get; }
    string? StartUrl { get; }
    string? StartString { get; }
    string? TemporaryFilesPath { get; }
    string? NotificationRegistrationId { get; }
    string Title { get; }
    bool TopMost { get; }
    int Zoom { get; }
    bool ZoomEnabled { get; }
}
