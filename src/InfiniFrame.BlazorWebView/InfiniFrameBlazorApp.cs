// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameBlazorApp(
    IServiceProvider provider,
    RootComponentList rootComponents,
    IInfiniFrameJsComponentConfiguration? rootComponentConfiguration = null,
    IDisposable? unhandledExceptionRegistration = null
) : IAsyncDisposable {
    public IServiceProvider ServiceProvider { get; }= provider;
    private RootComponentList RootComponents { get; }= rootComponents;
    private IInfiniFrameJsComponentConfiguration? RootComponentConfiguration { get; }= rootComponentConfiguration;
    private IDisposable? UnhandledExceptionRegistration { get; } = unhandledExceptionRegistration;

    private bool _disposed;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public async Task RunAsync(CancellationToken ct = default) {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var window = ServiceProvider.GetRequiredService<IInfiniFrameWindow>();

        if (RootComponentConfiguration is not null) {
            foreach ((Type, string) component in RootComponents) {
                RootComponentConfiguration.Add(component.Item1, component.Item2);
            }
        }

        try {
            await window.WaitForCloseAsync(ct);
        }
        finally {
            await DisposeAsync();
        }
    }
    
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
            DisposeAsync().AsTask().GetAwaiter().GetResult();
        }
    }

    public async ValueTask DisposeAsync() {
        if (_disposed) return;

        _disposed = true;

        try {
            UnhandledExceptionRegistration?.Dispose();

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
        catch (Exception e) when (IsNonFatalException(e)) {
            var logger = ServiceProvider.GetService<ILogger<InfiniFrameBlazorApp>>();
            logger?.LogError(e, "Error disposing of InfiniFrameBlazorApp");
        }

        GC.SuppressFinalize(this);
    }

    private static bool IsNonFatalException(Exception exception)
        => exception is not (OutOfMemoryException or AccessViolationException);
}
