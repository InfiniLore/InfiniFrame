// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniLore.InfiniFrame.NET;
using InfiniLore.Photino.Native;
using System.Runtime.InteropServices;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class PhotinoConfiguration: IPhotinoConfiguration {
    public string? BrowserControlInitParameters { get; set; }
    public bool Centered { get; set; }
    public bool Chromeless { get; set; }
    public bool ContextMenuEnabled { get; set; } = true;
    public List<string> CustomSchemeNames { get; set; } = new List<string>(16);
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
    public string Title { get; set; } = "Photino";
    public int Top { get; set; }
    public bool TopMost { get; set; }
    public bool Transparent { get; set; }
    public bool UseOsDefaultLocation { get; set; } = true;
    public bool UseOsDefaultSize { get; set; } = true;
    public string? UserAgent { get; set; } = "Photino WebView";
    public bool WebSecurityEnabled { get; set; } = true;
    public int Width { get; set; }
    public int Zoom { get; set; } = 100;
    public bool ZoomEnabled { get; set; } = true;
    
    public PhotinoNativeParameters ToParameters() {
        IntPtr[] customSchemeNameArray = new IntPtr[16];
        for (int i = 0; i < CustomSchemeNames.Count; i++) {
            customSchemeNameArray[i] = Marshal.StringToHGlobalAnsi(CustomSchemeNames[i]);
        }
        
        return new PhotinoNativeParameters {
            BrowserControlInitParameters = BrowserControlInitParameters,
            CenterOnInitialize = Centered,
            Chromeless = Chromeless,
            ContextMenuEnabled = ContextMenuEnabled,
            CustomSchemeNames = customSchemeNameArray,
            DevToolsEnabled = DevToolsEnabled,
            FileSystemAccessEnabled = FileSystemAccessEnabled,
            FullScreen = FullScreen,
            GrantBrowserPermissions = GrantBrowserPermissions,
            Height = Height,
            IgnoreCertificateErrorsEnabled = IgnoreCertificateErrorsEnabled,
            JavascriptClipboardAccessEnabled = JavascriptClipboardAccessEnabled,
            Left = Left,
            MaxHeight = MaxHeight,
            MaxWidth = MaxWidth,
            Maximized = Maximized,
            MediaAutoplayEnabled = MediaAutoplayEnabled,
            MediaStreamEnabled = MediaStreamEnabled,
            MinHeight = MinHeight,
            MinWidth = MinWidth,
            Minimized = Minimized,
            NotificationRegistrationId = NotificationRegistrationId,
            NotificationsEnabled = NotificationsEnabled,
            Resizable = Resizable,
            Size = Marshal.SizeOf<PhotinoNativeParameters>(),
            SmoothScrollingEnabled = SmoothScrollingEnabled,
            StartString = StartString,
            StartUrl = StartUrl,
            TemporaryFilesPath = TemporaryFilesPath,
            Title = Title,
            Top = Top,
            Topmost = TopMost,
            Transparent = Transparent,
            UseOsDefaultLocation = UseOsDefaultLocation,
            UseOsDefaultSize = UseOsDefaultSize,
            UserAgent = UserAgent,
            WebSecurityEnabled = WebSecurityEnabled,
            Width = Width,
            WindowIconFile = IconFilePath,
            Zoom = Zoom,
            ZoomEnabled = ZoomEnabled,
        };
    }
}
