// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.InfiniFrame;
using InfiniLore.InfiniFrame.Blazor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Drawing;

namespace Example.Blazor.Sample;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class Program {
    [STAThread]
    private static void Main(string[] args) {
        var appBuilder = InfiniFrameBlazorAppBuilder.CreateDefault(args);

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

        appBuilder.WithInfiniFrameWindowBuilder(builder => {
            builder
                // .SetTransparent(true)
                // .SetChromeless(true)
                // .SetResizable(true)
                .SetIconFile("favicon.ico")
                // .Center()
                // .SetUseOsDefaultSize(true)
                // .SetUseOsDefaultLocation(true);
                // .SetTitle("InfiniLore InfiniFrame.Blazor Sample")
                .SetLocation(new Point(100, 100))
                .SetSize(new Size(800, 600))
                // .SetMaxSize(new Size(800, 600))
                // .SetMinSize(new Size(600, 400))
                ;
        });

        InfiniFrameBlazorApp app = appBuilder.Build();

        app.Run();
    }
}
