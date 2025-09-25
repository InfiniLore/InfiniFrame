using InfiniLore.Photino.NET;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace InfiniLore.Photino.Blazor;
public class PhotinoBlazorAppBuilder {
    public RootComponentList RootComponents { get; } = new RootComponentList();
    public IServiceCollection Services { get; } = new ServiceCollection();

    public static PhotinoBlazorAppBuilder CreateDefault(string[]? args = null, Action<IPhotinoWindowBuilder>? windowBuilder = null) {
        return CreateDefault(null, args, windowBuilder);
    }

    public static PhotinoBlazorAppBuilder CreateDefault(IFileProvider? fileProvider, string[]? args = null, Action<IPhotinoWindowBuilder>? windowBuilder = null) {
        // We don't use the args for anything right now, but we want to accept them
        // here so that it shows up this way in the project templates.
        // var jsRuntime = DefaultWebAssemblyJSRuntime.Instance;
        var appBuilder = new PhotinoBlazorAppBuilder();
        appBuilder.Services.AddPhotinoBlazorDesktop(fileProvider: fileProvider, windowBuilder: windowBuilder);

        // Right now we don't have conventions or behaviors that are specific to this method
        // however, making this the default for the template allows us to add things like that
        // in the future, while giving `new BlazorDesktopHostBuilder` as an opt-out of opinionated
        // settings.
        return appBuilder;
    }

    public PhotinoBlazorAppBuilder AddPhotinoWindowBuilder(Action<IPhotinoWindowBuilder> windowBuilder) {
        Services.AddPhotinoWindowBuilder(windowBuilder);
        return new PhotinoBlazorAppBuilder();
    }

    public PhotinoBlazorApp Build() {
        ServiceProvider sp = Services.BuildServiceProvider();
        var app = sp.GetRequiredService<PhotinoBlazorApp>();

        app.Initialize(RootComponents);
        return app;
    }
}
