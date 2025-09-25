using InfiniLore.Photino.Blazor.Contracts;
using InfiniLore.Photino.NET;
using Microsoft.Extensions.DependencyInjection;

namespace InfiniLore.Photino.Blazor;
using Microsoft.Extensions.Options;

public class PhotinoBlazorApp(IPhotinoWindowBuilder builder, IPhotinoWebViewManager manager, IServiceProvider provider, IOptions<PhotinoBlazorAppConfiguration> config, IPhotinoJSComponentConfiguration? rootComponentConfiguration = null) {
    public IPhotinoWindowBuilder WindowBuilder => builder;
    public IServiceProvider Provider => provider;
    
    internal void Initialize(RootComponentList rootComponents) {
        builder.RegisterCustomSchemeHandler(PhotinoWebViewManager.BlazorAppScheme, HandleWebRequest)
            .SetUseOsDefaultSize(true)
            .SetUseOsDefaultLocation(true)
            .SetStartUrl(PhotinoWebViewManager.AppBaseUri);

        AppDomain.CurrentDomain.UnhandledException += (_, error) => {
            provider.GetService<IPhotinoWindow>()?.ShowMessage("Fatal exception", error.ExceptionObject.ToString());
        };

        if (rootComponentConfiguration is null) return;

        foreach ((Type, string) component in rootComponents) {
            rootComponentConfiguration.Add(component.Item1, component.Item2);
        }
    }

    public void Run() {
        var window = provider.GetRequiredService<IPhotinoWindow>();
        
        // manager.Navigate(window.StartUrl!);
        window.WaitForClose();
    }

    public Stream? HandleWebRequest(object? sender, string? scheme, string? url, out string? contentType) {
        contentType = null;
        return !string.IsNullOrWhiteSpace(url)
            ? manager.HandleWebRequest(sender, scheme, url, out contentType)
            : null;
    }
}
