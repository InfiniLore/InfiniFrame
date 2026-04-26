// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.BuilderSnapshots;
using InfiniFrame.Configuration;
using InfiniFrame.Js;
using InfiniFrame.Native;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowBuilder : IInfiniFrameWindowBuilder {
    private static readonly ILogger<IInfiniFrameWindow> FallbackLogger = NullLogger<IInfiniFrameWindow>.Instance;

    private readonly InfiniFrameWindowNativeParameterBuilder _configuration = new();
    private readonly InfiniFrameWindowCustomSchemeHandlers _customSchemeHandlers = new();
    private readonly InfiniFrameWindowMessageHandler _messageHandlers = new();

    private InfiniFrameWindowEvents _events = new();

    private InfiniFrameWindowBuilder() {}
    public IInfiniFrameWindowNativeParameterBuilder Configuration => _configuration;
    public IInfiniFrameWindowEvents Events => _events;
    public IInfiniFrameWindowMessageHandler MessageHandlers => _messageHandlers;

    public StaticAssetSettings? StaticAssets { get; set; }
    public IInfiniFrameWindowCustomSchemeHandlers CustomSchemeHandlers => _customSchemeHandlers;

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    public static InfiniFrameWindowBuilder Create(InfiniFrameWindowEvents? events = null) {
        var builder = new InfiniFrameWindowBuilder {
            _events = events ?? new InfiniFrameWindowEvents()
        };

        builder.MessageHandlers.RegisterHandler(HandlerNames.GetMessageRequest, InfiniFrameWindowMessageHandler.HandleMessageRequest);
        return builder;
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public IInfiniFrameWindow Build(IServiceProvider? provider = null) {
        InfiniFrameWindowBuildSnapshot snapshot = CreateSnapshot(provider);
        InfiniFrameWindowEvents events = InfiniFrameWindowEvents.FromSnapshot(snapshot.Events);
        InfiniFrameWindowMessageHandler messageHandlers = InfiniFrameWindowMessageHandler.FromSnapshot(snapshot.MessageHandlers);
        InfiniFrameWindowCustomSchemeHandlers customSchemes = InfiniFrameWindowCustomSchemeHandlers.FromSnapshot(snapshot.CustomSchemes);

        var window = new InfiniFrameWindow {
            ServiceProvider = provider,
            Logger = ResolveLogger(provider),
            CustomSchemes = customSchemes,
            Parent = null,
            Events = events,
            MessageHandlers = messageHandlers,
            StaticAssets = snapshot.StaticAssets
        };
        InfiniFrameUriSecurityPolicyRegistry.BindToWindow(window, snapshot.UriSecurityPolicy);

        InfiniFrameNativeParameters startupParameters = snapshot.StartupParameters;
        // Rebind callbacks to the per-window event instance that has Sender set via CompleteSetup.
        startupParameters.ClosingHandler = window.OnWindowClosing;
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

    private InfiniFrameNativeParameters GetParameters(IServiceProvider? provider = null) {
        if (provider is null) return _configuration.ToNativeParameters();

        var config = provider.GetService<IConfiguration>();
        IConfigurationSection? section = config?.GetSection("InfiniFrame");

        if (section is not null && section.Exists()) {
            InfiniFrameWindowNativeParameterSectionApplier.Apply(section, _configuration);
        }

        return _configuration.ToNativeParameters();
    }

    internal static ILogger<IInfiniFrameWindow> ResolveLogger(IServiceProvider? provider) {
        if (provider is null) return FallbackLogger;

        return provider.GetService<ILogger<IInfiniFrameWindow>>()
            ?? provider.GetService<ILoggerFactory>()?.CreateLogger<IInfiniFrameWindow>()
            ?? FallbackLogger;
    }

    internal InfiniFrameWindowBuildSnapshot CreateSnapshot(IServiceProvider? provider = null) {
        if (CustomSchemeHandlers.Length > CustomSchemeNameMemory.MaxCustomSchemeNames)
            throw new InvalidOperationException("Maximum number of custom scheme handlers is 16.");

        InfiniFrameWindowMessageHandlersSnapshot messageHandlersSnapshot = _messageHandlers.ToSnapshot();
        InfiniFrameWindowEventsSnapshot eventsSnapshot = AddWebMessageHandler(_events.ToSnapshot());
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
            StaticAssets,
            InfiniFrameUriSecurityPolicyRegistry.GetForBuilder(this));
    }

    private static InfiniFrameWindowEventsSnapshot AddWebMessageHandler(
        InfiniFrameWindowEventsSnapshot snapshot
    ) {
        Action<IInfiniFrameWindow, string>[] handlersWithBridge = [
            ..snapshot.WebMessageReceived, 
            InfiniFrameWindowMessageHandler.HandleMessageRequest
        ];

        return snapshot with {
            WebMessageReceived = handlersWithBridge
        };
    }

    private static void ApplyCustomSchemeNames(ref InfiniFrameNativeParameters startupParameters, InfiniFrameWindowCustomSchemeHandlersSnapshot customSchemesSnapshot) {
        var availableHandlers = new HashSet<string>(customSchemesSnapshot.Handlers.Select(static item => item.Key), StringComparer.Ordinal);
        var seen = new HashSet<string>(StringComparer.Ordinal);

        IntPtr[] customSchemeNameArray = CustomSchemeNameMemory.Allocate(
            customSchemesSnapshot.OrderedSchemeNames.Where(key => seen.Add(key) && availableHandlers.Contains(key))
        );

        CustomSchemeNameMemory.FreeAll(startupParameters.CustomSchemeNames);
        startupParameters.CustomSchemeNames = customSchemeNameArray;
    }
}
