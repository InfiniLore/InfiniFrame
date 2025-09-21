using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InfiniLore.Photino.NET;
public class PhotinoWindowBuilder : IPhotinoWindowBuilder {
    public bool UseDefaultLogger { get; set; } = true;
    public IPhotinoConfiguration Configuration { get; } = new PhotinoConfiguration();
    
    private PhotinoWindowBuilder() {}

    public static PhotinoWindowBuilder Create() {
        return new PhotinoWindowBuilder();
    }

    private PhotinoNativeParameters GetParameters(IServiceProvider? provider = null) {
        if (provider is null) return Configuration.ToParameters();
        
        var config = provider.GetService<IConfiguration>();
        var photinoConfiguration = config?.Get<IPhotinoConfiguration>();
        if (photinoConfiguration is not null) return photinoConfiguration.ToParameters();

        return Configuration.ToParameters();
    }

    private ILogger<PhotinoWindow> GetDefaultLogger() {
        if (!UseDefaultLogger)
            return LoggerFactory.Create(config => {
                config.ClearProviders();// Remove default console logger
            }).CreateLogger<PhotinoWindow>();

        return LoggerFactory.Create(config => {
            config.AddConsole().SetMinimumLevel(LogLevel.Debug);
        }).CreateLogger<PhotinoWindow>();
    }

    public IPhotinoWindow Build() {
        return new PhotinoWindow(GetParameters(), GetDefaultLogger());
    }
    public IPhotinoWindow Build(IServiceProvider provider) {
        return new PhotinoWindow(GetParameters(provider), provider.GetService<ILogger<PhotinoWindow>>() ?? GetDefaultLogger());
    }
}
