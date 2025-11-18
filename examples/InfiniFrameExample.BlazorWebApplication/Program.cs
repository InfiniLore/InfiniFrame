// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Server;
using InfiniFrameExample.BlazorWebApplication.Components;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Drawing;

namespace InfiniFrameExample.BlazorWebApplication;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class Program {
    [STAThread]
    private static void Main(string[] args) {
        // -------------------------------------------------------------------------------------------------------------
        // Builder
        // -------------------------------------------------------------------------------------------------------------
        InfiniFrameWebApplicationBuilder builder = InfiniFrameWebApplication.CreateBuilder(args);
        WebApplicationBuilder appBuilder = builder.WebApp;

        appBuilder.Services.AddLogging(config => {
            config.ClearProviders();
            config.AddSerilog();
        });

        appBuilder.Services.AddSerilog(config => {
            config.WriteTo.Async(static c => c.Console())
                .MinimumLevel.Debug();
        });

        // register the root component and selector
        appBuilder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        appBuilder.WebHost.UseStaticWebAssets();

        InfiniFrameWindowBuilder windowBuilder = builder.Window;
        windowBuilder
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

        // -------------------------------------------------------------------------------------------------------------
        // App
        // -------------------------------------------------------------------------------------------------------------
        InfiniFrameWebApplication app = builder.Build();
        WebApplication webApp = app.WebApp;

        webApp.UseAntiforgery();
        webApp.MapStaticAssets();
        webApp.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}
