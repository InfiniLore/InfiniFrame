// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Configuration;
using InfiniFrame.Native;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowBuilder : IInfiniFrameWindowBuilder {
    private readonly InfiniFrameWindowNativeParameterBuilder _configuration = new();
    public IInfiniFrameWindowNativeParameterBuilder Configuration => _configuration;

    private readonly InfiniFrameWindowEvents _events = new();
    public IInfiniFrameWindowEvents Events => _events;

    private readonly InfiniFrameWindowMessageHandlers _messageHandlers = new();
    public IInfiniFrameWindowMessageHandlers MessageHandlers => _messageHandlers;

    public StaticAssetSettings? StaticAssets { get; set; }

    private readonly InfiniFrameWindowCustomSchemeHandlers _customSchemeHandlers = new();
    public IInfiniFrameWindowCustomSchemeHandlers CustomSchemeHandlers => _customSchemeHandlers;

    private InfiniFrameWindowBuilder() {}

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    public static InfiniFrameWindowBuilder Create() => new();

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    private InfiniFrameNativeParameters GetParameters(IServiceProvider? provider = null) {
        if (provider is null) return _configuration.ToNativeParameters();

        var config = provider.GetService<IConfiguration>();
        IConfigurationSection? section = config?.GetSection("InfiniFrame");

        if (section is not null && section.Exists()) {
            InfiniFrameWindowNativeParameterSectionApplier.Apply(section, _configuration);
        }

        return _configuration.ToNativeParameters();
    }

    private static ILogger<InfiniFrameWindow> GetDefaultLogger() 
        => LoggerFactory.Create(config => {
            config.AddConsole().SetMinimumLevel(LogLevel.Debug);
        }).CreateLogger<InfiniFrameWindow>();

    public IInfiniFrameWindow Build(IServiceProvider? provider = null) {
        InfiniFrameWindowBuildSnapshot snapshot = CreateSnapshot(provider);

        var window = new InfiniFrameWindow {
            ServiceProvider = provider,
            Logger = provider?.GetService<ILogger<InfiniFrameWindow>>() ?? GetDefaultLogger(),
            CustomSchemes = snapshot.CustomSchemes,
            Parent = null,
            Events = snapshot.Events,
            MessageHandlers = snapshot.MessageHandlers,
            StaticAssets = snapshot.StaticAssets
        };

        InfiniFrameNativeParameters startupParameters = snapshot.StartupParameters;
        startupParameters.CustomSchemeHandler = window.OnCustomScheme;
        window.StartupParameters = startupParameters;
        
        snapshot.Events.CompleteSetup(window);
        window.Initialize();
        return window;

    }

    internal InfiniFrameWindowBuildSnapshot CreateSnapshot(IServiceProvider? provider = null) {
        if (CustomSchemeHandlers.Length > 16) throw new InvalidOperationException("Maximum number of custom scheme handlers is 16.");

        InfiniFrameWindowEvents eventsSnapshot = InfiniFrameWindowEvents.CopyFrom(_events);
        InfiniFrameWindowMessageHandlers messageHandlersSnapshot = InfiniFrameWindowMessageHandlers.CopyFrom(_messageHandlers);
        InfiniFrameWindowCustomSchemeHandlers customSchemesSnapshot = InfiniFrameWindowCustomSchemeHandlers.CopyFrom(_customSchemeHandlers);

        eventsSnapshot.WebMessageReceived.Add(messageHandlersSnapshot.Handle);

        // These are callbacks from C++ to C# and must reference the per-window snapshot.
        InfiniFrameNativeParameters startupParameters = GetParameters(provider);
        startupParameters.ClosingHandler = eventsSnapshot.OnWindowClosing;
        startupParameters.ResizedHandler = eventsSnapshot.OnSizeChanged;
        startupParameters.MaximizedHandler = eventsSnapshot.OnMaximized;
        startupParameters.RestoredHandler = eventsSnapshot.OnRestored;
        startupParameters.MinimizedHandler = eventsSnapshot.OnMinimized;
        startupParameters.MovedHandler = eventsSnapshot.OnLocationChanged;
        startupParameters.FocusInHandler = eventsSnapshot.OnFocusIn;
        startupParameters.FocusOutHandler = eventsSnapshot.OnFocusOut;
        startupParameters.WebMessageReceivedHandler = eventsSnapshot.OnWebMessageReceived;

        return new InfiniFrameWindowBuildSnapshot(
            startupParameters,
            eventsSnapshot,
            messageHandlersSnapshot,
            customSchemesSnapshot,
            StaticAssets);
    }
}
