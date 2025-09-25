using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InfiniLore.Photino.NET;
public class PhotinoWindowBuilder : IPhotinoWindowBuilder {
    public bool UseDefaultLogger { get; set; } = true;
    public IPhotinoConfiguration Configuration { get; } = new PhotinoConfiguration();
    public IPhotinoWindowEvents Events { get; } = new PhotinoWindowEvents();
    public Dictionary<string, NetCustomSchemeDelegate?> CustomSchemeHandlers { get; } = [];
    
    private PhotinoWindowBuilder() {}

    public static PhotinoWindowBuilder Create() {
        return new PhotinoWindowBuilder();
    }

    private PhotinoNativeParameters GetParameters(IServiceProvider? provider = null) {
        if (provider is null) return Configuration.ToParameters();
        
        var config = provider.GetService<IConfiguration>();
        var photinoConfiguration = config?.Get<IPhotinoConfiguration>();
        return photinoConfiguration?.ToParameters() ?? Configuration.ToParameters();
    }

    private ILogger<PhotinoWindow> GetDefaultLogger() {
        if (!UseDefaultLogger)
            return LoggerFactory.Create(config => {
                config.ClearProviders(); // Remove default console logger
            }).CreateLogger<PhotinoWindow>();

        return LoggerFactory.Create(config => {
            config.AddConsole().SetMinimumLevel(LogLevel.Debug);
        }).CreateLogger<PhotinoWindow>();
    }

    public IPhotinoWindow Build() {
        return new PhotinoWindow(Events, GetParameters(), CustomSchemeHandlers, GetDefaultLogger());
    }
    
    public IPhotinoWindow Build(IServiceProvider provider) {
        return new PhotinoWindow(Events, GetParameters(provider), CustomSchemeHandlers, provider.GetService<ILogger<PhotinoWindow>>() ?? GetDefaultLogger());
    }
}
