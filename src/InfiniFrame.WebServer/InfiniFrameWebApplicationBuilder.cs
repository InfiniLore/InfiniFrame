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
    public InfiniFrameWebApplication Build() {
        WebApplication webApp = WebApp.Build();

        webApp.UseDefaultFiles();

        return new InfiniFrameWebApplication {
            WebApp = webApp,
            Window = webApp.Services.GetRequiredService<IInfiniFrameWindow>()
        };
    }
}
