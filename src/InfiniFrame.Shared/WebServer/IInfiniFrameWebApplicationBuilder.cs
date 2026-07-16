// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace InfiniFrame.WebServer;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameWebApplicationBuilder {
    /// <summary>
    ///     Gets the underlying <see cref="WebApplicationBuilder"/> used to configure the ASP.NET Core application.
    /// </summary>
    WebApplicationBuilder WebApp { get; }

    /// <summary>
    ///     Gets the window builder used to configure the application window.
    /// </summary>
    IInfiniFrameWindowBuilder WindowBuilder { get; }

    /// <summary>
    ///     Gets the service collection used to configure application services.
    /// </summary>
    IServiceCollection Services { get; }
}
