using InfiniFrame.Window;
using Microsoft.Extensions.DependencyInjection;

namespace InfiniFrame;

/// <summary>Composes an InfiniFrame application before it is built.</summary>
public sealed class InfiniFrameApplicationBuilder {
    private readonly List<Action<InfiniFrameApplication>> _integrations = [];
    private readonly List<(string? Id, Action<IInfiniFrameWindowBuilder> Configure)> _windows = [];
    private string? _webView2RuntimePath;
    private string? _notificationRegistrationId;
    private string? _appUserModelId;
    private string? _defaultNotificationIcon;

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
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        _webView2RuntimePath = Path.GetFullPath(path);
        return this;
    }

    public InfiniFrameApplicationBuilder WithNotificationRegistrationId(string id) {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        _notificationRegistrationId = id;
        return this;
    }

    public InfiniFrameApplicationBuilder WithAppUserModelId(string id) {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        _appUserModelId = id;
        return this;
    }

    public InfiniFrameApplicationBuilder WithDefaultNotificationIcon(string path) {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        _defaultNotificationIcon = Path.GetFullPath(path);
        return this;
    }

    internal void AddIntegration(Action<InfiniFrameApplication> integration) {
        ArgumentNullException.ThrowIfNull(integration);
        _integrations.Add(integration);
    }

    internal Action<IInfiniFrameWindowBuilder>? TakeWindowConfiguration(string id) {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        int index = _windows.FindIndex(window => string.Equals(window.Id, id, StringComparison.Ordinal));
        if (index < 0) {
            int unnamedIndex = -1;
            for (int candidate = 0; candidate < _windows.Count; candidate++) {
                if (_windows[candidate].Id is not null) continue;
                if (unnamedIndex >= 0)
                    throw new InvalidOperationException(
                        $"Cannot determine which unnamed window should use integration '{id}'. " +
                        "Give the target window an id and pass the same id to the integration.");
                unnamedIndex = candidate;
            }
            index = unnamedIndex;
        }

        if (index < 0) return null;
        Action<IInfiniFrameWindowBuilder> configure = _windows[index].Configure;
        _windows.RemoveAt(index);
        return configure;
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
            ));

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
