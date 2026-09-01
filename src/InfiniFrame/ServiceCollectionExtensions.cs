// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using FluentValidation;
using InfiniFrame.Interop;
using InfiniFrame.NativeBridge.Parameters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Provides extension methods for registering InfiniFrame services with the Microsoft DI container.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <summary>
    ///     Registers the core InfiniFrame services required for application management, window management, events, and native interop.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="configure">Optional callback to configure the application settings.</param>
    /// <returns>The same service collection so calls can be chained.</returns>
    public static IServiceCollection AddInfiniFrame(this IServiceCollection services, Action<ApplicationConfiguration>? configure = null) {
        services.AddSingleton<IInfiniFrameApplication>(sp => {
            var logger = sp.GetRequiredService<ILogger<InfiniFrameApplication>>();
            var app = new InfiniFrameApplication(logger);
            if (configure is not null) {
                var config = new ApplicationConfiguration();
                configure(config);
                app.Initialize(config);
            }
            return app;
        });
        services.AddSingleton<IInfiniFrameEvents, InfiniFrameEvents>();
        services.AddSingleton<IInfiniFrameEventsStore, InfiniFrameEventsStore>();
        services.AddSingleton<IInfiniFrameWindowConfiguration, InfiniFrameWindowConfiguration>();
        services.AddSingleton<IValidator<InfiniFrameNativeParameters>, InfiniFrameNativeParametersValidator>();

        services.AddSingleton<InfiniFrameWindowFeaturesFactory>();
        services.AddSingleton<IExternalProcessLauncher, ExternalProcessLauncher>();

        return services;
    }
}
