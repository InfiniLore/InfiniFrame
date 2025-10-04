using InfiniLore.Photino.NET;
using Microsoft.Extensions.DependencyInjection;

namespace InfiniLore.Photino.Blazor;
using Microsoft.Extensions.Logging;

public class PhotinoBlazorApp(
    IServiceProvider provider,
    RootComponentList rootComponents,
    IPhotinoJsComponentConfiguration? rootComponentConfiguration = null
) : IAsyncDisposable {
    
    private bool _disposed;
    private readonly Lock _disposeLock = new Lock();
    
    public void Run() {
        lock (_disposeLock) ObjectDisposedException.ThrowIf(_disposed, this);
        
        var window = provider.GetRequiredService<IPhotinoWindow>();
        
        if (rootComponentConfiguration is not null) {
            foreach ((Type, string) component in rootComponents) {
                rootComponentConfiguration.Add(component.Item1, component.Item2);
            }
        }

        try {
            window.Invoke(window.WaitForClose);
        }
        finally {
            _ = Task.Run(DisposeAsync);
        }
    }
    public async ValueTask DisposeAsync() {
        lock (_disposeLock) {
            if (_disposed) return;
            _disposed = true;
        }

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
            
            var logger = provider.GetRequiredService<ILogger<PhotinoBlazorApp>>();
            logger.LogInformation("PhotinoBlazorApp disposed");
        }
        catch (Exception e) {
            var logger = provider.GetRequiredService<ILogger<PhotinoBlazorApp>>();
            logger.LogError(e, "Error disposing of PhotinoBlazorApp");
        }
        
        GC.SuppressFinalize(this);
    }
}
