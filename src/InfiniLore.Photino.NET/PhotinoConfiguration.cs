// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniLore.Photino.NET;
using System.Runtime.InteropServices;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class PhotinoConfiguration: IPhotinoConfiguration {
    public string? BrowserControlInitParameters { get; set; }
    public bool Centered { get; set; }
    public bool Chromeless { get; set; }
    public bool ContextMenuEnabled { get; set; } = true;
    public string[] CustomSchemeNames { get; set; } = new string[16];
    public bool DevToolsEnabled { get; set; } = true;
    public bool FileSystemAccessEnabled { get; set; } = true;
    public bool FullScreen { get; set; }
    public bool GrantBrowserPermissions { get; set; } = true;
    public int Height { get; set; }
    public string? IconFilePath { get; set; }
    public bool IgnoreCertificateErrorsEnabled { get; set; } = true;
    public bool JavascriptClipboardAccessEnabled { get; set; } = true;
    public int Left { get; set; }
    public int MaxHeight { get; set; } = int.MaxValue;
    public int MaxWidth { get; set; } = int.MaxValue;
    public bool Maximized { get; set; }
    public bool MediaAutoplayEnabled { get; set; } = true;
    public bool MediaStreamEnabled { get; set; } = true;
    public int MinHeight { get; set; }
    public int MinWidth { get; set; }
    public bool Minimized { get; set; }
    public bool NotificationsEnabled { get; set; } = true;
    public string? NotificationRegistrationId { get; set; }
    public bool Resizable { get; set; } = true;
    public bool SmoothScrollingEnabled { get; set; } = true;
    public string? StartString { get; set; }
    public string? StartUrl { get; set; }
    public string? TemporaryFilesPath { get; set; } = Path.Combine(Path.GetTempPath(), "photino");
    public string? Title { get; set; } = "Photino";
    public int Top { get; set; }
    public bool TopMost { get; set; }
    public bool Transparent { get; set; }
    public bool UseOsDefaultLocation { get; set; } = true;
    public bool UseOsDefaultSize { get; set; } = true;
    public string? UserAgent { get; set; } = "Photino WebView";
    public bool WebSecurityEnabled { get; set; } = true;
    public int Width { get; set; }
    public int Zoom { get; set; } = 100;
    
    public PhotinoNativeParameters ToParameters() => new PhotinoNativeParameters {
        CenterOnInitialize = Centered,
        Chromeless = Chromeless,
        Transparent = Transparent,
        ContextMenuEnabled = ContextMenuEnabled,
        DevToolsEnabled = DevToolsEnabled,
        MediaAutoplayEnabled = MediaAutoplayEnabled,
        UserAgent = UserAgent,
        FileSystemAccessEnabled = FileSystemAccessEnabled,
        WebSecurityEnabled = WebSecurityEnabled,
        JavascriptClipboardAccessEnabled = JavascriptClipboardAccessEnabled,
        MediaStreamEnabled = MediaStreamEnabled,
        SmoothScrollingEnabled = SmoothScrollingEnabled,
        IgnoreCertificateErrorsEnabled = IgnoreCertificateErrorsEnabled,
        NotificationsEnabled = NotificationsEnabled,
        FullScreen = FullScreen,
        GrantBrowserPermissions = GrantBrowserPermissions,
        Height = Height,
        WindowIconFile = IconFilePath,
        Left = Left,
        Top = Top,
        Maximized = Maximized,
        MaxWidth = MaxWidth,
        MaxHeight = MaxHeight,
        MinWidth = MinWidth,
        MinHeight = MinHeight,
        Minimized = Minimized,
        Resizable = Resizable,
        Width = Width,
        BrowserControlInitParameters = BrowserControlInitParameters,
        StartUrl = StartUrl,
        StartString = StartString,
        TemporaryFilesPath = TemporaryFilesPath,
        NotificationRegistrationId = NotificationRegistrationId,
        Title = Title,
        UseOsDefaultLocation = UseOsDefaultLocation,
        UseOsDefaultSize = UseOsDefaultSize,
        CustomSchemeNames = CustomSchemeNames,
        Zoom = Zoom,
        Size = Marshal.SizeOf<PhotinoNativeParameters>()
    };
}
