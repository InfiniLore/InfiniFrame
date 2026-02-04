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
        InfiniFrameWebApplicationBuilder builder = InfiniFrameWebApplication.CreateBuilder(args);
        // WebApplicationBuilder appBuilder = builder.WebApp;
        builder.WebApp.Services.AddSingleton<WebMessageCounter>();
        
        builder.Window
            .SetUseOsDefaultSize(false)
            .SetResizable(true)
            .Center()
            .SetTitle("InfiniLore InfiniFrame.NET REACT Sample")
            .SetSize(new Size(800, 600))
            .RegisterCustomSchemeHandler("app", handler: (_, _, _, out contentType) => {
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
            .RegisterWebMessageReceivedHandler((WebMessageCounter counter, object? sender, string message) => {
                var window = (InfiniFrameWindow)sender!;
                int count = counter.Increment();
                string response = $"[{count}] Received message: \"{message}\"";
                window.SendWebMessage(response);
            });
        
        InfiniFrameWebApplication application = builder.Build();

        application.UseAutoServerClose();

        application.WebApp.UseStaticFiles();
        application.WebApp.MapStaticAssets();

        application.Run();
    }
}
