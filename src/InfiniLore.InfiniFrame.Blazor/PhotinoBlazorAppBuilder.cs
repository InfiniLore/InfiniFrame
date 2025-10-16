using InfiniLore.InfiniFrame.NET;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace InfiniLore.InfiniFrame.Blazor;
using InfiniLore.Photino.Js;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

public class PhotinoBlazorAppBuilder {
    public RootComponentList RootComponents { get; } = new();
    public IServiceCollection Services { get; } = new ServiceCollection();
    public IPhotinoWindowBuilder WindowBuilder { get; } = PhotinoWindowBuilder.Create();

    private PhotinoBlazorAppBuilder() {}

    public static PhotinoBlazorAppBuilder CreateDefault(string[]? args = null, Action<IPhotinoWindowBuilder>? windowBuilder = null) => CreateDefault(null, args, windowBuilder);

    public static PhotinoBlazorAppBuilder CreateDefault(IFileProvider? fileProvider, string[]? args = null, Action<IPhotinoWindowBuilder>? windowBuilder = null) {
        // We don't use the args for anything right now, but we want to accept them
        // here so that it shows up this way in the project templates.
        var appBuilder = new PhotinoBlazorAppBuilder();

        appBuilder.Services.AddOptions<PhotinoBlazorAppConfiguration>();

        appBuilder.Services
            .AddSingleton(fileProvider ?? new PhysicalFileProvider(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot")))
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
            .AddBlazorWebView()
            .AddSingleton(appBuilder.WindowBuilder)
            .AddSingleton(appBuilder.RootComponents);

        return appBuilder;
    }

    public PhotinoBlazorAppBuilder WithPhotinoWindowBuilder(Action<IPhotinoWindowBuilder> windowBuilder) {
        windowBuilder.Invoke(WindowBuilder);
        return this;
    }

    public PhotinoBlazorApp Build() {
        ServiceProvider sp = Services.BuildServiceProvider();
        var manager = sp.GetRequiredService<IPhotinoWebViewManager>();

        WindowBuilder
            .RegisterCustomSchemeHandler(PhotinoWebViewManager.BlazorAppScheme, manager.HandleWebRequest)
            .SetStartUrl(PhotinoWebViewManager.AppBaseUri);

        AppDomain.CurrentDomain.UnhandledException += (_, error) => {
            sp.GetService<IPhotinoWindow>()?.ShowMessage("Fatal exception", error.ExceptionObject.ToString());
        };

        return sp.GetRequiredService<PhotinoBlazorApp>();
    }
}
