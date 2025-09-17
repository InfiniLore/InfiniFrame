using InfiniLore.Photino.Blazor;
using Microsoft.Extensions.DependencyInjection;

namespace Example.InfiniLore.Photino.Blazor.HelloWorld;

class Program
{
    [STAThread] private static void Main(string[] args)
    {
        var appBuilder = PhotinoBlazorAppBuilder.CreateDefault(args);
        appBuilder.Services
            .AddLogging();

        // register root component
        appBuilder.RootComponents.Add<App>("app");

        var app = appBuilder.Build();

        // customize window
        app.MainWindow
            .SetIconFile("favicon.ico")
            .SetTitle("Photino Hello World");

        AppDomain.CurrentDomain.UnhandledException += (_, error) =>
        {
            app.MainWindow.ShowMessage("Fatal exception", error.ExceptionObject.ToString());
        };

        app.Run();
    }
}
