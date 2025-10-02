using InfiniLore.Photino.NET;
using InfiniLore.Photino.NET.MessageHandlers;
using InfiniLore.Photino.NET.Server;
using System.Drawing;

namespace Example.InfiniLore.Photino.NetServer.Vue.Fullscreen;
public static class Program {
    [STAThread]
    public static void Main(string[] args) {
        var photinoServerBuilder = PhotinoServerBuilder.Create("wwwroot", args);
        photinoServerBuilder.UsePort(5172, 100);

        PhotinoServer photinoServer = photinoServerBuilder.Build();
        
        photinoServer.MapPhotinoJsEndpoints();

        photinoServer.Run();

        IPhotinoWindowBuilder windowBuilder = photinoServer.GetAttachedWindowBuilder()
            .Center()
            .SetUseOsDefaultSize(false)
            .SetTitle("InfiniLore Photino.NET VUE Sample")
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
                    """u8.ToArray()
                );
            })
            
            .RegisterFullScreenWebMessageHandler()
            .RegisterOpenExternalTargetWebMessageHandler()
            .RegisterTitleChangedWebMessageHandler()
            
            .RegisterWebMessageReceivedHandler((sender, message) => {
                if (sender is not IPhotinoWindow window) return;

                string response = $"Received message: \"{message}\"";
                window.SendWebMessage(response);
            });

        IPhotinoWindow window = windowBuilder.Build();

        window.WaitForClose();
        photinoServer.Stop();
    }
}
