using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using System;
using System.Linq;
using System.Net.NetworkInformation;

namespace Photino.NET.Server;

/// <summary>
/// The PhotinoServer class enables users to host their web projects in
/// a static, local file server to prevent CORS and other issues.
/// </summary>
public class PhotinoServer
{
    public static WebApplication CreateStaticFileServer(
        string[] args,
        out string baseUrl)
    {
        return CreateStaticFileServer(
            args,
            startPort: 8000,
            portRange: 100,
            webRootFolder: "wwwroot",
            out baseUrl);
    }

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
        var manifestEmbeddedFileProvider =
            new ManifestEmbeddedFileProvider(
                System.Reflection.Assembly.GetEntryAssembly(),
                $"Resources/{webRootFolder}");

        var physicalFileProvider = builder.Environment.WebRootFileProvider;

        //Try to read from disk first, if not found, try to read from embedded resources.
        CompositeFileProvider compositeWebProvider
            = new(physicalFileProvider, manifestEmbeddedFileProvider);

        builder.Environment.WebRootFileProvider = compositeWebProvider;

        int port = startPort;

        // Try ports until available port is found
        while (IPGlobalProperties
            .GetIPGlobalProperties()
            .GetActiveTcpListeners()
            .Any(x => x.Port == port))
        {
            if (port > port + portRange)
                throw new SystemException($"Couldn't find open port within range {port - portRange} - {port}.");
            port++;
        }

        baseUrl = $"http://localhost:{port}";

        builder.WebHost.UseUrls(baseUrl);

        WebApplication app = builder.Build();
        app.UseDefaultFiles();
        app.UseStaticFiles(new StaticFileOptions
        {
            DefaultContentType = "text/plain"
        });

        return app;
    }
}
