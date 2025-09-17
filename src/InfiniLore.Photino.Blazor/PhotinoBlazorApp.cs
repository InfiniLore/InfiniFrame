using InfiniLore.Photino.NET;
using Microsoft.Extensions.DependencyInjection;

namespace InfiniLore.Photino.Blazor;

public class PhotinoBlazorApp
{
    /// <summary>
    ///     Gets configuration for the service provider.
    /// </summary>
    public IServiceProvider Services { get; private set; } = null!;

    /// <summary>
    ///     Gets configuration for the root components in the window.
    /// </summary>
    public BlazorWindowRootComponents? RootComponents { get; private set; }

    public PhotinoWindow MainWindow { get; private set; } = null!;

    public PhotinoWebViewManager WindowManager { get; private set; } = null!;

    internal void Initialize(IServiceProvider services, RootComponentList rootComponents)
    {
        Services = services;
        RootComponents = Services.GetService<BlazorWindowRootComponents>();
        MainWindow = Services.GetRequiredService<PhotinoWindow>();
        WindowManager = Services.GetRequiredService<PhotinoWebViewManager>();
        
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
