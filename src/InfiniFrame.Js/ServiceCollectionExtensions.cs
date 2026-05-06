// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.BlazorWebView;
using InfiniFrame.WebServer;
using Microsoft.Extensions.DependencyInjection;

namespace InfiniFrame.Js;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class ServiceCollectionExtensions {
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static IServiceCollection AddInfiniFrameJs(this IServiceCollection services) {
        services.AddScoped<IInfiniFrameJs, InfiniFrameJs>();
        return services;
    }

    // ReSharper disable once UnusedMethodReturnValue.Global
    public static IInfiniFrameWebApplicationBuilder AddInfiniFrameJs(this IInfiniFrameWebApplicationBuilder builder) {
        // builder.Services.AddInfiniFrameJs(); // Cannot be added to a WebApp only, as the JSRuntime is Blazor specific
        builder.WindowBuilder.RegisterGetWebMessageHandler();
        return builder;
    }
    
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static IInfiniFrameBlazorAppBuilder AddInfiniFrameJs(this IInfiniFrameBlazorAppBuilder builder) {
        builder.Services.AddInfiniFrameJs();
        builder.WindowBuilder.RegisterGetWebMessageHandler();
        return builder;
    }
}
