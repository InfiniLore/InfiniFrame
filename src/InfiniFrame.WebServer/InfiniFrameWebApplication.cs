// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame.Utilities;

namespace InfiniFrame.WebServer;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Represents an InfiniFrame application that integrates an ASP.NET Core <see cref="WebApplication" /> with an
///     <see cref="IInfiniFrameWindow" />, providing lifecycle management for both the web server and the native window.
/// </summary>
public class InfiniFrameWebApplication {
    #if NET9_0_OR_GREATER
    private readonly Lock _shutdownLock = new();
    #else
    private readonly object _shutdownLock = new();
    #endif
    private Task? _shutdownTask;

    /// <summary>Gets or sets the logger for the application.</summary>
    public required ILogger<InfiniFrameWebApplication> Logger { get; init; }
    /// <summary>Gets or sets the underlying ASP.NET Core <see cref="WebApplication" />.</summary>
    public required WebApplication WebApp { get; init; }
    /// <summary>Gets or sets the lazy factory for the associated window.</summary>
    public required Lazy<IInfiniFrameWindow> LazyWindow { private get; init; }
    /// <summary>Gets the associated InfiniFrame window instance.</summary>
    public IInfiniFrameWindow Window => LazyWindow.Value;
    /// <summary>Gets or sets the InfiniFrame application instance.</summary>
    public IInfiniFrameApplication? Application { get; init; }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Runs the web application and window, blocking until the window is closed.
    /// </summary>
    /// <remarks>
    ///     This method uses synchronous-over-async patterns for ASP.NET Core host lifecycle
    ///     operations. It should only be called from threads without a SynchronizationContext
    ///     (e.g., console applications or the default thread pool). Prefer <see cref="RunAsync" />
    ///     for async contexts.
    /// </remarks>
    public void Run() {
        if (SynchronizationContext.Current is not null) {
            throw new InvalidOperationException(
                "Run() must be called from a thread without a SynchronizationContext to avoid deadlock during lifecycle operations. " +
                "Use RunAsync() instead.");
        }

        try {
            // Wait until the host is accepting requests before creating the window. On Windows,
            // the application message loop is required by WebView2 initialization and navigation;
            // WaitForCloseAsync only observes the closed signal.
            WebApp.StartAsync().GetAwaiter().GetResult();
            Window.WaitForClose();
        }
        finally {
            try {
                StopWebAppAsync(CancellationToken.None).GetAwaiter().GetResult();
            }
            finally {
                WebApp.DisposeAsync().AsTask().GetAwaiter().GetResult();
            }
        }
    }

    /// <summary>
    ///     Runs the web application and window asynchronously, waiting for the window to close before stopping the server.
    /// </summary>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that completes when the window has been closed and the web app has stopped.</returns>
    public async Task RunAsync(CancellationToken ct = default) {
        try {
            await WebApp.StartAsync(ct).ConfigureAwait(false);
            await Window.WaitForCloseAsync(ct).ConfigureAwait(false);
        }
        finally {
            try {
                await StopWebAppAsync(CancellationToken.None).ConfigureAwait(false);
            }
            finally {
                await WebApp.DisposeAsync().ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    ///     Configures the application to automatically handle server shutdown when the associated
    ///     window is closing or a close request is initiated. This method registers event handlers
    ///     that trigger graceful stopping of the web application upon user-initiated window close actions.
    ///     By invoking this method, the application ensures the proper release of resources and
    ///     termination of processes tied to the web server during window closure.
    /// </summary>
    /// <returns>
    ///     Returns the current instance of <see cref="InfiniFrameWebApplication" /> to enable method chaining.
    /// </returns>
    public InfiniFrameWebApplication UseAutoServerClose() {
        if (LazyWindow.IsValueCreated) {
            Window.RegisterWindowClosingHandler((_, _) => ClosingHandler());
            Window.RegisterWindowClosingRequestedHandler(_ => ClosingHandler());
            return this;
        }

        var builder = WebApp.Services.GetService<IInfiniFrameWindowBuilder>();
        if (builder is not null) {
            builder.RegisterWindowClosingHandler((_, _) => ClosingHandler());
            builder.RegisterWindowClosingRequestedHandler(_ => ClosingHandler());
        }
        return this;

        WindowClosingResult ClosingHandler() {
            // This runs inside a native UI callback. The shared shutdown task is observed by
            // RunAsync/StopAsync, so starting it here does not block the UI thread or lose it.
            _ = StopWebAppAsync();
            // return false else the window will not be closed (see old InfiniFrame code why)
            return WindowClosingResult.Close;
        }
    }

    /// <summary>
    ///     Stops the web application and closes the associated application window.
    ///     This method ensures that both the server instance and the user interface
    ///     are gracefully terminated.
    /// </summary>
    public void Stop() {
        StopAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    ///     Stops the web application and closes the associated window asynchronously.
    /// </summary>
    /// <param name="ct">A cancellation token that can be used to cancel the stop operation.</param>
    /// <returns>A task that completes when both the web app and window have been stopped.</returns>
    public async Task StopAsync(CancellationToken ct = default) {
        await StopWebAppAsync(ct).ConfigureAwait(false);
        await Window.CloseAsync(ct).ConfigureAwait(false);
        await Window.WaitForCloseAsync(ct).ConfigureAwait(false);
    }

    private Task StopWebAppAsync(CancellationToken ct = default) {
        Task shutdownTask;
        lock (_shutdownLock) {
            shutdownTask = _shutdownTask ??= StopWebAppCoreAsync();
        }

        return shutdownTask.WaitAsync(ct);
    }

    private async Task StopWebAppCoreAsync() {
        try {
            // Cancellation only cancels an individual caller's wait. Once shutdown starts it
            // must run to completion for all callers.
            await WebApp.StopAsync(CancellationToken.None).ConfigureAwait(false);
        }
        catch (Exception e) when (ExceptionsUtility.IsNonFatalException(e)) {
            Logger.LogError(e, "Error stopping web app");
        }
    }
}
