// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.BlazorWebView;
using InfiniFrameExample.BlazorWebView.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Drawing;

namespace InfiniFrameExample.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class Program {
    [STAThread]
    private static void Main(string[] args) {
#pragma warning disable CS0618 // Type or member is obsolete
        var appBuilder = InfiniFrameBlazorAppBuilder.CreateDefault(args);
#pragma warning restore CS0618

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
                .SetIconFile("wwwroot/favicon.ico")
                .SetWindowsAppUserModelId("InfiniLore.InfiniFrameExample.BlazorWebView")
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