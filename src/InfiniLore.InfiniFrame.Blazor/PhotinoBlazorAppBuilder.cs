using InfiniLore.InfiniFrame.Js;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace InfiniLore.InfiniFrame.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

public class PhotinoBlazorAppBuilder {
    public RootComponentList RootComponents { get; } = new();
    public IServiceCollection Services { get; } = new ServiceCollection();
    public IInfiniFrameWindowBuilder WindowBuilder { get; } = InfiniFrameWindowBuilder.Create();

    private PhotinoBlazorAppBuilder() {}

    public static PhotinoBlazorAppBuilder CreateDefault(string[]? args = null, Action<IInfiniFrameWindowBuilder>? windowBuilder = null) => CreateDefault(null, args, windowBuilder);

    public static PhotinoBlazorAppBuilder CreateDefault(IFileProvider? fileProvider, string[]? args = null, Action<IInfiniFrameWindowBuilder>? windowBuilder = null) {
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
            .AddScoped<IInfiniFrameJs, InfiniWindowJs>()
            .AddSingleton<IPhotinoWebViewManager, PhotinoWebViewManager>()
            .AddSingleton<IPhotinoJsComponentConfiguration, PhotinoJsComponentConfiguration>()
            .AddSingleton<Dispatcher, PhotinoDispatcher>()
            .AddSingleton<JSComponentConfigurationStore>()
            .AddSingleton<PhotinoBlazorApp>()
            .AddSingleton<PhotinoHttpHandler>()
            .AddSingleton<PhotinoSynchronizationContext>()
            .AddSingleton<IInfiniFrameWindow>(static provider => provider.GetRequiredService<IInfiniFrameWindowBuilder>().Build(provider))
            .AddBlazorWebView()
            .AddSingleton(appBuilder.WindowBuilder)
            .AddSingleton(appBuilder.RootComponents);

        return appBuilder;
    }

    public PhotinoBlazorAppBuilder WithPhotinoWindowBuilder(Action<IInfiniFrameWindowBuilder> windowBuilder) {
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
            sp.GetService<IInfiniFrameWindow>()?.ShowMessage("Fatal exception", error.ExceptionObject.ToString());
        };

        return sp.GetRequiredService<PhotinoBlazorApp>();
    }
}
