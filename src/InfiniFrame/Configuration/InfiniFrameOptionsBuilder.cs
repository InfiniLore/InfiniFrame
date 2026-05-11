// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Native;
using InfiniFrame.Utilities;
using System.Runtime.InteropServices;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameOptionsBuilder : IInfiniFrameOptionsBuilder {
    #region Native Parameters
    public string? BrowserControlInitParameters { get; set; }
    public bool Centered { get; set; }
    public bool Chromeless { get; set; }
    public bool ContextMenuEnabled { get; set; } = true;
    public List<string> CustomSchemeNames { get; set; } = new(16);
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
    public string? TemporaryFilesPath { get; set; } = Path.Join(Path.GetTempPath(), "infiniframe");
    
    private string? _title = TitleStringHelper.DefaultTitle;
    public string? Title {
        get => _title;
        set => _title = TitleStringHelper.Validate(value, LimitLinuxWindowTitleLength);
    }
    
    public int Top { get; set; }
    public bool TopMost { get; set; }
    public bool Transparent { get; set; }
    public bool UseOsDefaultLocation { get; set; } = true;
    public bool UseOsDefaultSize { get; set; } = true;
    public string? UserAgent { get; set; } = "InfiniFrame WebView";
    public bool WebSecurityEnabled { get; set; } = true;
    public int Width { get; set; }
    public int Zoom { get; set; } = 100;
    public bool ZoomEnabled { get; set; } = true;
    #endregion

    #region C# Options
    public bool LimitLinuxWindowTitleLength { get; set; } = false;
    public IInfiniFrameWindow? ParentWindow { get; set; } = null;
    
    private List<IInfiniFrameWindow> _childWindows = [];
    public IEnumerable<IInfiniFrameWindow> ChildWindows {
        get => _childWindows;
        set => _childWindows = value.ToList();
    }
    #endregion
    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public InfiniFrameNativeParameters ToNativeParameters() {
        IconFileUtility.TryResolveIconFilePath(IconFilePath, out string? resolvedIconFilePath);

        if (CustomSchemeNames.Count > CustomSchemeNameMemory.MaxCustomSchemeNames)
            throw new InvalidOperationException("Maximum number of custom schemes is 16.");

        IntPtr[] customSchemeNameArray = CustomSchemeNameMemory.Allocate(CustomSchemeNames);

        return new InfiniFrameNativeParameters {
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
            NativeParent = ParentWindow?.InstanceHandle ?? IntPtr.Zero,
            NotificationRegistrationId = NotificationRegistrationId,
            NotificationsEnabled = NotificationsEnabled,
            Resizable = Resizable,
            Size = Marshal.SizeOf<InfiniFrameNativeParameters>(),
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
            WindowIconFile = resolvedIconFilePath,
            Zoom = Zoom,
            ZoomEnabled = ZoomEnabled
        };
    }
}
