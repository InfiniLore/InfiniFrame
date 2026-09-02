// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.WebServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

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
    /// <returns>The application instance for chaining.</returns>
    public static InfiniFrameApplication WithWebServer(
        this InfiniFrameApplication app,
        Action<WebApplicationBuilder> configureWebApp,
        Action<IInfiniFrameWindowBuilder>? configureWindow = null
    ) {
        WebApplicationBuilder webAppBuilder = WebApplication.CreateBuilder();
        configureWebApp(webAppBuilder);

        // Store the web app so it can be started before window creation.
        WebApplication? webApp = null;

        app.SetOnBeforeRun(() => {
            webApp = webAppBuilder.Build();
            webApp.UseDefaultFiles();
            webApp.Start();
        });

        // Register window with the application.
        app.WithWindow(windowBuilder => {
            configureWindow?.Invoke(windowBuilder);

            // Auto-close the web server when the window closes.
            windowBuilder.RegisterWindowClosingHandler((_, _) => {
                _ = StopWebAppAsync();
                return WindowClosingResult.Close;
            });
            windowBuilder.RegisterWindowClosingRequestedHandler(_ => {
                _ = StopWebAppAsync();
            });
        });

        return app;

        async Task StopWebAppAsync() {
            if (webApp is null) return;
            try {
                await webApp.StopAsync(CancellationToken.None).ConfigureAwait(false);
            }
            catch {
                // Best effort during shutdown.
            }
        }
    }
}
