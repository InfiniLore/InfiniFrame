// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Reflection;
using InfiniFrame.BlazorWebView.FileProviders;
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

public sealed class InfiniFrameBlazorWebViewConfiguration {
    private readonly IServiceCollection _services;
    private readonly InfiniFrameWindowBuilder _windowBuilder = new();

    internal InfiniFrameBlazorWebViewConfiguration(IServiceCollection services) {
        _services = services;
        ConfigureServices();
    }

    public IInfiniFrameRootComponentList RootComponents { get; } = new InfiniFrameRootComponentList();

    public InfiniFrameBlazorWebViewConfiguration Configure(Action<InfiniFrameBlazorAppConfiguration> configure) {
        ArgumentNullException.ThrowIfNull(configure);
        _services.Configure(configure);
        return this;
    }

    public InfiniFrameBlazorWebViewConfiguration ConfigureWindow(Action<IInfiniFrameWindowBuilder> configure) {
        ArgumentNullException.ThrowIfNull(configure);
        configure(_windowBuilder);
        return this;
    }

    internal void AddSingleFileProvider(IFileProvider provider) {
        ArgumentNullException.ThrowIfNull(provider);
        _services.AddSingleton(provider);
    }

    internal void Apply(InfiniFrameApplication application, string windowId) {
        IServiceProvider services = application.RootServiceProvider;
        var manager = services.GetRequiredService<IInfiniFrameWebViewManager>();
        InfiniFrameBlazorAppConfiguration appConfig = services.GetService<IOptions<InfiniFrameBlazorAppConfiguration>>()?.Value
            ?? new InfiniFrameBlazorAppConfiguration();

        InfiniFrameUriSecurityPolicyRegistry.ConfigureForBuilder(
            _windowBuilder,
            configure: policyBuilder => policyBuilder.AddTrustedOrigin(appConfig.AppBaseUri));

        _windowBuilder.StaticAssets = services.GetRequiredService<IInfiniFrameStaticAssets>().DeepCopy();
        if (!_windowBuilder.EventsStore.CustomScheme.ContainsKey(InfiniFrameWebViewManager.BlazorAppScheme))
            _windowBuilder.RegisterCustomSchemeHandler(InfiniFrameWebViewManager.BlazorAppScheme, manager.HandleWebRequest);
        _windowBuilder.RegisterWebMessageReceivedHandler(manager.HandleWebMessage);
        _windowBuilder.SetStartPageUrl(BuildStartupUrl(appConfig));

        IInfiniFrameJsComponentConfiguration? jsConfiguration =
            services.GetService<IInfiniFrameJsComponentConfiguration>();
        if (jsConfiguration is not null) {
            application.WindowCreated += _ => {
                foreach ((Type componentType, string selector) in RootComponents)
                    jsConfiguration.Add(componentType, selector);
            };
        }

        IDisposable? exceptionRegistration = TryRegisterUnhandledExceptionHandler(services);
        application.RegisterWindowBuilder(windowId, _windowBuilder);
        if (exceptionRegistration is not null)
            application.RegisterShutdownAction(() => {
                exceptionRegistration.Dispose();
                return Task.CompletedTask;
            });
    }

    private void ConfigureServices() {
        IFileProvider fileProvider = ConfigureFileProvider(null);
        _services.AddOptions<InfiniFrameBlazorAppConfiguration>();
        _services
            .AddInfiniFrame()
            .AddScoped(static sp => {
                var handler = sp.GetRequiredService<InfiniFrameHttpHandler>();
                return new HttpClient(handler) { BaseAddress = new Uri(InfiniFrameWebViewManager.AppBaseUri) };
            })
            .AddSingleton<IInfiniFrameWebViewManager, InfiniFrameWebViewManager>()
            .AddSingleton<IInfiniFrameJsComponentConfiguration, InfiniFrameJsComponentConfiguration>()
            .AddSingleton<Dispatcher, InfiniFrameDispatcher>()
            .AddSingleton<InfiniFrameHttpHandler>()
            .AddSingleton<InfiniFrameSynchronizationContext>()
            .AddBlazorWebView()
            .AddSingleton(fileProvider)
            .AddSingleton<IInfiniFrameStaticAssets>(static provider => {
                InfiniFrameBlazorAppConfiguration config = provider.GetService<IOptions<InfiniFrameBlazorAppConfiguration>>()?.Value
                    ?? new InfiniFrameBlazorAppConfiguration();
                return new InfiniFrameStaticAssets {
                    FileProvider = provider.GetRequiredService<IFileProvider>(),
                    BaseUri = config.AppBaseUri.ToString(),
                    DefaultDocument = NormalizeHostPage(config.HostPage)
                };
            })
            .AddSingleton(RootComponents)
            .AddSingleton(RootComponents.JSComponents);

        _services.TryAddSingleton<IInfiniFrameUnhandledExceptionSource, AppDomainUnhandledExceptionSource>();
        _services.AddInfiniFrameJs();
        _windowBuilder.RegisterGetWebMessageHandler();
    }

