// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Blazor;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class ServiceCollectionExtensions {
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static IServiceCollection AddInfiniFrameJs(this IServiceCollection services) {
        services.AddScoped<IInfiniFrameJs, InfiniFrameJs>();
        return services;
    }
}
