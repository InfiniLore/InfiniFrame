// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.WebServer;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWebApplicationBuilder {
    public required WebApplicationBuilder WebApp { get; init; }
    public required InfiniFrameWindowBuilder Window { get; init; }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    internal InfiniFrameWebApplicationBuilder Initialize() {
        WebApp.Services
            .AddSingleton<IInfiniFrameWindowBuilder>(Window)
            .AddSingleton<IInfiniFrameWindow>(static provider => provider.GetRequiredService<IInfiniFrameWindowBuilder>().Build(provider))
            ;
        
        WebApp.WebHost.UseStaticWebAssets();

        // Prefer ASPNETCORE_URLS, then "urls", then hard-coded fallback
        string? configuredUrls = WebApp.Configuration["ASPNETCORE_URLS"]
            ?? WebApp.Configuration["urls"];

        // If there are multiple URLs (e.g. "https://localhost:7210;http://localhost:5210"), picks the first
        string? startUrl = configuredUrls?
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .FirstOrDefault();
        
        if (startUrl is not null) Window.SetStartUrl(startUrl);
        
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
