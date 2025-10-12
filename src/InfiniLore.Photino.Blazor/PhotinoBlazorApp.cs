using InfiniLore.Photino.NET;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Sockets;

namespace InfiniLore.Photino.Blazor;
public class PhotinoBlazorApp : IDisposable {
    private readonly WebApplication _webApp;
    private readonly IPhotinoWindowBuilder _windowBuilder;

    internal PhotinoBlazorApp(WebApplication webApp, IPhotinoWindowBuilder windowBuilder) {
        _webApp = webApp;
        _windowBuilder = windowBuilder;
        Services = webApp.Services;
    }

    /// <summary>
    ///     Access to the service provider
    /// </summary>
    public IServiceProvider Services { get; }

    /// <summary>
    ///     Access to the Photino window after it's built
    /// </summary>
    public IPhotinoWindow? MainWindow { get; private set; }

    /// <summary>
    ///     The configured app base URI from window configuration
    /// </summary>
    public Uri AppBaseUri => new Uri(_windowBuilder.Configuration.StartUrl ?? string.Empty);

    /// <summary>
    ///     The actual base URL after the web server starts
    /// </summary>
    public string? BaseUrl { get; private set; }

    /// <summary>
    ///     The base URI as a Uri object
    /// </summary>
    public Uri BaseUri => BaseUrl != null
        ? new Uri(BaseUrl)
        : AppBaseUri;

    /// <summary>
    ///     Start the application
    /// </summary>
    public async Task RunAsync() {
        // Start the ASP.NET Core application on a local-only address
        BaseUrl = await StartWebApplicationAsync();

        // Build and configure the Photino window
        BuildAndConfigureWindow(BaseUrl);

        // Start the message loop (blocks until window closes)
        MainWindow!.WaitForClose();

        // Cleanup
        await _webApp.StopAsync();
    }

    /// <summary>
    ///     Synchronous version of Run
    /// </summary>
    public void Run() {
        RunAsync().GetAwaiter().GetResult();
    }

    private async Task<string> StartWebApplicationAsync() {
        // Find an available port
        int port = GetAvailablePort();
        string url = $"http://localhost:{port}";

        // Configure the web app to listen on localhost only
        _webApp.Urls.Clear();
        _webApp.Urls.Add(url);

        // Start the web application
        await _webApp.StartAsync();

        return url;
    }

    private void BuildAndConfigureWindow(string baseUrl) {
        // If no start URL was explicitly set, use the web server URL
        if (string.IsNullOrEmpty(_windowBuilder.Configuration.StartUrl)) {
            _windowBuilder.SetStartUrl(baseUrl);
        }

        // Build the window with the service provider
        MainWindow = Services.GetService<IPhotinoWindow>();
    }

    private static int GetAvailablePort() {
        using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(new IPEndPoint(IPAddress.Loopback, 0));
        int port = ((IPEndPoint)socket.LocalEndPoint!).Port;
        return port;
    }

    public void Dispose() {
        MainWindow?.Close();
        (_webApp as IDisposable).Dispose();
    }
}
