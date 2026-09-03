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
/// <summary>
///     Default implementation of <see cref="IInfiniFrameWindowBuilder"/> that collects configuration,
///     features, and event handlers to construct an <see cref="IInfiniFrameWindow"/>.
/// </summary>
public class InfiniFrameWindowBuilder : IInfiniFrameWindowBuilder {

    internal IServiceCollection Services { get; init; } = new ServiceCollection().AddLogging().AddInfiniFrame().AddTransient<IInfiniFrameWindow, InfiniFrameWindow>();
    /// <inheritdoc cref="IInfiniFrameWindowBuilder.Configuration" />
    public IInfiniFrameWindowBuilderConfiguration Configuration { get; } = new InfiniFrameWindowBuilderConfiguration();
    /// <inheritdoc cref="IInfiniFrameWindowBuilder.Features" />
    public IInfiniFrameWindowBuilderFeatures Features { get; } = new InfiniFrameWindowBuilderFeatures();
    /// <inheritdoc cref="IInfiniFrameWindowBuilder.Debugging" />
    public IDebuggingInfiniFrameWindowBuilderFeature Debugging => Features.Debugging;
    /// <inheritdoc cref="IHasInfiniFrameEventsStore.EventsStore" />
    public IInfiniFrameEventsStore EventsStore { get; set; } = new InfiniFrameEventsStore();

    /// <inheritdoc cref="IInfiniFrameWindowBuilder.StaticAssets" />
    public IInfiniFrameStaticAssets? StaticAssets { get; set; }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="IInfiniFrameWindowBuilder.Build" />
    public IInfiniFrameWindow Build(IServiceProvider? provider = null) {
        bool ownsServiceProvider = provider is null;
        IServiceProvider actualProvider = provider ?? Services.BuildServiceProvider();
        var featureFactory = actualProvider.GetRequiredService<InfiniFrameWindowFeaturesFactory>();
        var validator = actualProvider.GetRequiredService<IValidator<InfiniFrameNativeParameters>>();

        // Ensure the application is initialized before creating any windows.
        var application = actualProvider.GetRequiredService<IInfiniFrameApplication>();
        if (application.ApplicationHandle == IntPtr.Zero && !application.IsShutdownRequested) {
            var appConfig = new ApplicationConfiguration();
            if (OperatingSystem.IsWindows()) {
                appConfig.HInstance = System.Diagnostics.Process.GetCurrentProcess().MainModule?.BaseAddress ?? IntPtr.Zero;
            }
            application.Initialize(appConfig);
        }

        InfiniFrameNativeParameters nativeParameters = CollectNativeParameters();

        // Instance arbitration check
        IInstanceArbitrationInfiniFrameWindowBuilderFeature arbitration = Features.InstanceArbitration;
        if (arbitration.Mode != InstanceArbitrationMode.Disabled
            && !InstanceArbitration.TryAcquirePrimaryInstance(arbitration.MutexName)) {
            throw new InstanceAlreadyRunningException();
        }

        // Create the window directly using ActivatorUtilities instead of resolving
        // from the provider. This breaks the circular dependency where the lazy
        // IInfiniFrameWindow factory (from InfiniFrameBlazorAppBuilder) calls
        // Build(provider), which resolves IInfiniFrameWindow from the same provider.
        var window = (InfiniFrameWindow)ActivatorUtilities.CreateInstance(
            actualProvider,
            typeof(InfiniFrameWindow));
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
