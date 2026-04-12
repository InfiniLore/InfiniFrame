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
    private InfiniFrameWindowEvents _events = new();
    private readonly InfiniFrameWindowMessageHandlers _messageHandlers = new();
    private readonly Dictionary<string, NetCustomSchemeDelegate?> _customSchemeHandlers = [];

    public StaticAssetSettings? StaticAssets { get; set; }

    public IInfiniFrameWindowNativeParameterBuilder Configuration => _configuration;

    public IInfiniFrameWindowEvents Events {
        get => _events;
        internal set => _events = value as InfiniFrameWindowEvents
            ?? throw new ArgumentException($"{nameof(Events)} must be of type {nameof(InfiniFrameWindowEvents)}.", nameof(value));
    }

    public IInfiniFrameWindowMessageHandlers MessageHandlers => _messageHandlers;

    public Dictionary<string, NetCustomSchemeDelegate?> CustomSchemeHandlers => _customSchemeHandlers;

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
        if (_customSchemeHandlers.Count > 16) throw new InvalidOperationException("Maximum number of custom scheme handlers is 16.");

        var eventsSnapshot = new InfiniFrameWindowEvents(_events);
        var messageHandlersSnapshot = new InfiniFrameWindowMessageHandlers(_messageHandlers);
        var customSchemesSnapshot = new Dictionary<string, NetCustomSchemeDelegate?>(_customSchemeHandlers);

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
