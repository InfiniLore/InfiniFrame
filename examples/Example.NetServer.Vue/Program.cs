using InfiniLore.InfiniFrame;
using InfiniLore.InfiniFrame.Server;
using System.Drawing;

namespace Example.NetServer.Vue;
public static class Program {
    [STAThread]
    public static void Main(string[] args) {
        var infiniFrameServerBuilder = InfiniFrameServerBuilder.Create("wwwroot", args);
        infiniFrameServerBuilder.UsePort(5173, 100);

        InfiniFrameServer infiniFrameServer = infiniFrameServerBuilder.Build();
        infiniFrameServer.Run();

        IInfiniFrameWindowBuilder windowBuilder = infiniFrameServer.GetAttachedWindowBuilder()
            .Center()
            .SetUseOsDefaultSize(false)
            .SetTitle("InfiniLore InfiniFrame.NET VUE Sample")
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
            .RegisterWebMessageReceivedHandler((sender, message) => {
                var window = (InfiniFrameWindow)sender!;
                string response = $"Received message: \"{message}\"";
                window.SendWebMessage(response);
            });

        IInfiniFrameWindow window = windowBuilder.Build();

        window.WaitForClose();
    }
}
