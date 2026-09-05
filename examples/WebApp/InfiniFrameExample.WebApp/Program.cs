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
        InfiniFrameApplication app = InfiniFrameApplication.Initialize()
            .WithWebServer(builder => {
                builder.WebApp.WebHost.UseUrls("http://127.0.0.1:5055");
                builder.WindowBuilder
                    .SetStartPageUrl("http://127.0.0.1:5055")
                    .SetTitle("InfiniFrame WebServer Repro")
                    .SetIconFile("wwwroot/favicon.ico");
                builder.ConfigureWebApplication(webApp => webApp.MapGet("/", handler: () => Results.Content(
                    "<html><body>InfiniFrame loaded</body></html>",
                    "text/html"
                )));
            });

        app.Run();
    }
}
