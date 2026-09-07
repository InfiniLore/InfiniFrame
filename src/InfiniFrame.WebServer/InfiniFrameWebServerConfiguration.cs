// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.WebServer;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public sealed class InfiniFrameWebServerConfiguration {
    private readonly WebApplicationBuilder _webApplicationBuilder;

    internal InfiniFrameWebServerConfiguration(WebApplicationBuilder webApp, IServiceCollection services) {
        _webApplicationBuilder = webApp;
        RootServices = services;
    }

    public IWebHostBuilder WebHost => _webApplicationBuilder.WebHost;
    internal WebApplicationBuilder Builder => _webApplicationBuilder;
    internal IServiceCollection RootServices { get; }
    internal string? ConfiguredStartUrl => _webApplicationBuilder.Configuration["ASPNETCORE_URLS"]
        ?? _webApplicationBuilder.Configuration["urls"]?
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .FirstOrDefault();

    private readonly List<Action<WebApplication>> _applicationConfiguration = [];

    public InfiniFrameWebServerConfiguration ConfigureWebApplication(Action<WebApplication> configure) {
        ArgumentNullException.ThrowIfNull(configure);
        _applicationConfiguration.Add(configure);
        return this;
    }

    internal void Apply(WebApplication application) {
        foreach (Action<WebApplication> configure in _applicationConfiguration)
            configure(application);
    }

    internal void AddRootServices() {
        foreach (ServiceDescriptor descriptor in RootServices)
            _webApplicationBuilder.Services.Add(descriptor);
    }

    internal WebApplication Build() => _webApplicationBuilder.Build();

    internal WebApplication BuildApplication() {
        AddRootServices();
        _webApplicationBuilder.WebHost.UseStaticWebAssets();
        WebApplication application = _webApplicationBuilder.Build();
        application.UseDefaultFiles();
        Apply(application);
        return application;
    }
}
