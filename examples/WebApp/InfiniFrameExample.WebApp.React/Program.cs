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
    private sealed class WebMessageCounter {
        private int _count;
        public int Increment() => Interlocked.Increment(ref _count);
    }

    [STAThread]
    public static void Main(string[] args) {
        InfiniFrameApplication app = InfiniFrameApplication.Initialize();
        InfiniFrameWebApplication webApp = app.WithWebServer(
            webAppBuilder => {
                webAppBuilder.Services.AddSingleton<WebMessageCounter>();
            },
            windowBuilder => {
                windowBuilder
                    .UseOsDefaultSize(false)
                    .SetResizable()
                    .CenteredOnMainMonitor()
                    .SetTitle("InfiniLore InfiniFrame.NET REACT Sample")
                    .SetSize(new Size(800, 600))
                    .RegisterCustomSchemeHandler("app", handler: (_, _) => (
                        new MemoryStream([
                            .. """
                            (() =>{
                                window.setTimeout(() => {
                                    alert(`🎉 Dynamically inserted JavaScript.`);
                                }, 1000);
                            })();
                            """u8
                        ])
                        , "text/javascript")
                    )
                    .RegisterWebMessageReceivedHandler((IInfiniFrameWindow window, string message, WebMessageCounter counter) => {
                        int count = counter.Increment();
                        string response = $"[{count}] Received message: \"{message}\"";
                        window.SendWebMessage(response);
                    });
            }
        );

        webApp.UseAutoServerClose();

        webApp.WebApp.UseStaticFiles();
        webApp.WebApp.MapStaticAssets();

        app.Run();
    }
}
