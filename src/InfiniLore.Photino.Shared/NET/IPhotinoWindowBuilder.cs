namespace InfiniLore.Photino.NET;

public interface IPhotinoWindowBuilder : IPhotinoWindowBase
{
    bool Centered { get; set; }
    new bool Chromeless { get; set; }
    new bool Transparent { get; set; }
    new bool ContextMenuEnabled { get; set; }
    new bool DevToolsEnabled { get; set; }
    bool MediaAutoplayEnabled { get; set; }
    string? UserAgent { get; set; }
    new bool FileSystemAccessEnabled { get; set; }
    new bool WebSecurityEnabled { get; set; }
    new bool JavascriptClipboardAccessEnabled { get; set; }
    new bool MediaStreamEnabled { get; set; }
    new bool SmoothScrollingEnabled { get; set; }
    new bool IgnoreCertificateErrorsEnabled { get; set; }
    new bool NotificationsEnabled { get; set; }
    new bool FullScreen { get; set; }
    new bool GrantBrowserPermissions { get; set; }
    new int Height { get; set; }
    new string? IconFilePath { get; set; }
    new int Left { get; set; }
    new int Top { get; set; }
    new bool Maximized { get; set; }
    new int MaxWidth { get; set; }
    new int MaxHeight { get; set; }
    new int MinWidth { get; set; }
    new int MinHeight { get; set; }
    new bool Minimized { get; set; }
    new bool Resizable { get; set; }
    new int Width { get; set; }
    new string? BrowserControlInitParameters { get; set; }
    new string? StartUrl { get; set; }
    new string? StartString { get; set; }
    new string? TemporaryFilesPath { get; set; }
    new string? NotificationRegistrationId { get; set; }
    new string? Title { get; set; }
    
    IPhotinoWindow Build();
    IPhotinoWindow Build(IServiceProvider provider);
}
