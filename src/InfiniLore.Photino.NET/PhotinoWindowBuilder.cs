using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InfiniLore.Photino.NET;

public class PhotinoWindowBuilder : IPhotinoWindowBuilder
{
    #region PhotinoWindowBase properties
    
    public bool Centered { get; set; }
    public bool Chromeless { get; set; } 
    public bool Transparent { get; set; }
    public bool ContextMenuEnabled { get; set; } = true;
    public bool DevToolsEnabled { get; set; } = true;
    public bool MediaAutoplayEnabled { get; set; } = true;
    public string UserAgent { get; set; } = "Photino WebView";
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

    #endregion
    
    public string StartUrl { get; set; } = PhotinoNativeParameters.Default.StartString;    

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
            WindowIconFile = IconFilePath,
            
            StartString = StartUrl,
        };
        return parameters;
    }

    public IPhotinoWindow Build()
    {
        return new PhotinoWindow(GetParameters());
    }
    public IPhotinoWindow Build(IServiceProvider provider)
    {
        return new PhotinoWindow(GetParameters(), null, provider.GetService<ILogger<PhotinoWindow>>());
    }
}
