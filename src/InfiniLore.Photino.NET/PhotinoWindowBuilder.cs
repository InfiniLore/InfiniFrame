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
    
    public IPhotinoWindow Build(IServiceProvider? provider = null) {
        var window = new PhotinoWindow(CustomSchemeHandlers, provider?.GetService<ILogger<PhotinoWindow>>() ?? GetDefaultLogger());
        
        //These are for the callbacks from C++ to C#.
        PhotinoNativeParameters startupParameters = GetParameters(provider);
        startupParameters.ClosingHandler = Events.OnWindowClosing;
        startupParameters.ResizedHandler = Events.OnSizeChanged;
        startupParameters.MaximizedHandler = Events.OnMaximized;
        startupParameters.RestoredHandler = Events.OnRestored;
        startupParameters.MinimizedHandler = Events.OnMinimized;
        startupParameters.MovedHandler = Events.OnLocationChanged;
        startupParameters.FocusInHandler = Events.OnFocusIn;
        startupParameters.FocusOutHandler = Events.OnFocusOut;
        startupParameters.WebMessageReceivedHandler = Events.OnWebMessageReceived;
        startupParameters.CustomSchemeHandler = window.OnCustomScheme;
        window.StartupParameters = startupParameters;
        
        window.MaxHeight = startupParameters.MaxHeight;
        window.MaxWidth = startupParameters.MaxWidth;
        window.MinHeight = startupParameters.MinHeight;
        window.MinWidth = startupParameters.MinWidth;
        
        window.Events = Events.DefineSender(window);
        window.Initialize();
        return window;

    }
}
