using Microsoft.Extensions.FileProviders;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;

namespace InfiniLore.Photino.NET.Server;
public class PhotinoServerBuilder {
    public WebApplicationBuilder WebAppBuilder { get; private init; } = null!;

    public int Port { get; internal set; } = 8000;
    public int PortRange { get; internal set; } = -1;

    private string? EmbeddedFileProviderBaseNamespace { get; init; }
    private string? BaseUrl { get; set; }

    private PhotinoServerBuilder() {}

    public static PhotinoServerBuilder Create(string webRootFolder = "wwwroot", string[]? args = null) => new() {
        EmbeddedFileProviderBaseNamespace = webRootFolder,
        WebAppBuilder = WebApplication.CreateBuilder(new WebApplicationOptions {
            Args = args,
            WebRootPath = webRootFolder
        })
    };

    public PhotinoServer Build() {
        InitializeFileProvider();
        InitializePortAssignment();
        InitializeWebHost();

        ArgumentException.ThrowIfNullOrWhiteSpace(BaseUrl);

        var server = new PhotinoServer {
            WebApp = WebAppBuilder.Build(),
            BaseUrl = BaseUrl
        };

        server.InitializeStaticFileServer();

        return server;
    }

    private void InitializeFileProvider() {
        if (Assembly.GetEntryAssembly() is not {} entryAssembly) {
            throw new SystemException("Could not find entry assembly.");
        }

        IFileProvider physicalFileProvider = WebAppBuilder.Environment.WebRootFileProvider;

        entryAssembly.GetManifestResourceNames();

        var embeddedFileProvider = new EmbeddedFileProvider(entryAssembly, EmbeddedFileProviderBaseNamespace);

        //Try to read from the disk first, if not found, try to read from embedded resources.
        var compositeWebProvider = new CompositeFileProvider(physicalFileProvider, embeddedFileProvider);

        WebAppBuilder.Environment.WebRootFileProvider = compositeWebProvider;
    }

    private void InitializePortAssignment() {
        // Don't do anything if no port range is specified
        if (PortRange < 0) return;

        IPEndPoint[] listeners = IPGlobalProperties
            .GetIPGlobalProperties()
            .GetActiveTcpListeners();

        // Try ports until an available port is found within the PortRange.
        for (int newPort = Port; newPort < Port + PortRange; newPort++) {
            if (listeners.Any(x => x.Port == newPort)) continue;

            Port = newPort;
            break;
        }
        if (Port == Port + PortRange) throw new SystemException($"Couldn't find open port within range {Port} - {Port + PortRange}.");
    }

    private void InitializeWebHost() {
        // TODO add https support
        BaseUrl = $"http://localhost:{Port}";
        WebAppBuilder.WebHost.UseUrls(BaseUrl);
    }
}
