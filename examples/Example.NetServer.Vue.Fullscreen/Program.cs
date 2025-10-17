using InfiniLore.InfiniFrame;
using InfiniLore.InfiniFrame.Js.MessageHandlers;
using InfiniLore.InfiniFrame.Server;
using System.Drawing;

namespace Example.NetServer.Vue.Fullscreen;
public static class Program {
    [STAThread]
    public static void Main(string[] args) {
        var infiniFrameServerBuilder = InfiniFrameServerBuilder.Create("wwwroot", args);
        infiniFrameServerBuilder.UsePort(5172, 100);

        InfiniFrameServer infiniFrameServer = infiniFrameServerBuilder.Build();

        infiniFrameServer.MapInfiniFrameJsEndpoints();

        infiniFrameServer.Run();

        IInfiniFrameWindowBuilder windowBuilder = infiniFrameServer.GetAttachedWindowBuilder()
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
                if (sender is not IInfiniFrameWindow window) return;

                string response = $"Received message: \"{message}\"";
                window.SendWebMessage(response);
            });

        IInfiniFrameWindow window = windowBuilder.Build();
        // window.SetLocation(new Point(1000,0));

        window.WaitForClose();
        infiniFrameServer.Stop();
    }
}
