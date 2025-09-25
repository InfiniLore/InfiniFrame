namespace InfiniLore.Photino.NET;
public interface IPhotinoWindowBuilder {
    IPhotinoConfiguration Configuration { get; }
    IPhotinoWindowEvents Events { get; }
    
    Dictionary<string, NetCustomSchemeDelegate?> CustomSchemeHandlers { get; }
    
    IPhotinoWindow Build(IServiceProvider? provider = null);
}
