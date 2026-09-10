// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Application;
using InfiniFrame.BlazorWebView;
using InfiniFrame.SingleFile;
using InfiniFrame.Window.Features.WebMessaging.Handlers;
using InfiniFrameExample.SingleFileExe.MudBlazor.Components;
using MudBlazor.Services;
using Serilog;

namespace InfiniFrameExample.SingleFileExe.MudBlazor;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class Program {
    [STAThread]
    private static void Main(string[] args) {
        InfiniFrameSingleFile.Initialize();
        
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Async(c => c.Console())
            .CreateLogger();

        try {
            Log.Information("Starting InfiniFrame MudBlazor example...");

            InfiniFrameApplicationBuilder builder = InfiniFrameApplication.CreateBuilder(args);
            builder.WithWindow(window => window
                .SetIconFile("wwwroot/favicon.ico")
                .RegisterOpenExternalTargetWebMessageHandler());
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
            InfiniFrameApplication application = builder
                .UseBlazorWebView(configuration => {
                    configuration.RootComponents.Add<App>("app");
                    configuration.AddSingleFileRequirements();
                })
                .Build();

            Log.Information("Running application...");
            application.Run();
        }
        catch (Exception ex) {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally {
            Log.CloseAndFlush();
        }
    }
}
