using InfiniLore.InfiniFrame.Blazor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InfiniLore.InfiniFrame.Blazor;
public class InfiniFrameBlazorApp(
    IServiceProvider provider,
    RootComponentList rootComponents,
    IInfiniFrameJsComponentConfiguration? rootComponentConfiguration = null
) : IAsyncDisposable {

    private bool _disposed;

    public void Run() {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var window = provider.GetRequiredService<IInfiniFrameWindow>();

        if (rootComponentConfiguration is not null) {
            foreach ((Type, string) component in rootComponents) {
                rootComponentConfiguration.Add(component.Item1, component.Item2);
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
            switch (provider) {
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
            var logger = provider.GetService<ILogger<InfiniFrameBlazorApp>>();
            logger?.LogError(e, "Error disposing of InfiniFrameBlazorApp");
        }

        GC.SuppressFinalize(this);
    }
}
