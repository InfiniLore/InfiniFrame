// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Debugging;
using InfiniFrame.NativeBridge.Parameters;
using InfiniFrame.Security;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowBuilder : IInfiniFrameWindowBuilder {
    public IInfiniFrameWindowBuilderConfiguration Configuration { get; } = new InfiniFrameWindowBuilderConfiguration();
    public IInfiniFrameWindowDebuggingBuilder Debugging { get; } = new InfiniFrameWindowDebuggingBuilder();
    public IInfiniFrameWindowBuilderFeatures Features { get; } = new InfiniFrameWindowBuilderFeatures();
    public IInfiniFrameEventsStore EventsStore { get; private init; } = new InfiniFrameEventsStore();
    
    public IInfiniFrameStaticAssets? StaticAssets { get; set; }
    
    public IServiceCollection Services { get; private init; } = new ServiceCollection().AddInfiniFrame();

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    
    public static InfiniFrameWindowBuilder Create(IServiceCollection? collection = null, InfiniFrameEventsStore? events = null) {
        var builder = new InfiniFrameWindowBuilder {
            EventsStore = events ?? new InfiniFrameEventsStore(),
            Services = (collection ?? new ServiceCollection()).AddInfiniFrame(),
        };

        return builder;
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public IInfiniFrameWindow Build(IServiceProvider? provider = null) {
        IServiceProvider actualProvider = provider ?? Services.BuildServiceProvider();
        
        // ReSharper disable once UseDeconstruction
        InfiniFrameWindowBuilderSnapshot snapshot = CreateSnapshot(provider);
        
        InfiniFrameNativeParameters nativeParameters = snapshot.StartupParameters;
        var events = new InfiniFrameEvents(
            ResolveLogger<InfiniFrameEvents>(actualProvider),
            snapshot.EventsStore
        );
        events.AssignEventCallbacks(ref nativeParameters);
        events.AssignDefaultEventCallbacks();

        var configuration = new InfiniFrameWindowConfiguration();
        var debugging = new InfiniFrameWindowDebugging(
            ResolveLogger<InfiniFrameWindowDebugging>(provider)
        );

        var window = new InfiniFrameWindow(
            logger: ResolveLogger<InfiniFrameWindow>(actualProvider),
            configuration: configuration,
            events: events,
            debugging: debugging,
            serviceProvider:actualProvider
        );
        window.AssignFeatures(actualProvider.GetRequiredService<InfiniFrameWindowFeaturesFactory>().Create(window, this));
        
        InfiniFrameUriSecurityPolicyRegistry.BindToWindow(window, snapshot.UriSecurityPolicy);
        
        events.AssignToWindow(window);
        debugging.AssignToWindow(window);
        window.Features.Lifecycle.Initialize();
        
        return window;

    }

    internal InfiniFrameNativeParameters CollectNativeParameters() {
        var parameters = new InfiniFrameNativeParameters();
        Configuration.ApplyToNativeParameters(ref parameters);
        return parameters;
    }

    internal static ILogger<T> ResolveLogger<T>(IServiceProvider? provider) {
        if (provider is null) return NullLogger<T>.Instance;

        return provider.GetService<ILogger<T>>()
            ?? provider.GetService<ILoggerFactory>()?.CreateLogger<T>()
            ?? NullLogger<T>.Instance;
    }

    internal InfiniFrameWindowBuilderSnapshot CreateSnapshot(IServiceProvider? provider = null) 
        => new(
            CollectNativeParameters(),
            EventsStore.DeepCopy(),
            StaticAssets?.DeepCopy(),
            InfiniFrameUriSecurityPolicyRegistry.GetForBuilder(this)
        );
}
