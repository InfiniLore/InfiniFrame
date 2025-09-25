namespace InfiniLore.Photino.NET;
public interface IPhotinoWindowBuilder {
    IPhotinoConfiguration Configuration { get; }
    Dictionary<string, NetCustomSchemeDelegate?> CustomSchemeHandlers { get; }
    
    IPhotinoWindow Build();
    IPhotinoWindow Build(IServiceProvider provider);
}
