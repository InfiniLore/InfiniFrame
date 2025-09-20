using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InfiniLore.Photino.NET;

public class PhotinoWindowBuilder : IPhotinoWindowBuilder
{
    public bool UseDefaultLogger { get; set; } = true;
    
    #region PhotinoWindowBase properties
    
    public bool Centered { get; set; }
    public bool Chromeless { get; set; } 
    public bool Transparent { get; set; }
    public bool ContextMenuEnabled { get; set; } = true;
    public bool DevToolsEnabled { get; set; } = true;
    public bool MediaAutoplayEnabled { get; set; } = true;
    public string? UserAgent { get; set; } = "Photino WebView";
    public bool FileSystemAccessEnabled { get; set; } = true;
    public bool WebSecurityEnabled { get; set; } = true;
    public bool JavascriptClipboardAccessEnabled { get; set; } = true;
    public bool MediaStreamEnabled { get; set; } = true;
    public bool SmoothScrollingEnabled { get; set; } = true;
    public bool IgnoreCertificateErrorsEnabled { get; set; } = true;
    public bool NotificationsEnabled { get; set; } = true;
    public bool FullScreen { get; set; }
    public bool GrantBrowserPermissions { get; set; } = true;
    public int Height { get; set; }
    public string? IconFilePath { get; set; }
    public int Left { get; set; }
    public int Top { get; set; }
    public bool Maximized { get; set; }
    public int MaxWidth { get; set; } = int.MaxValue;
    public int MaxHeight { get; set; } = int.MaxValue;
    public int MinWidth { get; set; }
    public int MinHeight { get; set; }
    public bool Minimized { get; set; }
    public bool Resizable { get; set; }
    public int Width { get; set; }
    public string? BrowserControlInitParameters { get; set; }
    public string? StartUrl { get; set; }    
    public string? StartString { get; set; }  
    public string? TemporaryFilesPath { get; set; } = Path.Combine(Path.GetTempPath(), "photino");
    public string? NotificationRegistrationId { get; set; }
    public string? Title { get; set; }
    
    #endregion
      

    private PhotinoWindowBuilder() {}

    public static PhotinoWindowBuilder Create()
    {
        return new PhotinoWindowBuilder();
    }

    private PhotinoNativeParameters GetParameters()
    {
        var parameters = new PhotinoNativeParameters
        {
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
        };
        return parameters;
    }

    private ILogger<PhotinoWindow> GetDefaultLogger()
    {
        if (!UseDefaultLogger) return LoggerFactory.Create(config => {
            config.ClearProviders(); // Remove default console logger
        }).CreateLogger<PhotinoWindow>();
        
        return  LoggerFactory.Create(config => {
            config.AddConsole().SetMinimumLevel(LogLevel.Debug);
        }).CreateLogger<PhotinoWindow>();
    }

    public IPhotinoWindow Build()
    {
        return new PhotinoWindow(GetParameters(), GetDefaultLogger());
    }
    public IPhotinoWindow Build(IServiceProvider provider)
    {
        return new PhotinoWindow(GetParameters(), provider.GetService<ILogger<PhotinoWindow>>() ?? GetDefaultLogger());
    }
}
