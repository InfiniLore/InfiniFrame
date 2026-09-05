// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.BlazorWebView;
using InfiniFrameExample.BlazorWebView.MudBlazor.Components;
using MudBlazor.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Drawing;

namespace InfiniFrameExample.BlazorWebView.MudBlazor;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class Program {
    [STAThread]
    private static void Main(string[] args) {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Async(static c => c.Console())
            .CreateLogger();

        try {
            Log.Information("Starting InfiniFrame BlazorWebView MudBlazor example...");

            var appBuilder = InfiniFrameBlazorAppBuilder.CreateDefault(args);

            appBuilder.Services
                .AddLogging(config => {
                    config.ClearProviders();
                    config.AddSerilog();
                })
                .AddSerilog(config => {
                    config.WriteTo.Async(static c => c.Console())
                        .MinimumLevel.Debug();
                })
                .AddMudServices();

            appBuilder.RootComponents.Add<App>("app");

            appBuilder.WithInfiniFrameWindowBuilder(builder => {
                builder
                    .SetIconFile("wwwroot/favicon.ico")
                    .SetLocation(new Point(100, 100))
                    .SetSize(new Size(800, 600));
            });

            Log.Information("Building InfiniFrame application...");
            InfiniFrameBlazorApp app = appBuilder.Build();

            Log.Information("Running application...");
            app.Run();
        }
        catch (Exception ex) {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally {
            Log.CloseAndFlush();
        }
    }
}
