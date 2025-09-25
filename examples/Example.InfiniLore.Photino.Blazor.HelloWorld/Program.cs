using InfiniLore.Photino.Blazor;
using InfiniLore.Photino.NET;
using Microsoft.Extensions.DependencyInjection;

namespace Example.InfiniLore.Photino.Blazor.HelloWorld;
public static class Program {
    [STAThread] private static void Main(string[] args) {
        var appBuilder = PhotinoBlazorAppBuilder.CreateDefault(args);
        appBuilder.AddPhotinoWindowBuilder(builder => {
            builder.SetIconFile("favicon.ico")
                .SetTitle("Photino Hello World");
        });

        appBuilder.Services
            .AddLogging();

        // register root component
        appBuilder.RootComponents.Add<App>("app");

        PhotinoBlazorApp app = appBuilder.Build();
        app.Run();
    }
}
