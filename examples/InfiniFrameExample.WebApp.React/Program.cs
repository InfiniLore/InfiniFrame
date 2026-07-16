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
        InfiniFrameWebApplicationBuilder appBuilder = InfiniFrameWebApplication.CreateBuilder(args);
        // WebApplicationBuilder appBuilder = builder.WebApp;
        appBuilder.WebApp.Services.AddSingleton<WebMessageCounter>();
        
        appBuilder.WindowBuilder
            .SetUseOsDefaultSize(false)
            .SetResizable(true)
            .Center()
            .SetTitle("InfiniLore InfiniFrame.NET REACT Sample")
            .SetSize(new Size(800, 600))
            .RegisterCustomSchemeHandler("app", handler: (_, _) => (
                new MemoryStream("""
                    (() =>{
                        window.setTimeout(() => {
                            alert(`🎉 Dynamically inserted JavaScript.`);
                        }, 1000);
                    })();
                    """u8.ToArray())
                ,"text/javascript")
            )
            .RegisterWebMessageReceivedHandler((IInfiniFrameWindow window, string message, WebMessageCounter counter ) => {
                int count = counter.Increment();
                string response = $"[{count}] Received message: \"{message}\"";
                window.SendWebMessage(response);
            });
        
        InfiniFrameWebApplication application = appBuilder.Build();

        application.UseAutoServerClose();

        application.WebApp.UseStaticFiles();
        application.WebApp.MapStaticAssets();

        application.Run();
    }
}
