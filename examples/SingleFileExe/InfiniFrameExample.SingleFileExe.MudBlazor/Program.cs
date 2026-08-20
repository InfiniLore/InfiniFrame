// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.BlazorWebView;
using InfiniFrame.SingleFile;
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

            appBuilder.WindowBuilder
                .SetIconFile("wwwroot/favicon.ico")
                .RegisterOpenExternalTargetWebMessageHandler();
            
            InfiniFrameSingleFile.AddSingleFileRequirements(appBuilder);

            Log.Information("Building InfiniFrame application...");
            InfiniFrameBlazorApp application = appBuilder.Build();

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
