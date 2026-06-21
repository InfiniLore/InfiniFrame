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

    private bool _disposed;
    public IServiceProvider ServiceProvider { get; } = provider;
    private IInfiniFrameRootComponentList RootComponents { get; } = rootComponents;
    private IInfiniFrameJsComponentConfiguration? RootComponentConfiguration { get; } = rootComponentConfiguration;
    private IDisposable? UnhandledExceptionRegistration { get; } = unhandledExceptionRegistration;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="IInfiniFrameBlazorApp.RunAsync"/>
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

    /// <inheritdoc cref="IInfiniFrameBlazorApp.Run"/>
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

        ILogger<InfiniFrameBlazorApp>? logger = null;

        try {
            logger = ServiceProvider.GetService<ILogger<InfiniFrameBlazorApp>>();

            UnhandledExceptionRegistration?.Dispose();

            switch (ServiceProvider) {
                case IAsyncDisposable asyncDisposable:
                    await asyncDisposable.DisposeAsync();
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
