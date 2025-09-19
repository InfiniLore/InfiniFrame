using InfiniLore.Photino.NET;
using InfiniLore.Photino.NET.Server;
using System.Diagnostics;
using System.Drawing;

namespace Example.InfiniLore.Photino.NetServer.Vue;

public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        PhotinoServer
            .CreateStaticFileServer(args, 5173, 100, "wwwroot", out var baseUrl)
            .RunAsync();
        
        var appUrl = Debugger.IsAttached ? "http://localhost:5173" : baseUrl;

        var window = new PhotinoWindow()
            .SetTitle("InfiniLore Photino.NET VUE Sample")
            .SetUseOsDefaultSize(false)
            .SetSize(new Size(800, 600))
            .Center()
            .SetResizable(true)
            .RegisterCustomSchemeHandler("app", (object _, string _, string _, out string? contentType) =>
            {
                contentType = "text/javascript";
                return new MemoryStream("""
                                        (() =>{
                                            window.setTimeout(() => {
                                                alert(`🎉 Dynamically inserted JavaScript.`);
                                            }, 1000);
                                        })();
                                        """u8.ToArray());
            })
            .RegisterWebMessageReceivedHandler((sender, message) =>
            {
                var window = (PhotinoWindow)sender!;
                var response = $"Received message: \"{message}\"";
                window.SendWebMessage(response);
            })
            .Load(baseUrl);
        
        window.WaitForClose();
    }
}
