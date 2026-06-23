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
        if (OperatingSystem.IsWindows()) {
            return WebView2WindowManager.Build(this, actualProvider);
        }

        return BuildCore(actualProvider, null);
    }

    internal IInfiniFrameWindow BuildCore(
        IServiceProvider actualProvider,
        WebView2WindowBuildPlan? webView2BuildPlan
    ) {
        var featureFactory = actualProvider.GetRequiredService<InfiniFrameWindowFeaturesFactory>();
        var validator = actualProvider.GetRequiredService<IValidator<InfiniFrameNativeParameters>>();

        var window = actualProvider.GetRequiredService<InfiniFrameWindow>();

        InfiniFrameNativeParameters nativeParameters = CollectNativeParameters(window.Id);

        window.AssignFeatures(featureFactory.Create(window, this));

        window.Events.PopulateFromBuilderEventStore(EventsStore);
        window.Events.AssignToNativeParameters(ref nativeParameters);
        webView2BuildPlan?.Apply(
            window,
            this,
            ref nativeParameters,
            actualProvider.GetRequiredService<Microsoft.Extensions.Logging.ILoggerFactory>()
                .CreateLogger("InfiniFrame.WebView2WindowManager")
        );
        bool shouldRegisterAutoProfile = webView2BuildPlan?.ShouldRegisterAutoProfile(this)
            ?? Features.Browser is InfiniFrameWindowBuilderFeatureBrowser {
                TemporaryFilesPathExplicitlyAssigned: false
            };
        if (shouldRegisterAutoProfile) {
            BrowserProfileUtility.RegisterAutoProfilePath(window.Id, nativeParameters.TemporaryFilesPath);
        }
        window.Events.AssignDefaultEventCallbacks();
        window.Events.AssignToWindow(window);

        window.Configuration.ParentWindow = Configuration.ParentWindow;
        window.Configuration.AssignNativeParameters(nativeParameters);

        InfiniFrameUriSecurityPolicyRegistry.BindToWindow(
            window,
            InfiniFrameUriSecurityPolicyRegistry.GetForBuilder(this)
        );
        
        validator.ValidateAndThrow(nativeParameters);

        try {
            window.Features.Lifecycle.Initialize();
        }
        catch {
            webView2BuildPlan?.Release();
            WebView2WindowManager.ReleaseWindow(window);
            throw;
        }
        finally {
            webView2BuildPlan?.Release();
        }

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
