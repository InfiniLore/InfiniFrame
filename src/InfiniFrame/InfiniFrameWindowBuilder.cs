// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.BuilderSnapshots;
using InfiniFrame.Configuration;
using InfiniFrame.Native;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowBuilder : IInfiniFrameWindowBuilder {
    private readonly InfiniFrameWindowNativeParameterBuilder _configuration = new();
    public IInfiniFrameWindowNativeParameterBuilder Configuration => _configuration;

    private InfiniFrameWindowEvents _events = new();
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
    public static InfiniFrameWindowBuilder Create(InfiniFrameWindowEvents? events = null) => new() {
        _events = events ?? new InfiniFrameWindowEvents()
    };

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
        InfiniFrameWindowEvents events = InfiniFrameWindowEvents.FromSnapshot(snapshot.Events);
        InfiniFrameWindowMessageHandlers messageHandlers = InfiniFrameWindowMessageHandlers.FromSnapshot(snapshot.MessageHandlers);
        InfiniFrameWindowCustomSchemeHandlers customSchemes = InfiniFrameWindowCustomSchemeHandlers.FromSnapshot(snapshot.CustomSchemes);

        var window = new InfiniFrameWindow {
            ServiceProvider = provider,
            Logger = provider?.GetService<ILogger<InfiniFrameWindow>>() ?? GetDefaultLogger(),
            CustomSchemes = customSchemes,
            Parent = null,
            Events = events,
            MessageHandlers = messageHandlers,
            StaticAssets = snapshot.StaticAssets
        };

        InfiniFrameNativeParameters startupParameters = snapshot.StartupParameters;
        // Rebind callbacks to the per-window event instance that has Sender set via CompleteSetup.
        startupParameters.ClosingHandler = events.OnWindowClosing;
        startupParameters.ResizedHandler = events.OnSizeChanged;
        startupParameters.MaximizedHandler = events.OnMaximized;
        startupParameters.RestoredHandler = events.OnRestored;
        startupParameters.MinimizedHandler = events.OnMinimized;
        startupParameters.MovedHandler = events.OnLocationChanged;
        startupParameters.FocusInHandler = events.OnFocusIn;
        startupParameters.FocusOutHandler = events.OnFocusOut;
        startupParameters.WebMessageReceivedHandler = events.OnWebMessageReceived;
        startupParameters.CustomSchemeHandler = window.OnCustomScheme;
        window.StartupParameters = startupParameters;
        
        events.CompleteSetup(window);
        window.Initialize();
        return window;

    }

    internal InfiniFrameWindowBuildSnapshot CreateSnapshot(IServiceProvider? provider = null) {
        if (CustomSchemeHandlers.Length > 16) throw new InvalidOperationException("Maximum number of custom scheme handlers is 16.");

        InfiniFrameWindowMessageHandlersSnapshot messageHandlersSnapshot = _messageHandlers.ToSnapshot();
        InfiniFrameWindowMessageHandlers messageHandlers = InfiniFrameWindowMessageHandlers.FromSnapshot(messageHandlersSnapshot);

        InfiniFrameWindowEventsSnapshot eventsSnapshot = AddWebMessageHandler(_events.ToSnapshot(), messageHandlers.Handle);
        InfiniFrameWindowCustomSchemeHandlersSnapshot customSchemesSnapshot = _customSchemeHandlers.ToSnapshot();

        InfiniFrameWindowEvents events = InfiniFrameWindowEvents.FromSnapshot(eventsSnapshot);

        // These are callbacks from C++ to C# and must reference the per-window snapshot.
        InfiniFrameNativeParameters startupParameters = GetParameters(provider);
        ApplyCustomSchemeNames(ref startupParameters, customSchemesSnapshot);
        startupParameters.ClosingHandler = events.OnWindowClosing;
        startupParameters.ResizedHandler = events.OnSizeChanged;
        startupParameters.MaximizedHandler = events.OnMaximized;
        startupParameters.RestoredHandler = events.OnRestored;
        startupParameters.MinimizedHandler = events.OnMinimized;
        startupParameters.MovedHandler = events.OnLocationChanged;
        startupParameters.FocusInHandler = events.OnFocusIn;
        startupParameters.FocusOutHandler = events.OnFocusOut;
        startupParameters.WebMessageReceivedHandler = events.OnWebMessageReceived;

        return new InfiniFrameWindowBuildSnapshot(
            startupParameters,
            eventsSnapshot,
            messageHandlersSnapshot,
            customSchemesSnapshot,
            StaticAssets);
    }

    private static InfiniFrameWindowEventsSnapshot AddWebMessageHandler(
        InfiniFrameWindowEventsSnapshot snapshot,
        Action<IInfiniFrameWindow, string> handler
    ) {
        Action<IInfiniFrameWindow, string>[] handlersWithBridge = [..snapshot.WebMessageReceived, handler];

        return snapshot with {
            WebMessageReceived = handlersWithBridge
        };
    }

    private static void ApplyCustomSchemeNames(ref InfiniFrameNativeParameters startupParameters, InfiniFrameWindowCustomSchemeHandlersSnapshot customSchemesSnapshot) {
        var customSchemeNameArray = new IntPtr[16];
        var index = 0;
        var availableHandlers = new HashSet<string>(customSchemesSnapshot.Handlers.Select(static item => item.Key), StringComparer.Ordinal);
        var seen = new HashSet<string>(StringComparer.Ordinal);
        
        foreach (string key in customSchemesSnapshot.OrderedSchemeNames.Where(key => seen.Add(key) && availableHandlers.Contains(key))) {
            if (index >= customSchemeNameArray.Length) {
                throw new InvalidOperationException("Maximum number of custom schemes is 16.");
            }

            customSchemeNameArray[index] = Marshal.StringToHGlobalAnsi(key);
            index++;
        }

        startupParameters.CustomSchemeNames = customSchemeNameArray;
    }
}
