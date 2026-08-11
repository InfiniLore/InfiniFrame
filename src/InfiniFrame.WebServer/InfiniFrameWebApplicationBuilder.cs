// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Security;
using Microsoft.Extensions.Logging.Abstractions;

namespace InfiniFrame.WebServer;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWebApplicationBuilder : IInfiniFrameWebApplicationBuilder {
    /// <inheritdoc cref="IInfiniFrameWebApplicationBuilder.WebApp"/>
    public required WebApplicationBuilder WebApp { get; init; }
    /// <inheritdoc cref="IInfiniFrameWebApplicationBuilder.WindowBuilder"/>
    public required IInfiniFrameWindowBuilder WindowBuilder { get; init; }

    /// <inheritdoc cref="IInfiniFrameWebApplicationBuilder.Services"/>
    public IServiceCollection Services => WebApp.Services;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    internal InfiniFrameWebApplicationBuilder Initialize() {
        Services
            .AddInfiniFrame()
            .AddSingleton(WindowBuilder)
            .AddSingleton<IInfiniFrameWindow>(static provider => provider.GetRequiredService<IInfiniFrameWindowBuilder>().Build(provider));

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

        return new InfiniFrameWebApplication {
            Logger = webApp.Services.GetService<ILogger<InfiniFrameWebApplication>>() ?? NullLogger<InfiniFrameWebApplication>.Instance,
            WebApp = webApp,
            LazyWindow = new Lazy<IInfiniFrameWindow>(() => webApp.Services.GetRequiredService<IInfiniFrameWindow>())
        };
    }
}