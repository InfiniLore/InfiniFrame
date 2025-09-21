namespace InfiniLore.Photino.NET;
public interface IPhotinoWindowBuilder {
    IPhotinoConfiguration Configuration { get; }
    
    IPhotinoWindow Build();
    IPhotinoWindow Build(IServiceProvider provider);
}
