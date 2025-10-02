namespace InfiniLore.Photino.NET;
public interface IPhotinoWindowBuilder {
    bool UseDefaultLogger { get; set; }
    
    IPhotinoConfiguration Configuration { get; }
    IPhotinoWindowEvents Events { get; }
    IPhotinoWindowMessageHandlers MessageHandlers { get; }
    
    Dictionary<string, NetCustomSchemeDelegate?> CustomSchemeHandlers { get; }
    
    IPhotinoWindow Build(IServiceProvider? provider = null);
}
