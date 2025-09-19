using InfiniLore.Photino.Blazor.Contracts;
using InfiniLore.Photino.NET;

namespace InfiniLore.Photino.Blazor;

public class PhotinoBlazorApp(IPhotinoWindow window, IPhotinoWebViewManager manager, IPhotinoJSComponentConfiguration? rootComponentConfiguration = null)
{
    internal void Initialize(RootComponentList rootComponents)
    {
        window.RegisterCustomSchemeHandler(PhotinoWebViewManager.BlazorAppScheme, HandleWebRequest);
        
        AppDomain.CurrentDomain.UnhandledException += (_, error) =>
        {
            window.ShowMessage("Fatal exception", error.ExceptionObject.ToString());
        };
        
        if (rootComponentConfiguration is null) return;
        foreach (var component in rootComponents) {
            rootComponentConfiguration.Add(component.Item1, component.Item2);
        }
    }

    public void Run()
    {
        if (string.IsNullOrWhiteSpace(window.StartUrl)) window.StartUrl = "/";

        manager.Navigate(window.StartUrl);
        window.WaitForClose();
    }

    public Stream? HandleWebRequest(object? sender, string? scheme, string? url, out string? contentType)
    {
        contentType = null;
        return !string.IsNullOrWhiteSpace(url) 
            ? manager.HandleWebRequest(sender, scheme, url, out contentType) 
            : null;
    }
}
