// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Native;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowBuilder : IInfiniFrameWindowBuilder {
    public bool UseDefaultLogger { get; set; } = true;
    public StaticAssetSettings? StaticAssets { get; set; }
    
    public IInfiniFrameWindowConfiguration Configuration { get; } = new InfiniFrameWindowConfiguration();
    public IInfiniFrameWindowEvents Events { get; internal set; } = new InfiniFrameWindowEvents();
    public IInfiniFrameWindowMessageHandlers MessageHandlers { get; } = new InfiniFrameWindowMessageHandlers();
    public Dictionary<string, NetCustomSchemeDelegate?> CustomSchemeHandlers { get; } = [];
    
    private InfiniFrameWindowBuilder() {}

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    public static InfiniFrameWindowBuilder Create() => new();

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    private InfiniFrameNativeParameters GetParameters(IServiceProvider? provider = null) {
        if (provider is null) return Configuration.ToParameters();

        var config = provider.GetService<IConfiguration>();
        IConfigurationSection? section = config?.GetSection("InfiniFrame");

        IInfiniFrameWindowConfiguration configuration = Configuration;
        if (section is not null && section.Exists()) {
            configuration = section.Get<InfiniFrameWindowConfiguration>() ?? Configuration;
        }

        return configuration.ToParameters();
    }

    private ILogger<InfiniFrameWindow> GetDefaultLogger() {
        if (!UseDefaultLogger)
            return LoggerFactory.Create(config => {
                config.ClearProviders();// Remove default console logger
            }).CreateLogger<InfiniFrameWindow>();

        return LoggerFactory.Create(config => {
            config.AddConsole().SetMinimumLevel(LogLevel.Debug);
        }).CreateLogger<InfiniFrameWindow>();
    }

    public IInfiniFrameWindow Build(IServiceProvider? provider = null) {
        if (CustomSchemeHandlers.Count > 16) throw new InvalidOperationException("Maximum number of custom scheme handlers is 16.");

        var window = new InfiniFrameWindow {
            ServiceProvider = provider,
            Logger = provider?.GetService<ILogger<InfiniFrameWindow>>() ?? GetDefaultLogger(),
            CustomSchemes = CustomSchemeHandlers,
            Parent = null,
            Events = Events,
            MessageHandlers = MessageHandlers,
            StaticAssets = StaticAssets
        };

        Events.WebMessageReceived.Add(MessageHandlers.Handle);

        //These are for the callbacks from C++ to C#.
        InfiniFrameNativeParameters startupParameters = GetParameters(provider);
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

        // window.IconFilePath = startupParameters.WindowIconFile;

        Events.CompleteSetup(window);
        window.Initialize();
        return window;

    }
}
