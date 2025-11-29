// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Js;
using InfiniFrame.Js.MessageHandlers;
using InfiniFrame.WebServer;
using InfiniFrameExample.WebApp.Blazor.Components;
using Serilog;
using System.Drawing;

namespace InfiniFrameExample.WebApp.Blazor;
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

        appBuilder.Services.AddHttpClient("ServerApi", (sp, client) => {
            var config = sp.GetRequiredService<IConfiguration>();

            // Prefer ASPNETCORE_URLS, then "urls", then a fallback
            string urls = config["ASPNETCORE_URLS"]
                ?? config["urls"]
                ?? "http://localhost:5000";

            string baseUrl = urls
                .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .First();

            client.BaseAddress = new Uri(baseUrl);
        });
        appBuilder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ServerApi"));

        appBuilder.Services.AddInfiniFrameJs();
        
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
            .RegisterOpenExternalTargetWebMessageHandler()
            // .SetMaxSize(new Size(800, 600))
            // .SetMinSize(new Size(600, 400))
            ;

        // -------------------------------------------------------------------------------------------------------------
        // App
        // -------------------------------------------------------------------------------------------------------------
        InfiniFrameWebApplication application = builder.Build();
        application.UseAutoServerClose();
        
        WebApplication webApp = application.WebApp;

        webApp.UseRouting();

        webApp.UseAntiforgery();
        webApp.MapStaticAssets();

        webApp.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        application.Run();
    }
}
