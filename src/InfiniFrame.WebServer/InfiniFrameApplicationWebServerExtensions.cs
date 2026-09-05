using InfiniFrame;

namespace InfiniFrame.WebServer;

/// <summary>Application-first integration for ASP.NET Core web servers.</summary>
public static class InfiniFrameApplicationWebServerExtensions {
    /// <summary>
    ///     Adds an ASP.NET Core web server and registers its native window with the application owner.
    /// </summary>
    public static InfiniFrameApplication WithWebServer(
        this InfiniFrameApplication application,
        Action<InfiniFrameWebApplicationBuilder> configure
    ) => WithWebServer(application, "web", [], configure);

    /// <summary>Adds a named ASP.NET Core web server to the application.</summary>
    public static InfiniFrameApplication WithWebServer(
        this InfiniFrameApplication application,
        string windowId,
        Action<InfiniFrameWebApplicationBuilder> configure
    ) => WithWebServer(application, windowId, [], configure);

    private static InfiniFrameApplication WithWebServer(
        InfiniFrameApplication application,
        string windowId,
        string[] args,
        Action<InfiniFrameWebApplicationBuilder> configure
    ) {
        ArgumentNullException.ThrowIfNull(application);
        ArgumentException.ThrowIfNullOrWhiteSpace(windowId);
        ArgumentNullException.ThrowIfNull(configure);

        var builder = new InfiniFrameWebApplicationBuilder {
            WebApp = WebApplication.CreateBuilder(args),
            WindowBuilder = new InfiniFrameWindowBuilder()
        }.Initialize();
        configure(builder);

        InfiniFrameWebApplication webApplication = builder.Build();
        webApplication.WebApp.StartAsync().GetAwaiter().GetResult();
        application.RegisterWindowBuilder(windowId, (InfiniFrameWindowBuilder)builder.WindowBuilder);
        application.RegisterShutdownAction(webApplication.StopServerAsync);
        return application;
    }
}
