// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InfiniFrame.BlazorWebView;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameBlazorApp(
    IServiceProvider provider,
    IInfiniFrameRootComponentList rootComponents,
    IInfiniFrameJsComponentConfiguration? rootComponentConfiguration = null,
    IDisposable? unhandledExceptionRegistration = null
) : IInfiniFrameBlazorApp {

    private int _disposed;
    public IServiceProvider ServiceProvider { get; } = provider;
    private IInfiniFrameRootComponentList RootComponents { get; } = rootComponents;
    private IInfiniFrameJsComponentConfiguration? RootComponentConfiguration { get; } = rootComponentConfiguration;
    private IDisposable? UnhandledExceptionRegistration { get; } = unhandledExceptionRegistration;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="IInfiniFrameBlazorApp.RunAsync"/>
    public async Task RunAsync(CancellationToken ct = default) {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);

        var window = ServiceProvider.GetRequiredService<IInfiniFrameWindow>();

        RegisterRootComponents();

        try {
            await window.WaitForCloseAsync(ct).ConfigureAwait(false);
        }
        finally {
            await DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <inheritdoc cref="IInfiniFrameBlazorApp.Run"/>
    public void Run() {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);

        var window = ServiceProvider.GetRequiredService<IInfiniFrameWindow>();

        RegisterRootComponents();

        try {
            window.WaitForClose();
        }
        finally {
            DisposeAsync().AsTask().GetAwaiter().GetResult();
        }
    }

    private void RegisterRootComponents() {
        if (RootComponentConfiguration is null) return;
        foreach ((Type, string) component in RootComponents) {
            RootComponentConfiguration.Add(component.Item1, component.Item2);
        }
    }

    public async ValueTask DisposeAsync() {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;

        ILogger<InfiniFrameBlazorApp>? logger = null;

        try {
            logger = ServiceProvider.GetService<ILogger<InfiniFrameBlazorApp>>();

            UnhandledExceptionRegistration?.Dispose();

            switch (ServiceProvider) {
                case IAsyncDisposable asyncDisposable:
                    await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                    break;

                case IDisposable disposable:
                    disposable.Dispose();
                    break;
            }
        }
        catch (Exception e) when (ExceptionsUtility.IsNonFatalException(e)) {
            logger?.LogError(e, "Error disposing of InfiniFrameBlazorApp");
        }

        GC.SuppressFinalize(this);
    }
}
