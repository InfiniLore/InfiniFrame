using InfiniLore.Photino.Blazor.Contracts;
using InfiniLore.Photino.NET;

namespace InfiniLore.Photino.Blazor;

public class PhotinoBlazorApp(IServiceProvider services, IPhotinoWindow window, IPhotinoWebViewManager manager, IPhotinoJSComponentConfiguration? rootComponents = null)
{
    /// <summary>
    ///     Gets configuration for the service provider.
    /// </summary>
    public IServiceProvider Services { get; private set; } = services;

    /// <summary>
    ///     Gets configuration for the root components in the window.
    /// </summary>
    public IPhotinoJSComponentConfiguration? RootComponents { get; private set; } = rootComponents;

    public IPhotinoWindow MainWindow { get; private set; } = window;

    public IPhotinoWebViewManager WindowManager { get; private set; } = manager;

    internal void Initialize(RootComponentList rootComponents)
    {
        MainWindow
            .SetTitle("InfiniLore.Photino.Blazor App")
            .SetUseOsDefaultSize(false)
            .SetUseOsDefaultLocation(false)
            .SetWidth(1000)
            .SetHeight(900)
            .SetLeft(450)
            .SetTop(100);

        MainWindow.RegisterCustomSchemeHandler(PhotinoWebViewManager.BlazorAppScheme, HandleWebRequest);

        if (RootComponents is null) return;
        foreach (var component in rootComponents) {
            RootComponents.Add(component.Item1, component.Item2);
        }
    }

    public void Run()
    {
        if (string.IsNullOrWhiteSpace(MainWindow.StartUrl)) MainWindow.StartUrl = "/";

        WindowManager.Navigate(MainWindow.StartUrl);
        MainWindow.WaitForClose();
    }

    public Stream? HandleWebRequest(object? sender, string? scheme, string? url, out string? contentType)
    {
        contentType = null;
        return url is not null 
            ? WindowManager.HandleWebRequest(sender, scheme, url, out contentType) 
            : null;
    }
}
