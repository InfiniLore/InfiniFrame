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
        InfiniFrameApplication app = InfiniFrameApplication.Initialize();
        InfiniFrameWebApplication webApp = app.WithWebServer(
            webAppBuilder => {
                webAppBuilder.Services
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

                webAppBuilder.Services.AddHttpClient("ServerApi", (sp, client) => {
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
                webAppBuilder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ServerApi"));

                webAppBuilder.Services.AddInfiniFrameJs();

                webAppBuilder.WebHost.UseStaticWebAssets();
            },
            windowBuilder => {
                windowBuilder
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
            }
        );

        // -------------------------------------------------------------------------------------------------------------
        // App
        // -------------------------------------------------------------------------------------------------------------
        webApp.UseAutoServerClose();

        WebApplication webAppInstance = webApp.WebApp;

        webAppInstance.UseRouting();

        webAppInstance.UseAntiforgery();
        webAppInstance.MapStaticAssets();

        webAppInstance.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}
