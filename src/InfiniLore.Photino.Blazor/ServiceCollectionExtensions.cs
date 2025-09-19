using InfiniLore.Photino.Blazor.Contracts;
using InfiniLore.Photino.NET;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace InfiniLore.Photino.Blazor;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPhotinoBlazorDesktop(this IServiceCollection services, Action<IPhotinoWindowBuilder>? builder = null, IFileProvider? fileProvider = null)
    {
        services.AddOptions<PhotinoBlazorAppConfiguration>();
        
        if (fileProvider is not null) services.AddSingleton(fileProvider);
        else services.AddSingleton<IFileProvider>(static _ => new PhysicalFileProvider(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot")));
        
        
        var windowBuilder = PhotinoWindowBuilder.Create();
        builder?.Invoke(windowBuilder);
        services.AddSingleton<IPhotinoWindowBuilder>(windowBuilder);
        
        return services
            .AddScoped(static sp =>
            {
                var handler = sp.GetRequiredService<PhotinoHttpHandler>();
                return new HttpClient(handler) { BaseAddress = new Uri(PhotinoWebViewManager.AppBaseUri) };
            })
            .AddSingleton<IPhotinoWebViewManager, PhotinoWebViewManager>()
            .AddSingleton<IPhotinoJSComponentConfiguration, PhotinoJSComponentConfiguration>()
            .AddSingleton<Dispatcher, PhotinoDispatcher>()
            .AddSingleton<JSComponentConfigurationStore>()
            .AddSingleton<PhotinoBlazorApp>()
            .AddSingleton<PhotinoHttpHandler>()
            .AddSingleton<PhotinoSynchronizationContext>()
            // TODO add the proper builder setup here
            .AddSingleton<IPhotinoWindow>(static provider => provider.GetRequiredService<IPhotinoWindowBuilder>().Build(provider))
            .AddBlazorWebView();
    }
}
