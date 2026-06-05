// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.NativeBridge.Parameters;
using InfiniFrame.Utilities;
using System.Runtime.InteropServices;
using RemoteDebuggingUtility = InfiniFrame.Debugging.RemoteDebuggingUtility;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameOptionsBuilder : IInfiniFrameOptionsBuilder {
    #region Native Parameters
    private string? _browserControlInitParameters;
    public string? BrowserControlInitParameters {
        get => _browserControlInitParameters;
        set => _browserControlInitParameters = value;
    }
    public bool Centered { get; set; }
    public bool Chromeless { get; set; }
    public bool ContextMenuEnabled { get; set; } = true;
    public List<string> CustomSchemeNames { get; set; } = new(16);
    public bool DevToolsEnabled { get; set; } = true;
    public bool WebInspectorEnabled { get; set; }
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
    public string? TemporaryFilesPath { get; set; } = Path.Join(
        Path.GetTempPath(),
        "infiniframe",
        Environment.ProcessId.ToString());
    
    private string? _title = TitleStringUtility.DefaultTitle;
    public string? Title {
        get => _title;
        set => _title = TitleStringUtility.Validate(value, LimitLinuxWindowTitleLength);
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
    private int? _remoteDebuggingPort;
    public int? RemoteDebuggingPort {
        get => _remoteDebuggingPort;
        set => _remoteDebuggingPort = RemoteDebuggingUtility.NormalizePort(value, nameof(RemoteDebuggingPort));
    }
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
        int? normalizedRemoteDebuggingPort = RemoteDebuggingUtility.NormalizePort(RemoteDebuggingPort, nameof(RemoteDebuggingPort));
        RemoteDebuggingUtility.EnsureSupportedPlatform(normalizedRemoteDebuggingPort);
        if (WebInspectorEnabled) {
            WebInspectorUtility.ThrowIfUnsupported();
        }

        IconFileUtility.TryResolveIconFilePath(IconFilePath, out string? resolvedIconFilePath);

        if (CustomSchemeNames.Count > CustomSchemeNameMemory.MaxCustomSchemeNames)
            throw new InvalidOperationException("Maximum number of custom schemes is 16.");

        IntPtr[] customSchemeNameArray = CustomSchemeNameMemory.Allocate(CustomSchemeNames);

        string? effectiveBrowserControlInitParameters = RemoteDebuggingUtility.ComposeBrowserControlInitParameters(
            BrowserControlInitParameters,
            normalizedRemoteDebuggingPort);

        return new InfiniFrameNativeParameters {
            BrowserControlInitParameters = effectiveBrowserControlInitParameters,
            CenterOnInitialize = Centered,
            Chromeless = Chromeless,
            ContextMenuEnabled = ContextMenuEnabled,
            CustomSchemeNames = customSchemeNameArray,
            DevToolsEnabled = DevToolsEnabled,
            WebInspectorEnabled = WebInspectorEnabled,
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
            RemoteDebuggingPort = normalizedRemoteDebuggingPort ?? 0,
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