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

    public IInfiniFrameWindowConfiguration Configuration { get; } = new InfiniFrameWindowConfiguration();
    public IInfiniFrameWindowEvents Events { get; } = new InfiniFrameWindowEvents();
    public IInfiniFrameWindowMessageHandlers MessageHandlers { get; } = new InfiniFrameWindowMessageHandlers();
    public Dictionary<string, NetCustomSchemeDelegate?> CustomSchemeHandlers { get; } = [];

    private InfiniFrameWindowBuilder() {}

    public static InfiniFrameWindowBuilder Create() => new();

    private InfiniFrameNativeParameters GetParameters(IServiceProvider? provider = null) {
        if (provider is null) return Configuration.ToParameters();

        var config = provider.GetService<IConfiguration>();
        IConfigurationSection? section = config?.GetSection("InfiniFrame");
        
        IInfiniFrameWindowConfiguration configuration = Configuration;
        if (section.Exists()) {
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
        #pragma warning disable CA2208
        if (CustomSchemeHandlers.Count > 16) throw new ArgumentOutOfRangeException(nameof(CustomSchemeHandlers), "Maximum number of custom scheme handlers is 16.");
        #pragma warning restore CA2208

        var window = new InfiniFrameWindow(
            CustomSchemeHandlers,
            provider?.GetService<ILogger<InfiniFrameWindow>>() ?? GetDefaultLogger()
        );

        Events.WebMessageReceived += MessageHandlers.Handle;

        //These are for the callbacks from C++ to C#.
        InfiniFrameNativeParameters startupParameters = GetParameters(provider);
        startupParameters.ClosingHandler = Events.InvokeOnWindowClosing;
        startupParameters.ResizedHandler = Events.InvokeOnSizeChanged;
        startupParameters.MaximizedHandler = Events.InvokeOnMaximized;
        startupParameters.RestoredHandler = Events.InvokeOnRestored;
        startupParameters.MinimizedHandler = Events.InvokeOnMinimized;
        startupParameters.MovedHandler = Events.InvokeOnLocationChanged;
        startupParameters.FocusInHandler = Events.InvokeOnFocusIn;
        startupParameters.FocusOutHandler = Events.InvokeOnFocusOut;
        startupParameters.WebMessageReceivedHandler = Events.InvokeOnWebMessageReceived;
        startupParameters.CustomSchemeHandler = window.OnCustomScheme;
        window.StartupParameters = startupParameters;

        window.MaxHeight = startupParameters.MaxHeight;
        window.MaxWidth = startupParameters.MaxWidth;
        window.MinHeight = startupParameters.MinHeight;
        window.MinWidth = startupParameters.MinWidth;

        // window.IconFilePath = startupParameters.WindowIconFile;

        window.Events = Events.DefineSender(window);
        window.MessageHandlers = MessageHandlers;
        window.Initialize();
        return window;

    }
}
