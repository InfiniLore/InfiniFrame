using InfiniLore.InfiniFrame;
using InfiniLore.InfiniFrame.Server;
using System.Drawing;

namespace Example.NetServer.React;
public static class Program {
    [STAThread]
    public static void Main(string[] args) {
        var photinoServerBuilder = PhotinoServerBuilder.Create("wwwroot", args);
        photinoServerBuilder.UsePort(5174, 100);

        PhotinoServer photinoServer = photinoServerBuilder.Build();
        photinoServer.Run();

        IPhotinoWindowBuilder windowBuilder = photinoServer.GetAttachedWindowBuilder()
            .SetUseOsDefaultSize(false)
            .SetResizable(true)
            .Center()
            .SetTitle("InfiniLore Photino.NET REACT Sample")
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
                        """u8.ToArray());
            })
            .RegisterWebMessageReceivedHandler((sender, message) => {
                var window = (InfiniFrameWindow)sender!;
                string response = $"Received message: \"{message}\"";
                window.SendWebMessage(response);
            });
        
        IPhotinoWindow window = windowBuilder.Build();
        
        window.WaitForClose();
    }
}
