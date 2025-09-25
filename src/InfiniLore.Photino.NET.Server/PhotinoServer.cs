using System.Reflection;

namespace InfiniLore.Photino.NET.Server;
/// <summary>
/// The PhotinoServer class enables users to host their web projects in
/// a static, local file server to prevent CORS and other issues.
/// </summary>
public class PhotinoServer {
    public WebApplication WebApp { get; internal init; } = null!;
    public string BaseUrl { get; internal init; } = null!;

    internal void InitializeStaticFileServer() {
        WebApp.UseDefaultFiles();
        WebApp.UseStaticFiles(new StaticFileOptions {
            DefaultContentType = "text/plain"
        });
    }

    public void Run() {
        _ = WebApp.RunAsync();
    }

    public void Stop() {
        _ = WebApp.StopAsync();
    }
}
