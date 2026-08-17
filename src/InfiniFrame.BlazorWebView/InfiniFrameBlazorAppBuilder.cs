// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.BlazorWebView.FileProviders.Static;
using InfiniFrame.Security;
using InfiniFrame.StaticAssets;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameBlazorAppBuilder : IInfiniFrameBlazorAppBuilder {
    /// <inheritdoc cref="IInfiniFrameBlazorAppBuilder.RootComponents" />
    public IInfiniFrameRootComponentList RootComponents { get; } = new InfiniFrameRootComponentList();
    /// <inheritdoc cref="IInfiniFrameBlazorAppBuilder.Services" />
    public IServiceCollection Services { get; } = new ServiceCollection();
    /// <inheritdoc cref="IInfiniFrameBlazorAppBuilder.WindowBuilder" />
    public IInfiniFrameWindowBuilder WindowBuilder { get; } = InfiniFrameWindowBuilder.Create();

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    private InfiniFrameBlazorAppBuilder() {}

    public static InfiniFrameBlazorAppBuilder CreateDefault(
        string[]? args = null,
        Action<IInfiniFrameWindowBuilder>? windowBuilder = null
    )
        => CreateDefault(null, args, windowBuilder);

    public static InfiniFrameBlazorAppBuilder CreateDefault(IFileProvider? fileProvider, string[]? args = null, Action<IInfiniFrameWindowBuilder>? windowBuilder = null) {
        // We don't use the args for anything right now, but we want to accept them
        // here so that it shows up this way in the project templates.
        var appBuilder = new InfiniFrameBlazorAppBuilder();
        IFileProvider resolvedFileProvider = ConfigureFileProvider(fileProvider);

        appBuilder.Services.AddOptions<InfiniFrameBlazorAppConfiguration>();

        appBuilder.Services
            .AddInfiniFrame()
            .AddScoped(static sp => {
                var handler = sp.GetRequiredService<InfiniFrameHttpHandler>();
                return new HttpClient(handler) { BaseAddress = new Uri(InfiniFrameWebViewManager.AppBaseUri) };
            })
            .AddSingleton<IInfiniFrameWebViewManager, InfiniFrameWebViewManager>()
            .AddSingleton<IInfiniFrameJsComponentConfiguration, InfiniFrameJsComponentConfiguration>()
            .AddSingleton<Dispatcher, InfiniFrameDispatcher>()
            .AddSingleton<InfiniFrameBlazorApp>()
            .AddSingleton<InfiniFrameHttpHandler>()
            .AddSingleton<InfiniFrameSynchronizationContext>()
            .AddSingleton<IInfiniFrameWindow>(static provider => provider.GetRequiredService<IInfiniFrameWindowBuilder>().Build(provider))
            .AddBlazorWebView()
            .AddSingleton(resolvedFileProvider)
            .AddSingleton<IInfiniFrameStaticAssets>(static provider => {
                InfiniFrameBlazorAppConfiguration config = provider.GetService<IOptions<InfiniFrameBlazorAppConfiguration>>()?.Value
                    ?? new InfiniFrameBlazorAppConfiguration();

                return new InfiniFrameStaticAssets {
                    FileProvider = provider.GetRequiredService<IFileProvider>(),
                    BaseUri = config.AppBaseUri.ToString(),
                    DefaultDocument = NormalizeHostPage(config.HostPage)
                };
            })
            .AddSingleton(appBuilder.WindowBuilder)
            .AddSingleton(appBuilder.RootComponents)
            .AddSingleton(appBuilder.RootComponents.JSComponents);

        appBuilder.Services.TryAddSingleton<IInfiniFrameUnhandledExceptionSource, AppDomainUnhandledExceptionSource>();

        appBuilder.Services.AddInfiniFrameJs();
        appBuilder.WindowBuilder.RegisterGetWebMessageHandler();

        windowBuilder?.Invoke(appBuilder.WindowBuilder);

        return appBuilder;
    }

    /// <summary>
    ///     Configures the file provider to be used by the application.
    ///     If a custom <see cref="IFileProvider" /> is provided, that instance will be used.
    ///     Otherwise, a default provider will be configured based on the application's "wwwroot" directory.
    /// </summary>
    /// <param name="fileProvider">
    ///     An optional <see cref="IFileProvider" /> instance.
    /// </param>
    /// <returns>
    ///     An instance of <see cref="IFileProvider" /> that represents either the specified file provider
    ///     or the default provider if none is supplied.
    /// </returns>
    private static IFileProvider ConfigureFileProvider(IFileProvider? fileProvider) {
        if (fileProvider is not null) return fileProvider;

        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var providers = new List<IFileProvider>();

        IFileProvider? staticWebAssetsProvider = StaticWebAssetsRuntimeFileProvider.TryCreate(baseDirectory);
        if (staticWebAssetsProvider is not null) providers.Add(staticWebAssetsProvider);

        string defaultWwwrootPath = Path.Join(baseDirectory, "wwwroot");
        bool hasPhysicalWwwroot = Directory.Exists(defaultWwwrootPath);
        PhysicalFileProvider? physicalWwwrootProvider = hasPhysicalWwwroot
            ? new PhysicalFileProvider(defaultWwwrootPath)
            : null;
        if (physicalWwwrootProvider is not null) providers.Add(physicalWwwrootProvider);

        return providers.Count switch {
            0 => new NullFileProvider(),
            1 => providers[0],
            _ => new DisposableCompositeFileProvider(providers, physicalWwwrootProvider!)
        };

    }

    public InfiniFrameBlazorAppBuilder WithInfiniFrameWindowBuilder(Action<IInfiniFrameWindowBuilder> windowBuilder) {
        windowBuilder.Invoke(WindowBuilder);
        return this;
    }

    /// <summary>
    ///     Builds a new <see cref="InfiniFrameBlazorApp" /> using a service provider created from <see cref="Services" />.
    /// </summary>
    /// <returns>A newly created <see cref="InfiniFrameBlazorApp" />.</returns>
    public InfiniFrameBlazorApp Build()
        => Build(Services.BuildServiceProvider());

    /// <summary>
    ///     Builds a new <see cref="InfiniFrameBlazorApp" /> using an externally supplied <see cref="IServiceProvider" />.
    /// </summary>
    /// <param name="serviceProvider">
    ///     The pre-built service provider to use for resolving all application services.
    ///     Ownership is transferred to the returned app instance; when that app is disposed, this provider is disposed if it
    ///     implements
    ///     <see cref="IAsyncDisposable" /> or <see cref="IDisposable" />. Do not dispose the same provider separately.
    /// </param>
    /// <returns>A newly created <see cref="InfiniFrameBlazorApp" />.</returns>
    /// <remarks>
    ///     Calling this method more than once on the same builder instance is not supported. Each call mutates builder state
    ///     (for example, by registering additional scheme handlers), which can lead to duplicate registrations.
    ///     Create a new builder for each app instance.
    /// </remarks>
    public InfiniFrameBlazorApp Build(IServiceProvider serviceProvider) {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        var manager = serviceProvider.GetRequiredService<IInfiniFrameWebViewManager>();
        InfiniFrameBlazorAppConfiguration appConfig = serviceProvider.GetService<IOptions<InfiniFrameBlazorAppConfiguration>>()?.Value
            ?? new InfiniFrameBlazorAppConfiguration();
        InfiniFrameUriSecurityPolicyRegistry.ConfigureForBuilder(
            WindowBuilder,
            configure: policyBuilder => policyBuilder.AddTrustedOrigin(appConfig.AppBaseUri));
        string startupUrl = BuildStartupUrl(appConfig);
        var staticAssets = serviceProvider.GetRequiredService<IInfiniFrameStaticAssets>();

        WindowBuilder.StaticAssets = staticAssets.DeepCopy();

        if (!WindowBuilder.EventsStore.CustomScheme.ContainsKey(InfiniFrameWebViewManager.BlazorAppScheme)) {
            WindowBuilder.RegisterCustomSchemeHandler(InfiniFrameWebViewManager.BlazorAppScheme, manager.HandleWebRequest);
        }

        WindowBuilder.SetStartPageUrl(startupUrl);

        IDisposable? unhandledExceptionRegistration = TryRegisterUnhandledExceptionHandler(serviceProvider);

        return new InfiniFrameBlazorApp(
            serviceProvider,
            serviceProvider.GetRequiredService<IInfiniFrameRootComponentList>(),
            serviceProvider.GetService<IInfiniFrameJsComponentConfiguration>(),
            unhandledExceptionRegistration
        );
    }

    private static string BuildStartupUrl(InfiniFrameBlazorAppConfiguration configuration) {
        Uri appBaseUri = configuration.AppBaseUri;
        string hostPage = NormalizeHostPage(configuration.HostPage);

        return string.Equals(hostPage, "index.html", StringComparison.OrdinalIgnoreCase)
            ? appBaseUri.ToString()
            : new Uri(appBaseUri, hostPage).ToString();
    }

    private static string NormalizeHostPage(string? hostPage)
        => !string.IsNullOrWhiteSpace(hostPage)
            ? hostPage.TrimStart('/')
            : "index.html";

    private static IDisposable? TryRegisterUnhandledExceptionHandler(IServiceProvider serviceProvider) {
        bool enableGlobalUnhandledExceptionHandler = serviceProvider.GetService<IOptions<InfiniFrameBlazorAppConfiguration>>()?
            .Value.EnableGlobalUnhandledExceptionHandler ?? true;

        if (!enableGlobalUnhandledExceptionHandler) return null;

        var exceptionSource = serviceProvider.GetRequiredService<IInfiniFrameUnhandledExceptionSource>();

        return exceptionSource.Register((_, error) => {
            try {
                var window = serviceProvider.GetService<IInfiniFrameWindow>();

                // Only interact if safe
                window?.Invoke(() => {
                    window.ShowMessage(
                        "Fatal exception",
                        error.ExceptionObject.ToString()
                    );
                });
            }
            catch (ObjectDisposedException) {
                // Window already closed; nothing to report.
            }
            catch (InvalidOperationException) {
                // Service not available; nothing to report.
            }
        });
    }
}
