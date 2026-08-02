// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using FluentValidation;
using InfiniFrame.NativeBridge.Parameters;
using Microsoft.Extensions.DependencyInjection;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Provides extension methods for registering InfiniFrame services with the Microsoft DI container.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <summary>
    ///     Registers the core InfiniFrame services required for window management, events, and native interop.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <returns>The same service collection so calls can be chained.</returns>
    public static IServiceCollection AddInfiniFrame(this IServiceCollection services) {
        services.AddTransient<IInfiniFrameEvents, InfiniFrameEvents>();
        services.AddTransient<IInfiniFrameEventsStore, InfiniFrameEventsStore>();
        services.AddTransient<IInfiniFrameWindowConfiguration, InfiniFrameWindowConfiguration>();
        services.AddTransient<InfiniFrameWindow>();
        services.AddSingleton<IValidator<InfiniFrameNativeParameters>, InfiniFrameNativeParametersValidator>();

        services.AddSingleton<InfiniFrameWindowFeaturesFactory>();

        return services;
    }
}