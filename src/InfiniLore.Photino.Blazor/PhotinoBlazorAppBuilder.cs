using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace InfiniLore.Photino.Blazor;

public class PhotinoBlazorAppBuilder
{
    public RootComponentList RootComponents { get; } = new RootComponentList();
    public IServiceCollection Services { get; } = new ServiceCollection();

    public static PhotinoBlazorAppBuilder CreateDefault(string[]? args = null)
    {
        return CreateDefault(null, args);
    }

    public static PhotinoBlazorAppBuilder CreateDefault(IFileProvider? fileProvider, string[]? args = null)
    {
        // We don't use the args for anything right now, but we want to accept them
        // here so that it shows up this way in the project templates.
        // var jsRuntime = DefaultWebAssemblyJSRuntime.Instance;
        var builder = new PhotinoBlazorAppBuilder();
        builder.Services.AddPhotinoBlazorDesktop(fileProvider);

        // Right now we don't have conventions or behaviors that are specific to this method
        // however, making this the default for the template allows us to add things like that
        // in the future, while giving `new BlazorDesktopHostBuilder` as an opt-out of opinionated
        // settings.
        return builder;
    }

    public PhotinoBlazorApp Build(Action<IServiceProvider>? serviceProviderOptions = null)
    {
        // register root components with DI container
        // Services.AddSingleton(RootComponents);

        var sp = Services.BuildServiceProvider();
        var app = sp.GetRequiredService<PhotinoBlazorApp>();

        serviceProviderOptions?.Invoke(sp);

        app.Initialize(RootComponents);
        return app;
    }
}