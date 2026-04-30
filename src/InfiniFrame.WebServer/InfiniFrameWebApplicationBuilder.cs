// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Js;

namespace InfiniFrame.WebServer;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWebApplicationBuilder : IInfiniFrameWebApplicationBuilder {
    public required WebApplicationBuilder WebApp { get; init; }
    public required IInfiniFrameWindowBuilder WindowBuilder { get; init; }
    
    public IServiceCollection Services => WebApp.Services;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    internal InfiniFrameWebApplicationBuilder Initialize() {
        Services
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

        if (startUrl is not null) WindowBuilder.SetStartUrl(startUrl);

        this.AddInfiniFrameJs();
        
        return this;
    }

    public InfiniFrameWebApplication Build() {
        WebApplication webApp = WebApp.Build();

        webApp.UseDefaultFiles();

        return new InfiniFrameWebApplication {
            WebApp = webApp,
            LazyWindow = new Lazy<IInfiniFrameWindow>(() => webApp.Services.GetRequiredService<IInfiniFrameWindow>())
        };
    }
}
