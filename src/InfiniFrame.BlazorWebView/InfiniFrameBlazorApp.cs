// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Blazor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameBlazorApp(
    IServiceProvider provider,
    RootComponentList rootComponents,
    IInfiniFrameJsComponentConfiguration? rootComponentConfiguration = null
) : IAsyncDisposable {
    public IServiceProvider ServiceProvider { get; }= provider;
    private RootComponentList RootComponents { get; }= rootComponents;
    private IInfiniFrameJsComponentConfiguration? RootComponentConfiguration { get; }= rootComponentConfiguration;

    private bool _disposed;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public void Run() {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var window = ServiceProvider.GetRequiredService<IInfiniFrameWindow>();

        if (RootComponentConfiguration is not null) {
            foreach ((Type, string) component in RootComponents) {
                RootComponentConfiguration.Add(component.Item1, component.Item2);
            }
        }

        try {
            window.WaitForClose();
        }
        finally {
            // TODO think about proper exception handling here
            window.Invoke(() => _ = Task.Run(DisposeAsync));
        }
    }

    public async ValueTask DisposeAsync() {
        if (_disposed) return;

        _disposed = true;

        try {
            switch (ServiceProvider) {
                case ServiceProvider serviceProvider: {
                    await serviceProvider.DisposeAsync();
                    break;
                }

                case IAsyncDisposable asyncDisposable: {
                    await asyncDisposable.DisposeAsync();
                    break;
                }

                case IDisposable disposable: {
                    disposable.Dispose();
                    break;
                }
            }
        }
        catch (Exception e) {
            var logger = ServiceProvider.GetService<ILogger<InfiniFrameBlazorApp>>();
            logger?.LogError(e, "Error disposing of InfiniFrameBlazorApp");
        }

        GC.SuppressFinalize(this);
    }
}
