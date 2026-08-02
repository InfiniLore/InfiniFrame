// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.DependencyInjection;

namespace InfiniFrame.BlazorWebView;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public interface IInfiniFrameBlazorAppBuilder {
    /// <summary>
    ///     Gets the list of root components to be added to the Blazor application.
    /// </summary>
    IInfiniFrameRootComponentList RootComponents { get; }

    /// <summary>
    ///     Gets the service collection used to configure application services.
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    ///     Gets the window builder used to configure the application window.
    /// </summary>
    IInfiniFrameWindowBuilder WindowBuilder { get; }
}