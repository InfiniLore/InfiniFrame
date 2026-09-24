// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;
using InfiniFrame;
using InfiniFrame.Application;
using InfiniFrame.BlazorWebView;
using InfiniFrameExample.BlazorWebView.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace InfiniFrameExample.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class Program {
    [STAThread]
    private static void Main(string[] args) {
        InfiniFrameApplicationBuilder builder = InfiniFrameApplication.CreateBuilder(args);

        builder.Services.AddLogging(config => {
                config.ClearProviders();
                config.AddSerilog();
        });

        builder.Services.AddSerilog(config => {
                config.WriteTo.Async(static c => c.Console())
                    .MinimumLevel.Debug();
        });

        builder.WithWindow(window => window
            .SetIconFile("wwwroot/favicon.ico")
            .SetLocation(new Point(100, 100))
            .SetSize(new Size(800, 600)));
        

        builder.UseBlazorWebView(configuration => {
            // register the root component and selector
            configuration.RootComponents.Add<App>("app");

        });

        InfiniFrameApplication app = builder.Build();

        app.Run();
    }
}
