// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
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
        InfiniFrameWebApplicationBuilder appBuilder = InfiniFrameWebApplication.CreateBuilder(args);

        appBuilder.Services
            .AddLogging(config => {
                config.ClearProviders();
                config.AddSerilog();
            })
            .AddSerilog(config => {
                config.WriteTo.Async(static c => c.Console())
                    .MinimumLevel.Debug();
            })
            .AddRazorComponents()
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

        appBuilder.WebApp.WebHost.UseStaticWebAssets();

        appBuilder.WindowBuilder
            // .SetTransparent(true)
            // .SetChromeless(true)
            // .SetResizable(true)
            .SetIconFile("wwwroot/favicon.ico")
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
        InfiniFrameWebApplication application = appBuilder.Build();
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