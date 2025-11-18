// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.WebServer;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWebApplication {
    public required WebApplication WebApp { get; init; }
    public required IInfiniFrameWindow Window { get; init; }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public static InfiniFrameWebApplicationBuilder CreateBuilder(params string[] args) {
        WebApplicationBuilder webAppBuilder = WebApplication.CreateBuilder(args);
        var windowBuilder = InfiniFrameWindowBuilder.Create();
        
        webAppBuilder.Services
            .AddSingleton<IInfiniFrameWindowBuilder>(windowBuilder)
            .AddSingleton<IInfiniFrameWindow>(static provider => provider.GetRequiredService<IInfiniFrameWindowBuilder>().Build(provider))
            ;

        // Prefer ASPNETCORE_URLS, then "urls", then hard-coded fallback
        string? configuredUrls = webAppBuilder.Configuration["ASPNETCORE_URLS"]
            ?? webAppBuilder.Configuration["urls"];

        // If there are multiple URLs (e.g. "https://localhost:7210;http://localhost:5210"), picks the first
        string? startUrl = configuredUrls?
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .FirstOrDefault();
        windowBuilder.Configuration.StartUrl = startUrl;

        return new InfiniFrameWebApplicationBuilder {
            WebApp = webAppBuilder,
            Window = windowBuilder
        };
    }

    public void Run() {
        var thread = new Thread(WebApp.Run);
        thread.Start();
        
        Window.WaitForClose();
    }

    public void Stop() {
        _ = WebApp.StopAsync();
        Window.Close();
    }
}
