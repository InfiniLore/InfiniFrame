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

            InfiniFrameApplication app = InfiniFrameApplication.Initialize();
            app.WithBlazorWebView(blazorBuilder => {
                blazorBuilder.Services
                    .AddLogging(config => {
                        config.ClearProviders();
                        config.AddSerilog();
                    })
                    .AddSerilog(config => {
                        config.WriteTo.Async(static c => c.Console())
                            .MinimumLevel.Debug();
                    })
                    .AddMudServices();

                blazorBuilder.RootComponents.Add<App>("app");

                blazorBuilder.WindowBuilder
                    .SetIconFile("wwwroot/favicon.ico")
                    .RegisterOpenExternalTargetWebMessageHandler();

                blazorBuilder.AddSingleFileRequirements();
            });

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