    private static IFileProvider ConfigureFileProvider(IFileProvider? fileProvider) {
        if (fileProvider is not null) return fileProvider;
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var providers = new List<IFileProvider>();
        IFileProvider? staticWebAssets = StaticWebAssetsRuntimeFileProvider.TryCreate(baseDirectory, Assembly.GetEntryAssembly());
        if (staticWebAssets is not null) providers.Add(staticWebAssets);
        string wwwroot = Path.Join(baseDirectory, "wwwroot");
        PhysicalFileProvider? physical = Directory.Exists(wwwroot) ? new PhysicalFileProvider(wwwroot) : null;
        if (physical is not null) providers.Add(physical);
        return providers.Count switch {
            0 => new NullFileProvider(),
            1 => providers[0],
            _ => new DisposableCompositeFileProvider(providers, physical!)
        };
    }

    private static string BuildStartupUrl(InfiniFrameBlazorAppConfiguration configuration) {
        Uri appBaseUri = configuration.AppBaseUri;
        string hostPage = NormalizeHostPage(configuration.HostPage);
        return string.Equals(hostPage, "index.html", StringComparison.OrdinalIgnoreCase)
            ? appBaseUri.ToString()
            : new Uri(appBaseUri, hostPage).ToString();
    }

    private static string NormalizeHostPage(string? hostPage) =>
        !string.IsNullOrWhiteSpace(hostPage) ? hostPage.TrimStart('/') : "index.html";

    private static IDisposable? TryRegisterUnhandledExceptionHandler(IServiceProvider services) {
        bool enabled = services.GetService<IOptions<InfiniFrameBlazorAppConfiguration>>()?.Value
            .EnableGlobalUnhandledExceptionHandler ?? true;
        if (!enabled) return null;
        var source = services.GetRequiredService<IInfiniFrameUnhandledExceptionSource>();
        return source.Register((_, error) => {
            try {
                var window = services.GetService<IInfiniFrameWindow>();
                window?.Invoke(() => window.ShowMessage("Fatal exception", error.ExceptionObject.ToString()));
            }
            catch (ObjectDisposedException) { }
            catch (InvalidOperationException) { }
        });
    }
}

public static class InfiniFrameApplicationBlazorWebViewExtensions {
    public static InfiniFrameApplicationBuilder WithBlazorWebView(
        this InfiniFrameApplicationBuilder builder,
        Action<InfiniFrameBlazorWebViewConfiguration> configure
    ) => builder.UseBlazorWebView(configure);

    public static InfiniFrameApplicationBuilder WithBlazorWebView(
        this InfiniFrameApplicationBuilder builder,
        string windowId,
        Action<InfiniFrameBlazorWebViewConfiguration> configure
    ) => builder.UseBlazorWebView(windowId, configure);

    public static InfiniFrameApplicationBuilder UseBlazorWebView(
        this InfiniFrameApplicationBuilder builder,
        Action<InfiniFrameBlazorWebViewConfiguration> configure
    ) => UseBlazorWebView(builder, "blazor", configure);

    public static InfiniFrameApplicationBuilder UseBlazorWebView(
        this InfiniFrameApplicationBuilder builder,
        string windowId,
        Action<InfiniFrameBlazorWebViewConfiguration> configure
    ) {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(windowId);
        ArgumentNullException.ThrowIfNull(configure);

        Action<IInfiniFrameWindowBuilder>? configureWindow = builder.TakeWindowConfiguration(windowId);
        var configuration = new InfiniFrameBlazorWebViewConfiguration(builder.Services);
        if (configureWindow is not null) configuration.ConfigureWindow(configureWindow);
        configure(configuration);
        builder.Services.AddSingleton<IInfiniFrameWindow>(provider =>
            provider.GetRequiredService<IInfiniFrameApplication>().GetWindow(windowId));
        builder.AddIntegration(application => {
            configuration.Apply(application, windowId);
        });
        return builder;
    }
}
