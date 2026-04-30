// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.WebServer;
using System.Drawing;

namespace InfiniFrameExample.WebApp.Vue;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class Program {
    [STAThread]
    public static void Main(string[] args) {
        InfiniFrameWebApplicationBuilder appBuilder = InfiniFrameWebApplication.CreateBuilder(args);
        // WebApplicationBuilder appBuilder = builder.WebApp;

        appBuilder.WindowBuilder
            .Center()
            // .SetTransparent(true)
            // .SetUseOsDefaultSize(false)
            .SetTitle("InfiniLore InfiniFrame.NET VUE Sample")
            .SetSize(new Size(800, 600))
            .SetLocation(1000, 0)
            .SetBrowserControlInitParameters("--remote-debugging-port=9222")
            .RegisterFullScreenWebMessageHandler()
            .RegisterOpenExternalTargetWebMessageHandler()
            .RegisterTitleChangedWebMessageHandler()
            .RegisterWindowManagementWebMessageHandler()
            .RegisterWebMessageReceivedHandler((_, message) => {
                // ReSharper disable twice UnusedVariable
                string response = $"Received message: \"{message}\"";
                
                // ... do something with the message
            })
            ;
        
        InfiniFrameWebApplication application = appBuilder.Build();

        application.UseAutoServerClose();
        
        application.WebApp.UseStaticFiles();
        application.WebApp.MapStaticAssets();

        application.Run();
    }
}
