// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Blazor;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Extension methods for registering InfiniFrame.Blazor services.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <summary>
    ///     Registers <see cref="IInfiniFrameJs"/> services for Blazor component interop.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static IServiceCollection AddInfiniFrameJs(this IServiceCollection services) {
        services.AddScoped<IInfiniFrameJs, InfiniFrameJs>();
        return services;
    }
}
