// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.WebServer;
using System.Drawing;

namespace InfiniFrameExample.WebApp.React;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class Program {
    [STAThread]
    public static void Main(string[] args) {
        InfiniFrameWebApplicationBuilder builder = InfiniFrameWebApplication.CreateBuilder(args);
        WebApplicationBuilder appBuilder = builder.WebApp;

        appBuilder.WebHost.UseStaticWebAssets();

        builder.Window
            .SetUseOsDefaultSize(false)
            .SetResizable(true)
            .Center()
            .SetTitle("InfiniLore InfiniFrame.NET REACT Sample")
            .SetSize(new Size(800, 600))
            .RegisterCustomSchemeHandler("app", handler: (object _, string _, string _, out string? contentType) => {
                contentType = "text/javascript";
                return new MemoryStream(
                    """
                        (() =>{
                            window.setTimeout(() => {
                                alert(`🎉 Dynamically inserted JavaScript.`);
                            }, 1000);
                        })();
                        """u8.ToArray());
            })
            .RegisterWebMessageReceivedHandler((sender, message) => {
                var window = (InfiniFrameWindow)sender!;
                string response = $"Received message: \"{message}\"";
                window.SendWebMessage(response);
            });
        
        InfiniFrameWebApplication application = builder.Build();

        application.WebApp.UseDefaultFiles();
        application.WebApp.UseStaticFiles();

        application.Run();
    }
}
