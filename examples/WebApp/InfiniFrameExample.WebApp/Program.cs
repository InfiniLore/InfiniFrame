// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using Microsoft.AspNetCore.Builder;
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
                    webApp.MapGet("/", handler: () => Results.Content(
                        "<html><body>InfiniFrame loaded</body></html>",
                        "text/html"
                    ));
                },
                configureWindow: window => window
                    .SetTitle("InfiniFrame WebServer Repro")
                    .SetIconFile("wwwroot/favicon.ico")
            );

        app.Run();
    }
}
