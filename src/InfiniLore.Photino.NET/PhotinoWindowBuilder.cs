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
