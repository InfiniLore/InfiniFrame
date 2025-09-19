namespace InfiniLore.Photino.NET;

public class PhotinoWindowBuilder : IPhotinoWindowBuilder
{
    private PhotinoNativeParameters _startupParameters = PhotinoNativeParameters.Default;
    public PhotinoNativeParameters StartupParameters => _startupParameters;

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

    private PhotinoWindowBuilder() {}

    public PhotinoWindowBuilder Create()
    {
        return new PhotinoWindowBuilder();
    }

    public PhotinoWindow Build()
    {
        return new PhotinoWindow();
    }
}
