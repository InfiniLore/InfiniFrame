using InfiniLore.Photino.Js;
using InfiniLore.Photino.NET;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace InfiniLore.Photino.Blazor;
public class PhotinoBlazorAppBuilder {
    private readonly WebApplicationBuilder _webAppBuilder;
    private bool _addedInteractiveServer;

    private PhotinoBlazorAppBuilder(WebApplicationBuilder webAppBuilder, IPhotinoWindowBuilder windowBuilder) {
        _webAppBuilder = webAppBuilder;
        Window = windowBuilder;
    }

    public static PhotinoBlazorAppBuilder CreateDefault(string[] args) {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Configure for desktop app (no listening ports by default)
        builder.WebHost.UseUrls();// Empty URLs - no network listening

        // Add Blazor services
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services
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

        // Add Photino-specific services
        builder.Services.AddSingleton<IPhotinoEnvironment, PhotinoEnvironment>();

        // Create the window builder    
        var windowBuilder = PhotinoWindowBuilder.Create();
        builder.Services.AddSingleton<IPhotinoWindowBuilder>(windowBuilder);
        builder.Services.AddSingleton(windowBuilder.Configuration);

        return new PhotinoBlazorAppBuilder(builder, windowBuilder);
    }

    /// <summary>
    ///     Access to the underlying WebApplicationBuilder services
    /// </summary>
    public IServiceCollection Services => _webAppBuilder.Services;

    /// <summary>
    ///     Access to configuration
    /// </summary>
    public ConfigurationManager Configuration => _webAppBuilder.Configuration;

    /// <summary>
    ///     Access to hosting configuration
    /// </summary>
    public IWebHostBuilder WebHost => _webAppBuilder.WebHost;

    /// <summary>
    ///     Access to the Photino window builder for window configuration
    /// </summary>
    public IPhotinoWindowBuilder Window { get; }

    /// <summary>
    ///     Configure root components
    /// </summary>
    public PhotinoBlazorAppBuilder AddRootComponent<TComponent>(string selector)
        where TComponent : IComponent {
        // Store root component info for later use
        Services.Configure<RootComponentConfiguration>(config => {
            config.Components.Add(new RootComponentDescriptor {
                ComponentType = typeof(TComponent),
                Selector = selector
            });
        });

        _addedInteractiveServer = true;

        return this;
    }

    public PhotinoBlazorApp Build() {
        // Register Photino configuration as a service
        Services.AddSingleton(Window.Configuration);
        Services.AddSingleton(Window);

        WebApplication webApp = _webAppBuilder.Build();

        // Configure the request pipeline for desktop mode
        webApp.UseStaticFiles();
        webApp.UseAntiforgery();

        // Get the root component configuration
        var rootComponentConfig = Services.BuildServiceProvider()
            .GetService<IOptions<RootComponentConfiguration>>()?.Value;

        // Map the first root component (or a default if none specified)
        if (rootComponentConfig?.Components.Count > 0) {
            var firstComponent = rootComponentConfig.Components[0];
            MethodInfo? mapMethod = typeof(RazorComponentsEndpointRouteBuilderExtensions)
                .GetMethod(nameof(RazorComponentsEndpointRouteBuilderExtensions.MapRazorComponents))
                ?.MakeGenericMethod(firstComponent.ComponentType);

            object? razorComponents = mapMethod?.Invoke(null, new object[] { webApp });

            if (_addedInteractiveServer && razorComponents != null) {
                MethodInfo? addInteractiveMethod = razorComponents.GetType()
                    .GetMethod(nameof(ServerRazorComponentsEndpointConventionBuilderExtensions.AddInteractiveServerRenderMode));
                addInteractiveMethod?.Invoke(null, new[] { razorComponents });
            }
        }

        return new PhotinoBlazorApp(webApp, Window);
    }
}
