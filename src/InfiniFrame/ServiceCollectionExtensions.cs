// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using FluentValidation;
using InfiniFrame.Debugging;
using InfiniFrame.NativeBridge.Parameters;
using Microsoft.Extensions.DependencyInjection;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class ServiceCollectionExtensions {
    public static IServiceCollection AddInfiniFrame(this IServiceCollection services) {
        services.AddTransient<IInfiniFrameEvents, InfiniFrameEvents>();
        services.AddTransient<IInfiniFrameEventsStore, InfiniFrameEventsStore>();
        services.AddTransient<IInfiniFrameWindowDebugging, InfiniFrameWindowDebugging>();
        services.AddTransient<IInfiniFrameWindowConfiguration, InfiniFrameWindowConfiguration>();
        services.AddTransient<InfiniFrameWindow>();
        services.AddSingleton<IValidator<InfiniFrameNativeParameters>, InfiniFrameNativeParametersValidator>();
        
        services.AddSingleton<InfiniFrameWindowFeaturesFactory>();
        
        return services;
    }
}
