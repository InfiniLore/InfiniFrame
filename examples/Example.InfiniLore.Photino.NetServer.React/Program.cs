using InfiniLore.Photino.NET;
using InfiniLore.Photino.NET.Server;
using System.Drawing;

namespace Example.InfiniLore.Photino.NetServer.React;
public static class Program {
    [STAThread]
    public static void Main(string[] args) {
        var photinoServerBuilder = PhotinoServerBuilder.Create("wwwroot", args);
        photinoServerBuilder.UsePort(5174, 100);

        PhotinoServer photinoServer = photinoServerBuilder.Build();
        photinoServer.Run();



        IPhotinoWindowBuilder windowBuilder = photinoServer.GetAttachedWindowBuilder()
            .Center();
        // .SetResizable(true);

        IPhotinoWindow window = windowBuilder.Build()
            .SetTitle("InfiniLore Photino.NET REACT Sample")
            .SetUseOsDefaultSize(false)
            .SetSize(new Size(800, 600))
            .RegisterCustomSchemeHandler("app", (object _, string _, string _, out string? contentType) => {
                contentType = "text/javascript";
                return new MemoryStream("""
                                            (() =>{
                                                window.setTimeout(() => {
                                                    alert(`🎉 Dynamically inserted JavaScript.`);
                                                }, 1000);
                                            })();
                                            """u8.ToArray());
            })
            .RegisterWebMessageReceivedHandler((sender, message) => {
                var window = (PhotinoWindow)sender!;
                string response = $"Received message: \"{message}\"";
                window.SendWebMessage(response);
            });

        window.WaitForClose();

    }
}
