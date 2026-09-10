// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Drawing;
using InfiniFrame;
using InfiniFrame.Application;
using InfiniFrame.WebServer;
using InfiniFrame.Window.Features.WebMessaging.Handlers;

namespace InfiniFrameExample.WebApp.Vue;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class Program {
    [STAThread]
    public static void Main(string[] args) {
        InfiniFrameApplication application = InfiniFrameApplication.CreateBuilder(args)
            .WithWindow(window => {
                if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux()) window.Debugging.SetRemoteDebuggingPort(9222);
                window
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
            })
            .UseWebServer(builder => {
                builder.ConfigureWebApplication(webApp => {
                    webApp.MapStaticAssets();
                });
            })
            .Build();

        application.Run();
    }
}
