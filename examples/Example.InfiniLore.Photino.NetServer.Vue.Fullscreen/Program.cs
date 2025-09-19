using InfiniLore.Photino.NET;
using InfiniLore.Photino.NET.Server;
using System.Drawing;

namespace Example.InfiniLore.Photino.NetServer.Vue.Fullscreen;

public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var photinoServerBuilder = PhotinoServerBuilder.Create("wwwroot", args);
        photinoServerBuilder.UsePort(5172, 100);

        var photinoServer = photinoServerBuilder.Build();
        photinoServer.Run();

        var window = photinoServer.GetAttachedWindow()
            .SetTitle("InfiniLore Photino.NET VUE Sample")
            .SetUseOsDefaultSize(false)
            .SetSize(new Size(800, 600))
            .Center()
            .SetResizable(true)
            .RegisterCustomSchemeHandler("app", (object _, string _, string _, out string? contentType) =>
            {
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
            .RegisterWebMessageReceivedHandler((sender, message) =>
            {
                if (StandardWebMessageHandlers.TryHandleWebMessage(sender, message)) return;
                if (sender is not PhotinoWindow window) return;
                var response = $"Received message: \"{message}\"";
                window.SendWebMessage(response);
            });

        window.WaitForClose();
        photinoServer.Stop();
    }
}
