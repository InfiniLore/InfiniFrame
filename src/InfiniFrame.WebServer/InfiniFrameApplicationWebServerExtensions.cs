// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.WebServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Extension methods for integrating ASP.NET Core web server with <see cref="InfiniFrameApplication"/>.
/// </summary>
public static class InfiniFrameApplicationWebServerExtensions {
    /// <summary>
    ///     Adds an ASP.NET Core web server to the application. The web server starts before the
    ///     native window is created, so the window can navigate to the server's URL.
    ///     When the window closes, the web server is also stopped.
    /// </summary>
    /// <param name="app">The application instance.</param>
    /// <param name="configureWebApp">Callback to configure the <see cref="WebApplicationBuilder"/>.</param>
    /// <param name="configureWindow">Optional callback to configure the window builder.</param>
    /// <returns>The <see cref="InfiniFrameWebApplication"/> for further configuration (e.g. <c>UseAutoServerClose()</c>).</returns>
    public static InfiniFrameWebApplication WithWebServer(
        this InfiniFrameApplication app,
        Action<WebApplicationBuilder> configureWebApp,
        Action<IInfiniFrameWindowBuilder>? configureWindow = null
    ) {
        WebApplicationBuilder webAppBuilder = WebApplication.CreateBuilder();
        configureWebApp(webAppBuilder);

        // Build and start the web server before window creation.
        WebApplication webApp = webAppBuilder.Build();
        webApp.UseDefaultFiles();
        webApp.Start();

        // Create window builder and register with application.
        var windowBuilder = new InfiniFrameWindowBuilder();
        configureWindow?.Invoke(windowBuilder);

        // Use a stable string key for the window so we can retrieve it later.
        string windowId = $"webapp-{app.Id}";
        app.WithWindow(windowId, windowBuilder);

        // Build the wrapper.
        var wrapper = new InfiniFrameWebApplication {
            Logger = NullLogger<InfiniFrameWebApplication>.Instance,
            WebApp = webApp,
            LazyWindow = new Lazy<IInfiniFrameWindow>(() => app.GetWindow(windowId)),
            Application = app
        };

        // Auto-close the web server when the window closes.
        windowBuilder.RegisterWindowClosingHandler((_, _) => StopWebApp(webApp));
        windowBuilder.RegisterWindowClosingRequestedHandler(_ => StopWebApp(webApp));

        return wrapper;
    }

    private static WindowClosingResult StopWebApp(WebApplication webApp) {
        _ = webApp.StopAsync(CancellationToken.None);
        return WindowClosingResult.Close;
    }
}
