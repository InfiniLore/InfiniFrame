// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Security;
using Microsoft.Extensions.Logging.Abstractions;

namespace InfiniFrame.WebServer;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Builder for creating an ASP.NET Core web application with a native InfiniFrame window.
/// </summary>
public class InfiniFrameWebApplicationBuilder : IInfiniFrameWebApplicationBuilder {
    /// <inheritdoc cref="IInfiniFrameWebApplicationBuilder.WebApp" />
    public required WebApplicationBuilder WebApp { get; init; }
    /// <inheritdoc cref="IInfiniFrameWebApplicationBuilder.WindowBuilder" />
    public required IInfiniFrameWindowBuilder WindowBuilder { get; init; }

    /// <inheritdoc cref="IInfiniFrameWebApplicationBuilder.Services" />
    public IServiceCollection Services => WebApp.Services;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    internal InfiniFrameWebApplicationBuilder Initialize(IInfiniFrameApplication? application = null) {
        Services
            .AddInfiniFrame()
            .AddSingleton(WindowBuilder)
            .AddSingleton<IInfiniFrameWindow>(static provider => provider.GetRequiredService<IInfiniFrameWindowBuilder>().Build(provider));

        if (application is not null)
            Services.AddSingleton(application);

        WebApp.WebHost.UseStaticWebAssets();

        // Prefer ASPNETCORE_URLS, then "urls" else it has to be set by the dev themselves
        string? configuredUrls = WebApp.Configuration["ASPNETCORE_URLS"]
            ?? WebApp.Configuration["urls"];

        // If there are multiple URLs (e.g. "https://localhost:7210;http://localhost:5210"), picks the first
        string? startUrl = configuredUrls?
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .FirstOrDefault();

        if (startUrl is not null) WindowBuilder.SetStartPageUrl(startUrl);

        WindowBuilder.RegisterGetWebMessageHandler();

        return this;
    }

    /// <summary>
    ///     Builds the web application and native window, returning an <see cref="InfiniFrameWebApplication"/>.
    /// </summary>
    /// <returns>The built <see cref="InfiniFrameWebApplication"/>.</returns>
    public InfiniFrameWebApplication Build() {
        WebApplication webApp = WebApp.Build();

        webApp.UseDefaultFiles();

        string? configuredUrls = WebApp.Configuration["ASPNETCORE_URLS"]
            ?? WebApp.Configuration["urls"];
        string? startUrl = configuredUrls?
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .FirstOrDefault();

        if (startUrl is not null && Uri.TryCreate(startUrl, UriKind.Absolute, out Uri? baseUri)) {
            InfiniFrameUriSecurityPolicyRegistry.ConfigureForBuilder(
                WindowBuilder,
                configure: policyBuilder => policyBuilder.AddTrustedOrigin(baseUri));
        }

        // Initialize the application singleton so it's ready before any windows are created.
        var application = webApp.Services.GetRequiredService<IInfiniFrameApplication>();
        if (application.ApplicationHandle == IntPtr.Zero && !application.IsShutdownRequested) {
            var appConfig = new ApplicationConfiguration();
            if (OperatingSystem.IsWindows()) {
                appConfig.HInstance = System.Diagnostics.Process.GetCurrentProcess().MainModule?.BaseAddress ?? IntPtr.Zero;
            }
            application.Initialize(appConfig);
        }

        return new InfiniFrameWebApplication {
            Logger = webApp.Services.GetService<ILogger<InfiniFrameWebApplication>>() ?? NullLogger<InfiniFrameWebApplication>.Instance,
            WebApp = webApp,
            LazyWindow = new Lazy<IInfiniFrameWindow>(() => webApp.Services.GetRequiredService<IInfiniFrameWindow>())
        };
    }
}
