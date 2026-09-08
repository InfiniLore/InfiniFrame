// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Reflection;
using InfiniFrame.Application;
using InfiniFrame.BlazorWebView.FileProviders;
using InfiniFrame.Security;
using InfiniFrame.StaticAssets;
using InfiniFrame.Window;
using InfiniFrame.Window.Features.WebMessaging.Handlers;
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
    private readonly List<Action<IInfiniFrameWindowBuilder>> _windowConfigurations = [];

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
        _windowConfigurations.Add(configure);
        return this;
    }

    internal void AddSingleFileProvider(IFileProvider provider) {
        ArgumentNullException.ThrowIfNull(provider);
        _services.AddSingleton(provider);
    }

    internal void Apply(InfiniFrameApplication application, IReadOnlyList<string> windowIds) {
        IServiceProvider services = application.RootServiceProvider;
        var manager = services.GetRequiredService<IInfiniFrameWebViewManager>();
        InfiniFrameBlazorAppConfiguration appConfig = services.GetService<IOptions<InfiniFrameBlazorAppConfiguration>>()?.Value
            ?? new InfiniFrameBlazorAppConfiguration();

        IInfiniFrameJsComponentConfiguration? jsConfiguration =
            services.GetService<IInfiniFrameJsComponentConfiguration>();
        if (jsConfiguration is not null) {
            application.WindowCreated += _ => {
                foreach ((Type componentType, string selector) in RootComponents)
                    jsConfiguration.Add(componentType, selector);
            };
        }

        IDisposable? exceptionRegistration = TryRegisterUnhandledExceptionHandler(services);
        application.ApplyWindowIntegration(windowIds, "BlazorWebView", windowBuilder => {
            foreach (Action<IInfiniFrameWindowBuilder> configure in _windowConfigurations)
                configure(windowBuilder);
            InfiniFrameUriSecurityPolicyRegistry.ConfigureForBuilder(
                windowBuilder,
                policyBuilder => policyBuilder.AddTrustedOrigin(appConfig.AppBaseUri));
            windowBuilder.StaticAssets = services.GetRequiredService<IInfiniFrameStaticAssets>().DeepCopy();
            if (!windowBuilder.EventsStore.CustomScheme.ContainsKey(InfiniFrameWebViewManager.BlazorAppScheme))
                windowBuilder.RegisterCustomSchemeHandler(InfiniFrameWebViewManager.BlazorAppScheme, manager.HandleWebRequest);
            windowBuilder.RegisterWebMessageReceivedHandler(manager.HandleWebMessage);
            windowBuilder.RegisterGetWebMessageHandler();
            windowBuilder.SetStartPageUrl(BuildStartupUrl(appConfig));
        });
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

    public static InfiniFrameApplicationBuilder WithBlazorWebView(
        this InfiniFrameApplicationBuilder builder,
        IEnumerable<string> windowIds,
        Action<InfiniFrameBlazorWebViewConfiguration> configure
    ) => builder.UseBlazorWebView(windowIds, configure);

    public static InfiniFrameApplicationBuilder UseBlazorWebView(
        this InfiniFrameApplicationBuilder builder,
        Action<InfiniFrameBlazorWebViewConfiguration> configure
    ) => builder.UseBlazorWebView(configure, []);

    public static InfiniFrameApplicationBuilder UseBlazorWebView(
        this InfiniFrameApplicationBuilder builder,
        string windowId,
        Action<InfiniFrameBlazorWebViewConfiguration> configure
    ) => builder.UseBlazorWebView(configure, [windowId]);

    public static InfiniFrameApplicationBuilder UseBlazorWebView(
        this InfiniFrameApplicationBuilder builder,
        IEnumerable<string> windowIds,
        Action<InfiniFrameBlazorWebViewConfiguration> configure
    ) {
        ArgumentNullException.ThrowIfNull(windowIds);
        return builder.UseBlazorWebView(configure, windowIds.ToArray());
    }

    public static InfiniFrameApplicationBuilder UseBlazorWebView(
        this InfiniFrameApplicationBuilder builder,
        Action<InfiniFrameBlazorWebViewConfiguration> configure,
        params string[] windowIds
    ) {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configure);

        var configuration = new InfiniFrameBlazorWebViewConfiguration(builder.Services);
        configure(configuration);
        builder.Services.AddSingleton<IInfiniFrameWindow>(provider =>
            ResolveTargetWindow(provider.GetRequiredService<IInfiniFrameApplication>(), windowIds));
        builder.AddIntegration(application => {
            application.ValidateWindowIntegrationTargets(windowIds, "BlazorWebView");
            configuration.Apply(application, windowIds);
        });
        return builder;
    }

    private static IInfiniFrameWindow ResolveTargetWindow(IInfiniFrameApplication application, IReadOnlyList<string> windowIds) {
        if (windowIds.Count > 0) return application.GetWindow(windowIds[0]);
        if (application.Windows.Count == 1) return application.Windows[0];
        throw new InvalidOperationException("BlazorWebView could not resolve its target window.");
    }
}
