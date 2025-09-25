using InfiniLore.Photino.Blazor.Contracts;
using InfiniLore.Photino.NET;
using Microsoft.Extensions.DependencyInjection;

namespace InfiniLore.Photino.Blazor;

public class PhotinoBlazorApp(IPhotinoWindowBuilder builder, IPhotinoWebViewManager manager, IServiceProvider provider, IPhotinoJSComponentConfiguration? rootComponentConfiguration = null) {
    internal void Initialize(RootComponentList rootComponents) {
        builder.RegisterCustomSchemeHandler(PhotinoWebViewManager.BlazorAppScheme, HandleWebRequest);

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
        
        manager.Navigate(string.IsNullOrWhiteSpace(window.StartUrl) ? "/" : window.StartUrl);
        window.WaitForClose();
    }

    public Stream? HandleWebRequest(object? sender, string? scheme, string? url, out string? contentType) {
        contentType = null;
        return !string.IsNullOrWhiteSpace(url)
            ? manager.HandleWebRequest(sender, scheme, url, out contentType)
            : null;
    }
}
