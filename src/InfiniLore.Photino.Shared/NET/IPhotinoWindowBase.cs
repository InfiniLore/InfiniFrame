namespace InfiniLore.Photino.NET;

public interface IPhotinoWindowBase
{
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
    string? IconFilePath { get; }
}
