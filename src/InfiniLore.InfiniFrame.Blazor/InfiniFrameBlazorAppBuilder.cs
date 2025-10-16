// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.InfiniFrame.Js;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace InfiniLore.InfiniFrame.Blazor;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameBlazorAppBuilder {
    public RootComponentList RootComponents { get; } = new();
    public IServiceCollection Services { get; } = new ServiceCollection();
    public IInfiniFrameWindowBuilder WindowBuilder { get; } = InfiniFrameWindowBuilder.Create();

    private InfiniFrameBlazorAppBuilder() {}

    public static InfiniFrameBlazorAppBuilder CreateDefault(string[]? args = null, Action<IInfiniFrameWindowBuilder>? windowBuilder = null) => CreateDefault(null, args, windowBuilder);

    public static InfiniFrameBlazorAppBuilder CreateDefault(IFileProvider? fileProvider, string[]? args = null, Action<IInfiniFrameWindowBuilder>? windowBuilder = null) {
        // We don't use the args for anything right now, but we want to accept them
        // here so that it shows up this way in the project templates.
        var appBuilder = new InfiniFrameBlazorAppBuilder();

        appBuilder.Services.AddOptions<InfiniFrameBlazorAppConfiguration>();

        appBuilder.Services
            .AddSingleton(fileProvider ?? new PhysicalFileProvider(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot")))
            .AddScoped(static sp => {
                var handler = sp.GetRequiredService<InfiniFrameHttpHandler>();
                return new HttpClient(handler) { BaseAddress = new Uri(InfiniFrameWebViewManager.AppBaseUri) };
            })
            .AddScoped<IInfiniFrameJs, InfiniFrameJs>()
            .AddSingleton<IInfiniFrameWebViewManager, InfiniFrameWebViewManager>()
            .AddSingleton<IInfiniFrameJsComponentConfiguration, InfiniFrameJsComponentConfiguration>()
            .AddSingleton<Dispatcher, InfiniFrameDispatcher>()
            .AddSingleton<JSComponentConfigurationStore>()
            .AddSingleton<InfiniFrameBlazorApp>()
            .AddSingleton<InfiniFrameHttpHandler>()
            .AddSingleton<InfiniFrameSynchronizationContext>()
            .AddSingleton<IInfiniFrameWindow>(static provider => provider.GetRequiredService<IInfiniFrameWindowBuilder>().Build(provider))
            .AddBlazorWebView()
            .AddSingleton(appBuilder.WindowBuilder)
            .AddSingleton(appBuilder.RootComponents);

        return appBuilder;
    }

    public InfiniFrameBlazorAppBuilder WithInfiniFrameWindowBuilder(Action<IInfiniFrameWindowBuilder> windowBuilder) {
        windowBuilder.Invoke(WindowBuilder);
        return this;
    }

    public InfiniFrameBlazorApp Build() {
        ServiceProvider sp = Services.BuildServiceProvider();
        var manager = sp.GetRequiredService<IInfiniFrameWebViewManager>();

        WindowBuilder
            .RegisterCustomSchemeHandler(InfiniFrameWebViewManager.BlazorAppScheme, manager.HandleWebRequest)
            .SetStartUrl(InfiniFrameWebViewManager.AppBaseUri);

        AppDomain.CurrentDomain.UnhandledException += (_, error) => {
            sp.GetService<IInfiniFrameWindow>()?.ShowMessage("Fatal exception", error.ExceptionObject.ToString());
        };

        return sp.GetRequiredService<InfiniFrameBlazorApp>();
    }
}
