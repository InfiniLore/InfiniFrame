using Microsoft.Extensions.FileProviders;
using System.Net.NetworkInformation;
using System.Reflection;

namespace InfiniLore.Photino.NET.Server;

/// <summary>
/// The PhotinoServer class enables users to host their web projects in
/// a static, local file server to prevent CORS and other issues.
/// </summary>
public class PhotinoServer
{
    public static WebApplication CreateStaticFileServer(string[] args, out string baseUrl) => CreateStaticFileServer(
    args,
    startPort: 8000,
    portRange: 100,
    webRootFolder: "wwwroot",
    out baseUrl
    );

    public static WebApplication CreateStaticFileServer(
        string[] args,
        int startPort,
        int portRange,
        string webRootFolder,
        out string baseUrl)
    {
        //This will create the web root folder on disk if it doesn't exist
        var builder = WebApplication
            .CreateBuilder(new WebApplicationOptions()
            {
                Args = args,
                WebRootPath = webRootFolder
            });

        //Try to read files from the embedded resources - from a slightly different path, prefixed with Resources/
        if (Assembly.GetEntryAssembly() is not {} entryAssembly)
        {
            throw new SystemException("Could not find entry assembly.");
        }
        
        var physicalFileProvider = builder.Environment.WebRootFileProvider;
        var manifestEmbeddedFileProvider = new ManifestEmbeddedFileProvider(entryAssembly, $"Resources/{webRootFolder}");

        //Try to read from the disk first, if not found, try to read from embedded resources.
        var compositeWebProvider = new CompositeFileProvider(physicalFileProvider, manifestEmbeddedFileProvider);

        builder.Environment.WebRootFileProvider = compositeWebProvider;

        var port = startPort;

        // Try ports until available port is found
        while (IPGlobalProperties
               .GetIPGlobalProperties()
               .GetActiveTcpListeners()
               .Any(x => x.Port == port))
        {
            if (port > port + portRange) throw new SystemException($"Couldn't find open port within range {port - portRange} - {port}.");
            port++;
        }

        // Because this is for locally running and serving the static files, we don't need to worry about SSL.
        baseUrl = $"http://localhost:{port}";

        builder.WebHost.UseUrls(baseUrl);

        var app = builder.Build();
        app.UseDefaultFiles();
        app.UseStaticFiles(new StaticFileOptions 
        {
            DefaultContentType = "text/plain"
        });

        return app;
    }
}
