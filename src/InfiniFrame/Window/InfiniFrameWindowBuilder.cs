// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.BuilderSnapshots;
using InfiniFrame.Configuration;
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
    private InfiniFrameWindowBuilder() {}
    public IInfiniFrameWindowNativeParameterBuilder Configuration => _configuration;

    public StaticAssetSettings? StaticAssets { get; set; }
    
    public IInfiniFrameWindowEventsStore EventsStore { get; init; } = new InfiniFrameWindowEventsStore();

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    public static InfiniFrameWindowBuilder Create(InfiniFrameWindowEventsStore? events = null) {
        var builder = new InfiniFrameWindowBuilder {
            EventsStore = events ?? new InfiniFrameWindowEventsStore()
        };

        return builder;
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public IInfiniFrameWindow Build(IServiceProvider? provider = null) {
        InfiniFrameWindowBuildSnapshot snapshot = CreateSnapshot(provider);
        var events = new InfiniFrameWindowEvents(snapshot.EventsStore.DeepCopy());
        
        var window = new InfiniFrameWindow {
            ServiceProvider = provider,
            Logger = ResolveLogger(provider),
            Parent = null,
            Events = events,
            StaticAssets = snapshot.StaticAssets
        };
        InfiniFrameUriSecurityPolicyRegistry.BindToWindow(window, snapshot.UriSecurityPolicy);

        InfiniFrameNativeParameters startupParameters = snapshot.StartupParameters;
        events.CompleteSetup(window, ref startupParameters);
        
        window.StartupParameters = startupParameters;
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
        return new InfiniFrameWindowBuildSnapshot(
            GetParameters(provider),
            EventsStore.DeepCopy(),
            StaticAssets,
            InfiniFrameUriSecurityPolicyRegistry.GetForBuilder(this));
    }
}
