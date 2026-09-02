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
        InfiniFrameApplication app = InfiniFrameApplication.Initialize();
        InfiniFrameWebApplication webApp = app.WithWebServer(
            _ => { },
            windowBuilder => {
                if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux()) windowBuilder.Debugging.SetRemoteDebuggingPort(9222);

                windowBuilder
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
            }
        );

        webApp.UseAutoServerClose();

        webApp.WebApp.UseStaticFiles();
        webApp.WebApp.MapStaticAssets();

        app.Run();
    }
}
