// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.WebServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace InfiniFrameExample.WebApp;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class Program {
    [STAThread]
    public static void Main(string[] args) {
        var app = InfiniFrameApplication.Initialize()
            .WithWebServer(
                configureWebApp: webApp => {
                    webApp.WebHost.UseUrls("http://127.0.0.1:5055");
                },
                configureWindow: window => window
                    .SetTitle("InfiniFrame WebServer Repro")
                    .SetIconFile("wwwroot/favicon.ico")
            );

        app.WebApp.MapGet("/", handler: () => Results.Content(
            "<html><body>InfiniFrame loaded</body></html>",
            "text/html"
        ));

        app.Run();
    }
}
