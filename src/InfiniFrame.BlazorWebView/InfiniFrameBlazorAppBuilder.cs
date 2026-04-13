// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Blazor;
using InfiniFrame.Js;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameBlazorAppBuilder {
    public RootComponentList RootComponents { get; } = new();
    public IServiceCollection Services { get; } = new ServiceCollection();
    public IInfiniFrameWindowBuilder WindowBuilder { get; } = InfiniFrameWindowBuilder.Create();

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    private InfiniFrameBlazorAppBuilder() {}

    public static InfiniFrameBlazorAppBuilder CreateDefault(
        string[]? args = null,
        Action<IInfiniFrameWindowBuilder>? windowBuilder = null
    ) 
        => CreateDefault(null, args, windowBuilder);

    public static InfiniFrameBlazorAppBuilder CreateDefault(IFileProvider? fileProvider, string[]? args = null, Action<IInfiniFrameWindowBuilder>? windowBuilder = null) {
        // We don't use the args for anything right now, but we want to accept them
        // here so that it shows up this way in the project templates.
        var appBuilder = new InfiniFrameBlazorAppBuilder();

        appBuilder.Services.AddOptions<InfiniFrameBlazorAppConfiguration>();

        appBuilder.Services
            .AddSingleton(ConfigureFileProvider(fileProvider))
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

        windowBuilder?.Invoke(appBuilder.WindowBuilder);

        return appBuilder;
    }

    /// <summary>
    /// Configures the file provider to be used by the application.
    /// If a custom <see cref="IFileProvider"/> is provided, that instance will be used.
    /// Otherwise, a default provider will be configured based on the application's "wwwroot" directory.
    /// </summary>
    /// <param name="fileProvider">
    /// An optional <see cref="IFileProvider"/> instance.
    /// </param>
    /// <returns>
    /// An instance of <see cref="IFileProvider"/> that represents either the specified file provider
    /// or the default provider if none is supplied.
    /// </returns>
    private static IFileProvider ConfigureFileProvider(IFileProvider? fileProvider) {
        if (fileProvider is not null) return fileProvider;

        string defaultWwwrootPath = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "wwwroot");
        if (!Directory.Exists(defaultWwwrootPath)) return new NullFileProvider();
        return new PhysicalFileProvider(defaultWwwrootPath);
    }

    public InfiniFrameBlazorAppBuilder WithInfiniFrameWindowBuilder(Action<IInfiniFrameWindowBuilder> windowBuilder) {
        windowBuilder.Invoke(WindowBuilder);
        return this;
    }

    public InfiniFrameBlazorApp Build()
        => Build(Services.BuildServiceProvider());

    public InfiniFrameBlazorApp Build(IServiceProvider serviceProvider) {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        var manager = serviceProvider.GetRequiredService<IInfiniFrameWebViewManager>();

        WindowBuilder
            .RegisterCustomSchemeHandler(InfiniFrameWebViewManager.BlazorAppScheme, manager.HandleWebRequest)
            .SetStartUrl(InfiniFrameWebViewManager.AppBaseUri);

        AppDomain.CurrentDomain.UnhandledException += (_, error) => {
            serviceProvider.GetService<IInfiniFrameWindow>()?.ShowMessage("Fatal exception", error.ExceptionObject.ToString());
        };

        return new InfiniFrameBlazorApp(
            serviceProvider,
            serviceProvider.GetRequiredService<RootComponentList>(),
            serviceProvider.GetService<IInfiniFrameJsComponentConfiguration>()
        );
    }
}
