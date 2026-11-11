// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Hosting;
using InfiniFrame.Window;
using Microsoft.Extensions.DependencyInjection;

namespace InfiniFrame.Application;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>Composes an InfiniFrame application before it is built.</summary>
public sealed class InfiniFrameApplicationBuilder {
    private readonly List<Action<InfiniFrameApplication>> _integrations = [];
    private readonly List<(string? Id, Action<IInfiniFrameWindowBuilder> Configure)> _windows = [];
    private string? _webView2RuntimePath;
    private string? _notificationRegistrationId;
    private string? _appUserModelId;
    private string? _defaultNotificationIcon;
    private IInfiniFrameHostProvider? _hostProvider;

    public IReadOnlyCollection<(string? Id, Action<IInfiniFrameWindowBuilder> Configure)> WindowRegistrations 
        => _windows.AsReadOnly();

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    internal InfiniFrameApplicationBuilder(string[]? args) {
        Args = args ?? [];
        Services = new ServiceCollection().AddLogging().AddInfiniFrame();
    }

    internal string[] Args { get; }
    public IServiceCollection Services { get; }

    public InfiniFrameApplicationBuilder WithWindow(Action<IInfiniFrameWindowBuilder> configure) {
        ArgumentNullException.ThrowIfNull(configure);
        _windows.Add((null, configure));
        return this;
    }

    public InfiniFrameApplicationBuilder WithWindow(string id, Action<IInfiniFrameWindowBuilder> configure) {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(configure);
        if (_windows.Any(window => string.Equals(window.Id, id, StringComparison.Ordinal)))
            throw new ArgumentException($"A window with id '{id}' is already registered.", nameof(id));
        _windows.Add((id, configure));
        return this;
    }

    public InfiniFrameApplicationBuilder WithWebView2RuntimePath(string path) {
        ThrowIfHostProviderSelected(nameof(WithWebView2RuntimePath));
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        _webView2RuntimePath = Path.GetFullPath(path);
        return this;
    }

    public InfiniFrameApplicationBuilder WithNotificationRegistrationId(string id) {
        ThrowIfHostProviderSelected(nameof(WithNotificationRegistrationId));
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        _notificationRegistrationId = id;
        return this;
    }

    public InfiniFrameApplicationBuilder WithAppUserModelId(string id) {
        ThrowIfHostProviderSelected(nameof(WithAppUserModelId));
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        _appUserModelId = id;
        return this;
    }

    public InfiniFrameApplicationBuilder WithDefaultNotificationIcon(string path) {
        ThrowIfHostProviderSelected(nameof(WithDefaultNotificationIcon));
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        _defaultNotificationIcon = Path.GetFullPath(path);
        return this;
    }

    /// <summary>The explicitly selected external host provider, if any.</summary>
    internal IInfiniFrameHostProvider? HostProvider => _hostProvider;

    internal void SetHostProvider(IInfiniFrameHostProvider provider) {
        ArgumentNullException.ThrowIfNull(provider);
        if (_hostProvider is not null)
            throw new InvalidOperationException("A host provider is already selected for this application. Select exactly one before Build().");
        if (_webView2RuntimePath is not null || _notificationRegistrationId is not null || _appUserModelId is not null || _defaultNotificationIcon is not null)
            throw new InvalidOperationException(
                "An external host provider cannot be combined with default-engine settings " +
                "(WebView2 runtime path, notification registration id, AppUserModelId or default notification icon).");
        _hostProvider = provider;
    }

    private void ThrowIfHostProviderSelected(string setting) {
        if (_hostProvider is not null)
            throw new InvalidOperationException($"{setting} configures the default native backend and cannot be combined with an external host provider.");
    }

    internal void AddIntegration(Action<InfiniFrameApplication> integration) {
        ArgumentNullException.ThrowIfNull(integration);
        _integrations.Add(integration);
    }

    public InfiniFrameApplication Build() {
        InfiniFrameApplication? application = null;
        IServiceProvider? serviceProvider = null;
        bool serviceProviderAttached = false;
        try {
            application = InfiniFrameApplication.Initialize(new ApplicationConfiguration(
                _webView2RuntimePath,
                _notificationRegistrationId,
                _appUserModelId,
                _defaultNotificationIcon
            ), _hostProvider);

            Services.AddSingleton<IInfiniFrameApplication>(application);
            serviceProvider = Services.BuildServiceProvider();

            foreach ((string? id, Action<IInfiniFrameWindowBuilder> configure) in _windows) {
                if (id is null) application.RegisterWindow(configure);
                else application.RegisterWindow(id, configure);
            }
            application.AttachServiceProvider(serviceProvider);
            serviceProviderAttached = true;
            foreach (Action<InfiniFrameApplication> integration in _integrations)
                integration(application);
            return application;
        }
        catch {
            application?.Dispose();
            if (!serviceProviderAttached && serviceProvider is IDisposable disposableServiceProvider)
                disposableServiceProvider.Dispose();
            throw;
        }
    }
}
