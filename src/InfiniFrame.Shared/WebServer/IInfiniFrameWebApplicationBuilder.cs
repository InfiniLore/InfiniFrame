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
    WebApplicationBuilder WebApp { get; }
    IInfiniFrameWindowBuilder WindowBuilder { get; }
    
    IServiceCollection Services { get; }
}
