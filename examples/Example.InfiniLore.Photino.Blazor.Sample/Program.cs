using InfiniLore.Photino.Blazor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using InfiniLore.Photino.NET;

namespace Example.InfiniLore.Photino.Blazor.Sample;
using System.Drawing;

public static class Program {
    [STAThread] 
    private static void Main(string[] args) {
        var appBuilder = PhotinoBlazorAppBuilder.CreateDefault(args);

        appBuilder.Services.AddLogging(config => {
            config.ClearProviders();
            config.AddSerilog();
        });

        appBuilder.Services.AddSerilog(config => {
            config.WriteTo.Console()
                .MinimumLevel.Debug();
        });

        // register root component and selector
        appBuilder.RootComponents.Add<App>("app");

        PhotinoBlazorApp app = appBuilder.Build();

        // customize window
        app.WindowBuilder
            .SetIconFile("favicon.ico")
            .Center()
            .SetUseOsDefaultSize(false)
            .SetTitle("InfiniLore Photino.Blazor Sample")
            .SetSize(new Size(800, 600));
        app.Run();

    }
}
