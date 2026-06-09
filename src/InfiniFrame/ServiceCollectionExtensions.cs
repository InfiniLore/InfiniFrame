// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.DependencyInjection;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class ServiceCollectionExtensions {
    public static IServiceCollection AddInfiniFrame(this IServiceCollection services) {

        services.AddSingleton<InfiniFrameWindowFeaturesFactory>();
        
        services.AddTransient<IInfiniFrameWindowFeatures>(sp => {
            var window = sp.GetRequiredService<IInfiniFrameWindow>();
            var factory = sp.GetRequiredService<InfiniFrameWindowFeaturesFactory>();
            return factory.Create(window);
        });
        
        return services;
    }
}
