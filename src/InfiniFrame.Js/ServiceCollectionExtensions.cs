// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.DependencyInjection;

namespace InfiniFrame.Js;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class ServiceCollectionExtensions {
    public static IServiceCollection AddInfiniFrameJs(this IServiceCollection services) {
        services.AddScoped<IInfiniFrameJs, InfiniFrameJs>();
        services.AddSingleton<IInfiniFrameGetMessageService, InfiniFrameGetMessageService>();
        return services;
    }
}
