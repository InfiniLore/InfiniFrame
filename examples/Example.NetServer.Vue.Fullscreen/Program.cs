using InfiniLore.Photino.Js.MessageHandlers;
using InfiniLore.Photino.NET;
using InfiniLore.Photino.NET.Server;
using System.Drawing;

namespace Example.NetServer.Vue.Fullscreen;
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
            // .SetTransparent(true)
            // .SetUseOsDefaultSize(false)
            .SetTitle("InfiniLore Photino.NET VUE Sample")
            .SetSize(new Size(800, 600))
            .SetLocation(1000,0)
            .SetBrowserControlInitParameters("--remote-debugging-port=9222")
            
            .RegisterFullScreenWebMessageHandler()
            .RegisterOpenExternalTargetWebMessageHandler()
            .RegisterTitleChangedWebMessageHandler()
            .RegisterWindowManagementWebMessageHandler()
            
            .RegisterWebMessageReceivedHandler((sender, message) => {
                if (sender is not IPhotinoWindow window) return;

                string response = $"Received message: \"{message}\"";
                window.SendWebMessage(response);
            });
        
        IPhotinoWindow window = windowBuilder.Build();
        // window.SetLocation(new Point(1000,0));
        
        window.WaitForClose();
        photinoServer.Stop();
    }
}
