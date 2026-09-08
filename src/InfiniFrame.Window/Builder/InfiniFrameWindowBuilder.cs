// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using FluentValidation;
using InfiniFrame.NativeBridge.Parameters.Window;
using InfiniFrame.Security;
using InfiniFrame.Window.Events;
using InfiniFrame.Window.Features.InstanceArbitration;
using Microsoft.Extensions.DependencyInjection;

namespace InfiniFrame.Window.Builder;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Default implementation of <see cref="IInfiniFrameWindowBuilder"/> that collects configuration,
///     features, and event handlers to construct an <see cref="IInfiniFrameWindow"/>.
/// </summary>
public class InfiniFrameWindowBuilder : IInfiniFrameWindowBuilder {
    internal IntPtr ApplicationHandle { get; private set; }

    private IServiceCollection Services { get; }
    /// <inheritdoc cref="IInfiniFrameWindowBuilder.Configuration" />
    public IInfiniFrameWindowBuilderConfiguration Configuration { get; } = new InfiniFrameWindowBuilderConfiguration();
    /// <inheritdoc cref="IInfiniFrameWindowBuilder.Features" />
    public IInfiniFrameWindowBuilderFeatures Features { get; } = new InfiniFrameWindowBuilderFeatures();
    /// <inheritdoc cref="IInfiniFrameWindowBuilder.Debugging" />
    public IDebuggingInfiniFrameWindowBuilderFeature Debugging => Features.Debugging;
    /// <inheritdoc cref="IHasInfiniFrameEventsStore.EventsStore" />
    public IInfiniFrameEventsStore EventsStore { get; }

    /// <inheritdoc cref="IInfiniFrameWindowBuilder.StaticAssets" />
    public IInfiniFrameStaticAssets? StaticAssets { get; set; }

    public InfiniFrameWindowBuilder(IServiceCollection? collection = null, InfiniFrameEventsStore? events = null) {
        EventsStore = events ?? new InfiniFrameEventsStore();
        Services = (collection ?? new ServiceCollection())
            .AddLogging()
            .AddInfiniFrame();
    }

    internal void SetApplicationHandle(IntPtr handle) => ApplicationHandle = handle;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="IInfiniFrameWindowBuilder.Build" />
    public IInfiniFrameWindow Build(IServiceProvider? provider = null) {
        bool ownsServiceProvider = provider is null;
        IServiceProvider actualProvider = provider ?? Services.BuildServiceProvider();
        var featureFactory = actualProvider.GetRequiredService<InfiniFrameWindowFeaturesFactory>();
        var validator = actualProvider.GetRequiredService<IValidator<InfiniFrameNativeWindowParameters>>();

        InfiniFrameNativeWindowParameters nativeParameters = CollectNativeParameters();
        validator.ValidateAndThrow(nativeParameters);

        // Instance arbitration check
        IInstanceArbitrationInfiniFrameWindowBuilderFeature arbitration = Features.InstanceArbitration;
        if (arbitration.Mode != InstanceArbitrationMode.Disabled
            && !InstanceArbitration.TryAcquirePrimaryInstance(arbitration.MutexName)) {
            throw new InstanceAlreadyRunningException();
        }

        var window = actualProvider.GetRequiredService<InfiniFrameWindow>();
        window.SetOwnsServiceProvider(ownsServiceProvider);

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

        window.Features.Lifecycle.Initialize();

        return window;

    }

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Creates a new <see cref="InfiniFrameWindowBuilder"/> with optional DI and event store overrides.
    /// </summary>
    /// <param name="collection">Optional service collection. If <c>null</c>, a default collection with logging and InfiniFrame core services is created.</param>
    /// <param name="events">Optional pre-configured event store. If <c>null</c>, a new empty store is created.</param>
    /// <returns>A configured <see cref="InfiniFrameWindowBuilder"/> ready for feature configuration.</returns>
    internal static InfiniFrameWindowBuilder Create(IServiceCollection? collection = null, InfiniFrameEventsStore? events = null)
        => new(collection, events);

    internal InfiniFrameNativeWindowParameters CollectNativeParameters() {
        var parameters = new InfiniFrameNativeWindowParameters();

        Configuration.ApplyToNativeParameters(ref parameters);
        Features.ApplyToNativeParameters(ref parameters);
        parameters.ApplicationInstance = ApplicationHandle;

        return parameters;
    }
}
