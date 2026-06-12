// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using FluentValidation;
using InfiniFrame.NativeBridge.Parameters;
using InfiniFrame.Security;
using Microsoft.Extensions.DependencyInjection;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowBuilder : IInfiniFrameWindowBuilder {
    public IInfiniFrameWindowBuilderConfiguration Configuration { get; } = new InfiniFrameWindowBuilderConfiguration();
    public IInfiniFrameWindowBuilderFeatures Features { get; } = new InfiniFrameWindowBuilderFeatures();
    public IInfiniFrameWindowBuilderFeatureDebugging Debugging => Features.Debugging;
    public IInfiniFrameEventsStore EventsStore { get; private init; } = new InfiniFrameEventsStore();

    public IInfiniFrameStaticAssets? StaticAssets { get; set; }

    private IServiceCollection Services { get; init; } = new ServiceCollection().AddInfiniFrame();

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    public static InfiniFrameWindowBuilder Create(IServiceCollection? collection = null, InfiniFrameEventsStore? events = null) {
        var builder = new InfiniFrameWindowBuilder {
            EventsStore = events ?? new InfiniFrameEventsStore(),
            Services = (collection ?? new ServiceCollection())
                .AddLogging()
                .AddInfiniFrame()
        };

        return builder;
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public IInfiniFrameWindow Build(IServiceProvider? provider = null) {
        IServiceProvider actualProvider = provider ?? Services.BuildServiceProvider();
        var featureFactory = actualProvider.GetRequiredService<InfiniFrameWindowFeaturesFactory>();
        var validator = actualProvider.GetRequiredService<IValidator<InfiniFrameNativeParameters>>();

        IInfiniFrameEventsStore sourceStore = actualProvider.GetService<WindowBuildSnapshot>()?.EventsStore ?? EventsStore;

        InfiniFrameNativeParameters nativeParameters = CollectNativeParameters();

        var window = actualProvider.GetRequiredService<InfiniFrameWindow>();

        window.AssignFeatures(featureFactory.Create(window, this));

        window.Events.PopulateFromBuilderEventStore(sourceStore);
        window.Events.AssignToNativeParameters(ref nativeParameters);
        window.Events.AssignDefaultEventCallbacks();
        window.Events.AssignToWindow(window);

        window.Configuration.ParentWindow = Configuration.ParentWindow;
        window.Configuration.AssignNativeParameters(nativeParameters);

        InfiniFrameUriSecurityPolicyRegistry.BindToWindow(
            window,
            InfiniFrameUriSecurityPolicyRegistry.GetForBuilder(this)
        );
        
        validator.ValidateAndThrow(nativeParameters);

        window.Features.Lifecycle.Initialize();

        return window;

    }

    internal InfiniFrameNativeParameters CollectNativeParameters() {
        var parameters = new InfiniFrameNativeParameters();
        
        Configuration.ApplyToNativeParameters(ref parameters);
        Features.ApplyToNativeParameters(ref parameters);

        return parameters;
    }
}
