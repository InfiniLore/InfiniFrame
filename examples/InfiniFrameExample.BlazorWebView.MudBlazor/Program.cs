// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrameExample.BlazorWebView.MudBlazor.Components;
using Serilog;
using System.Drawing;
using InfiniFrame;
using InfiniFrame.Application;
using InfiniFrame.BlazorWebView;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;

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

            InfiniFrameApplicationBuilder builder = InfiniFrameApplication.CreateBuilder(args);
            builder.Services
                .AddLogging(config => {
                    config.ClearProviders();
                    config.AddSerilog();
                })
                .AddSerilog(config => {
                    config.WriteTo.Async(static c => c.Console())
                        .MinimumLevel.Debug();
                })
                .AddMudServices();

            InfiniFrameApplication app = builder
                .UseBlazorWebView(configuration => {
                    configuration.ConfigureWindow(window => window
                        .SetIconFile("wwwroot/favicon.ico")
                        .SetLocation(new Point(100, 100))
                        .SetSize(new Size(800, 600)));
                    configuration.RootComponents.Add<App>("app");
                })
                .Build();

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
