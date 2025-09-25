using InfiniLore.Photino.Blazor;
using Microsoft.Extensions.DependencyInjection;

namespace Example.InfiniLore.Photino.Blazor.NativeAOT;
using global::InfiniLore.Photino.NET;

class Program {
    [STAThread] private static void Main(string[] args) {
        var appBuilder = PhotinoBlazorAppBuilder.CreateDefault(args);

        appBuilder.Services
            .AddLogging();

        // register root component and selector
        appBuilder.RootComponents.Add<App>("app");

        PhotinoBlazorApp app = appBuilder.Build();

        // customize window
        app.WindowBuilder
            .SetIconFile("favicon.ico")
            .SetTitle("Photino Blazor Sample");

        // AppDomain.CurrentDomain.UnhandledException += (_, error) => {
        //     app.MainWindow.ShowMessage("Fatal exception", error.ExceptionObject.ToString());
        // };

        app.Run();

    }
}
