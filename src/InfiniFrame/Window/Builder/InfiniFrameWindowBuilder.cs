// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using FluentValidation;
using InfiniFrame.NativeBridge.Parameters;
using InfiniFrame.Security;
using InfiniFrame.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace InfiniFrame;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowBuilder : IInfiniFrameWindowBuilder {
    /// <inheritdoc cref="IInfiniFrameWindowBuilder.Configuration"/>
    public IInfiniFrameWindowBuilderConfiguration Configuration { get; } = new InfiniFrameWindowBuilderConfiguration();
    /// <inheritdoc cref="IInfiniFrameWindowBuilder.Features"/>
    public IInfiniFrameWindowBuilderFeatures Features { get; } = new InfiniFrameWindowBuilderFeatures();
    /// <inheritdoc cref="IInfiniFrameWindowBuilder.Debugging"/>
    public IInfiniFrameWindowBuilderFeatureDebugging Debugging => Features.Debugging;
    /// <inheritdoc cref="IHasInfiniFrameEventsStore.EventsStore"/>
    public IInfiniFrameEventsStore EventsStore { get; private init; } = new InfiniFrameEventsStore();

    /// <inheritdoc cref="IInfiniFrameWindowBuilder.StaticAssets"/>
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
    /// <inheritdoc cref="IInfiniFrameWindowBuilder.Build"/>
    public IInfiniFrameWindow Build(IServiceProvider? provider = null) {
        IServiceProvider actualProvider = provider ?? Services.BuildServiceProvider();
        var featureFactory = actualProvider.GetRequiredService<InfiniFrameWindowFeaturesFactory>();
        var validator = actualProvider.GetRequiredService<IValidator<InfiniFrameNativeParameters>>();

        var window = actualProvider.GetRequiredService<InfiniFrameWindow>();

        InfiniFrameNativeParameters nativeParameters = CollectNativeParameters(window.Id);
        if (Features.Browser is InfiniFrameWindowBuilderFeatureBrowser { TemporaryFilesPathExplicitlyAssigned: false }) {
            BrowserProfileUtility.RegisterAutoProfilePath(window.Id, nativeParameters.TemporaryFilesPath);
        }

        window.AssignFeatures(featureFactory.Create(window, this));

        window.Events.PopulateFromBuilderEventStore(EventsStore);
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

    internal InfiniFrameNativeParameters CollectNativeParameters()
        => CollectNativeParameters(null);

    internal InfiniFrameNativeParameters CollectNativeParameters(Guid? windowId) {
        var parameters = new InfiniFrameNativeParameters();
        
        Configuration.ApplyToNativeParameters(ref parameters);
        Features.ApplyToNativeParameters(ref parameters);
        if (windowId is { } id && Features.Browser is InfiniFrameWindowBuilderFeatureBrowser browser) {
            parameters.TemporaryFilesPath = browser.ResolveTemporaryFilesPath(id);
        }

        return parameters;
    }
}
