// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Debugging;
using InfiniFrame.NativeBridge.Parameters;
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
    public IInfiniFrameOptionsBuilder Configuration { get; } = new InfiniFrameOptionsBuilder();
    public IInfiniFrameWindowDebuggingBuilder Debugging => Configuration.Debugging;
    public IInfiniFrameEventsStore EventsStore { get; private init; } = new InfiniFrameEventsStore();
    
    public IInfiniFrameStaticAssets? StaticAssets { get; set; }

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    
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
        events.AssignDefaultEventCallbacks();

        var configuration = new InfiniFrameOptions(Configuration, ref nativeParameters);
        var debugging = new InfiniFrameWindowDebugging(
            ResolveLogger<InfiniFrameWindowDebugging>(provider)
        );
        
        var window = new InfiniFrameWindow {
            ServiceProvider = provider,
            Logger = ResolveLogger<InfiniFrameWindow>(provider),
            Events = events,
            Debugging = debugging,
            StaticAssets = snapshot.StaticAssets,
            Configuration = configuration
        };
        
        InfiniFrameUriSecurityPolicyRegistry.BindToWindow(window, snapshot.UriSecurityPolicy);
        
        events.AssignToWindow(window);
        debugging.AssignToWindow(window);
        window.Initialize();
        
        return window;

    }

    private InfiniFrameNativeParameters GetNativeParameters(IServiceProvider? provider = null) {
        if (provider is not null) {
            var config = provider.GetService<IConfiguration>();
            IConfigurationSection? section = config?.GetSection("InfiniFrame");

            if (section is not null && section.Exists()) {
                InfiniFrameOptionsSectionApplier.Apply(section, Configuration);
            }
        }
        return Configuration.ToNativeParameters();
    }

    internal static ILogger<T> ResolveLogger<T>(IServiceProvider? provider) {
        if (provider is null) return NullLogger<T>.Instance;

        return provider.GetService<ILogger<T>>()
            ?? provider.GetService<ILoggerFactory>()?.CreateLogger<T>()
            ?? NullLogger<T>.Instance;
    }

    internal InfiniFrameWindowBuilderSnapshot CreateSnapshot(IServiceProvider? provider = null) 
        => new(
            GetNativeParameters(provider),
            EventsStore.DeepCopy(),
            StaticAssets?.DeepCopy(),
            InfiniFrameUriSecurityPolicyRegistry.GetForBuilder(this)
        );
}
