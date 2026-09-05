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
        InfiniFrameApplication application = InfiniFrameApplication.Initialize()
            .WithWebServer(builder => {
        // WebApplicationBuilder appBuilder = builder.WebApp;

        if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux()) builder.WindowBuilder.Debugging.SetRemoteDebuggingPort(9222);

        builder.WindowBuilder
            .CenteredOnMainMonitor()
            // .SetTransparent(true)
            // .SetUseOsDefaultSize(false)
            .SetTitle("InfiniLore InfiniFrame.NET VUE Sample")
            .SetSize(new Size(800, 600))
            .SetLocation(1000, 0)
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

        builder.ConfigureWebApplication(webApp => {
            webApp.UseStaticFiles();
            webApp.MapStaticAssets();
        });
    });

        application.Run();
    }
}
