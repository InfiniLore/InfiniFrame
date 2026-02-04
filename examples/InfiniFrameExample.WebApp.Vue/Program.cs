// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.Js.MessageHandlers;
using InfiniFrame.WebServer;
using System.Drawing;

namespace InfiniFrameExample.WebApp.Vue;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class Program {
    [STAThread]
    public static void Main(string[] args) {
        InfiniFrameWebApplicationBuilder builder = InfiniFrameWebApplication.CreateBuilder(args);
        // WebApplicationBuilder appBuilder = builder.WebApp;

        builder.Window
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
            .RegisterWebMessageReceivedHandler((sender, message) => {
                // ReSharper disable twice UnusedVariable
                if (sender is not IInfiniFrameWindow window) return;
                string response = $"Received message: \"{message}\"";
                
                // ... do something with the message
            })
            ;
        
        InfiniFrameWebApplication application = builder.Build();

        application.UseAutoServerClose();
        
        application.WebApp.UseStaticFiles();
        application.WebApp.MapStaticAssets();

        application.Run();
    }
}
