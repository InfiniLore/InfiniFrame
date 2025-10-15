using InfiniLore.Photino.Blazor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using InfiniLore.Photino.NET;

namespace Example.Blazor.Sample;
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
            config.WriteTo.Async(static c => c.Console())
                .MinimumLevel.Debug();
        });

        // register the root component and selector
        appBuilder.RootComponents.Add<App>("app");

        appBuilder.WithPhotinoWindowBuilder(builder => {
            builder
                // .SetTransparent(true)
                .SetChromeless(true)
                // .SetResizable(true)
                .SetIconFile("favicon.ico")
                // .Center()
                // .SetUseOsDefaultSize(true)
                // .SetUseOsDefaultLocation(true);
                // .SetTitle("InfiniLore Photino.Blazor Sample")
                .SetLocation(new Point(100, 100))
                .SetSize(new Size(800, 600))
                // .SetMaxSize(new Size(800, 600))
                // .SetMinSize(new Size(600, 400))
                ;
        });

        PhotinoBlazorApp app = appBuilder.Build();
        
        app.Run();
    }
}
