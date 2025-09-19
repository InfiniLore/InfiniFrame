using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InfiniLore.Photino.NET;

public class PhotinoWindowBuilder : IPhotinoWindowBuilder
{
    private PhotinoNativeParameters _startupParameters = PhotinoNativeParameters.Default;

    #region PhotinoWindowBase properties
    public bool Centered
    {
        get => _startupParameters.CenterOnInitialize; 
        set => _startupParameters.CenterOnInitialize = value;
    }
    
    public bool Chromeless
    {
        get => _startupParameters.Chromeless;
        set => _startupParameters.Chromeless = value;
    }
    
    public bool Transparent
    {
        get => _startupParameters.Transparent;
        set => _startupParameters.Transparent = value;
    }
    
    public bool ContextMenuEnabled 
    {
        get => _startupParameters.ContextMenuEnabled;
        set => _startupParameters.ContextMenuEnabled = value;
    }
    
    public bool DevToolsEnabled 
    {
        get => _startupParameters.DevToolsEnabled;
        set => _startupParameters.DevToolsEnabled = value;
    }
    
    public bool MediaAutoplayEnabled 
    {
        get => _startupParameters.MediaAutoplayEnabled;
        set => _startupParameters.MediaAutoplayEnabled = value;
    }
    
    public string UserAgent 
    {
        get => _startupParameters.UserAgent;
        set => _startupParameters.UserAgent = value;
    }
    
    public bool FileSystemAccessEnabled 
    {
        get => _startupParameters.FileSystemAccessEnabled;
        set => _startupParameters.FileSystemAccessEnabled = value;
    }
    
    public bool WebSecurityEnabled 
    {
        get => _startupParameters.WebSecurityEnabled;
        set => _startupParameters.WebSecurityEnabled = value;
    }

    #endregion
    
    public string StartUrl 
    {
        get => _startupParameters.StartUrl;
        set => _startupParameters.StartUrl = value;
    }

    private PhotinoWindowBuilder() {}

    public static PhotinoWindowBuilder Create()
    {
        return new PhotinoWindowBuilder();
    }

    public IPhotinoWindow Build()
    {
        return new PhotinoWindow(_startupParameters);
    }
    public IPhotinoWindow Build(IServiceProvider provider)
    {
        return new PhotinoWindow(_startupParameters, null, provider.GetService<ILogger<PhotinoWindow>>());
    }
}
