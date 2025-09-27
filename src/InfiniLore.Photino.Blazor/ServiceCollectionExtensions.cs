using InfiniLore.Photino.NET;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace InfiniLore.Photino.Blazor;
using InfiniLore.Photino.Js;

public static class ServiceCollectionExtensions {
    public static IServiceCollection AddPhotinoBlazorDesktop(this IServiceCollection services, IFileProvider? fileProvider = null, Action<IPhotinoWindowBuilder>? windowBuilder = null) {
        services.AddOptions<PhotinoBlazorAppConfiguration>();

        if (fileProvider is not null) services.AddSingleton(fileProvider);
        else services.AddSingleton<IFileProvider>(static _ => new PhysicalFileProvider(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot")));


        var builder = PhotinoWindowBuilder.Create();
        windowBuilder?.Invoke(builder);
        services.AddSingleton<IPhotinoWindowBuilder>(builder);

        return services
            .AddScoped(static sp => {
                var handler = sp.GetRequiredService<PhotinoHttpHandler>();
                return new HttpClient(handler) { BaseAddress = new Uri(PhotinoWebViewManager.AppBaseUri) };
            })
            .AddScoped<IInfiniWindowJs, InfiniWindowJs>()
            .AddSingleton<IPhotinoWebViewManager, PhotinoWebViewManager>()
            .AddSingleton<IPhotinoJsComponentConfiguration, PhotinoJsComponentConfiguration>()
            .AddSingleton<Dispatcher, PhotinoDispatcher>()
            .AddSingleton<JSComponentConfigurationStore>()
            .AddSingleton<PhotinoBlazorApp>()
            .AddSingleton<PhotinoHttpHandler>()
            .AddSingleton<PhotinoSynchronizationContext>()
            .AddSingleton<IPhotinoWindow>(static provider => provider.GetRequiredService<IPhotinoWindowBuilder>().Build(provider))
            .AddBlazorWebView();
    }

    public static IServiceCollection AddPhotinoWindowBuilder(this IServiceCollection services, Action<IPhotinoWindowBuilder> windowBuilder) {
        var builder = PhotinoWindowBuilder.Create();
        windowBuilder.Invoke(builder);
        services.AddSingleton<IPhotinoWindowBuilder>(builder);
        return services;
    }
}
