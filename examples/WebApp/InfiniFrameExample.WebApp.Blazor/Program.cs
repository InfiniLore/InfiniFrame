// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;
using InfiniFrame;
using InfiniFrame.WebServer;
using InfiniFrameExample.WebApp.Blazor.Components;
using Serilog;

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
        InfiniFrameApplicationBuilder builder = InfiniFrameApplication.CreateBuilder(args);

        builder.WithWindow(window => window
            .SetIconFile("wwwroot/favicon.ico")
            .SetLocation(new Point(100, 100))
            .SetSize(new Size(800, 600))
            .RegisterOpenExternalTargetWebMessageHandler()
        );

        builder.Services
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

        builder.Services.AddHttpClient("ServerApi", configureClient: (sp, client) => {
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
        builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ServerApi"));

        builder.Services.AddInfiniFrameJs();

        builder.UseWebServer(web => {

                web.WebHost.UseStaticWebAssets();

            web.ConfigureWebApplication(webApp => {
                webApp.UseRouting();
                webApp.UseAntiforgery();
                webApp.MapStaticAssets();
                webApp.MapRazorComponents<App>()
                    .AddInteractiveServerRenderMode();
            });
        });

        InfiniFrameApplication application = builder.Build();
        application.Run();
    }
}
