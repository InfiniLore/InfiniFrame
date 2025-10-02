using InfiniLore.Photino.NET;
using Microsoft.Extensions.DependencyInjection;

namespace InfiniLore.Photino.Blazor;
public class PhotinoBlazorApp(
    IPhotinoWebViewManager manager,
    IServiceProvider provider,
    RootComponentList rootComponents,
    IPhotinoJsComponentConfiguration? rootComponentConfiguration = null
) {
    public IServiceProvider Provider => provider;
    
    public void Run() {
        var window = provider.GetRequiredService<IPhotinoWindow>();
        
        if (rootComponentConfiguration is not null) {
            foreach ((Type, string) component in rootComponents) {
                rootComponentConfiguration.Add(component.Item1, component.Item2);
            }
        }
        
        window.WaitForClose();
    }

    public Stream? HandleWebRequest(object? sender, string? scheme, string? url, out string? contentType)
        => manager.HandleWebRequest(sender, scheme, url, out contentType);
}
