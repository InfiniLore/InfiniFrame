// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.DependencyInjection;

namespace InfiniFrame.BlazorWebView;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameBlazorAppBuilder {
    IInfiniFrameRootComponentList RootComponents { get; }
    IServiceCollection Services { get; }
    IInfiniFrameWindowBuilder WindowBuilder { get; }
}
