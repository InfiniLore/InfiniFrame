// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Native;
using InfiniFrame.Security;
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

    public IInfiniFrameOptionsBuilder Configuration { get; } = new InfiniFrameOptionsBuilder();
    public IInfiniFrameEventsStore EventsStore { get; private init; } = new InfiniFrameEventsStore();
    
    public IInfiniFrameStaticAssets? StaticAssets { get; set; }

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    private InfiniFrameWindowBuilder() {}
    
    public static InfiniFrameWindowBuilder Create(InfiniFrameEventsStore? events = null) {
        var builder = new InfiniFrameWindowBuilder {
            EventsStore = events ?? new InfiniFrameEventsStore()
        };

        return builder;
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public IInfiniFrameWindow Build(IServiceProvider? provider = null) {
        // ReSharper disable once UseDeconstruction
        InfiniFrameWindowBuilderSnapshot snapshot = CreateSnapshot(provider);
        
        InfiniFrameNativeParameters nativeParameters = snapshot.StartupParameters;
        var events = new InfiniFrameEvents(snapshot.EventsStore);
        events.AssignEventCallbacks(ref nativeParameters);

        var configuration = new InfiniFrameOptions(Configuration, ref nativeParameters);
        
        var window = new InfiniFrameWindow {
            ServiceProvider = provider,
            Logger = ResolveLogger(provider),
            Events = events,
            StaticAssets = snapshot.StaticAssets,
            Configuration = configuration
        };
        
        InfiniFrameUriSecurityPolicyRegistry.BindToWindow(window, snapshot.UriSecurityPolicy);
        
        events.AssignSender(window);
        window.Initialize();
        
        return window;

    }

    private InfiniFrameNativeParameters GetNativeParameters(IServiceProvider? provider = null) {
        if (provider is null) return Configuration.ToNativeParameters();

        var config = provider.GetService<IConfiguration>();
        IConfigurationSection? section = config?.GetSection("InfiniFrame");

        if (section is not null && section.Exists()) {
            InfiniFrameOptionsSectionApplier.Apply(section, Configuration);
        }

        return Configuration.ToNativeParameters();
    }

    internal static ILogger<IInfiniFrameWindow> ResolveLogger(IServiceProvider? provider) {
        if (provider is null) return FallbackLogger;

        return provider.GetService<ILogger<IInfiniFrameWindow>>()
            ?? provider.GetService<ILoggerFactory>()?.CreateLogger<IInfiniFrameWindow>()
            ?? FallbackLogger;
    }

    internal InfiniFrameWindowBuilderSnapshot CreateSnapshot(IServiceProvider? provider = null) {
        return new InfiniFrameWindowBuilderSnapshot(
            GetNativeParameters(provider),
            EventsStore.DeepCopy(),
            StaticAssets?.DeepCopy(),
            InfiniFrameUriSecurityPolicyRegistry.GetForBuilder(this));
    }
}
