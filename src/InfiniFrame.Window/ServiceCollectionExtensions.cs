// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using FluentValidation;
using InfiniFrame.NativeBridge.Parameters;
using InfiniFrame.Window.Events;
using InfiniFrame.Window.Interop;
using Microsoft.Extensions.DependencyInjection;

namespace InfiniFrame.Window;
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
        services.AddSingleton<IInfiniFrameEvents, InfiniFrameEvents>();
        services.AddSingleton<IInfiniFrameEventsStore, InfiniFrameEventsStore>();
        services.AddSingleton<IInfiniFrameWindowConfiguration, InfiniFrameWindowConfiguration>();
        services.AddTransient<InfiniFrameWindow>();
        services.AddSingleton<IValidator<InfiniFrameNativeWindowParameters>, InfiniFrameNativeWindowParametersValidator>();

        services.AddSingleton<InfiniFrameWindowFeaturesFactory>();
        services.AddSingleton<IExternalProcessLauncher, ExternalProcessLauncher>();

        return services;
    }
}
