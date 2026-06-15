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
    private int _shutdownStarted;
    
    /// <summary>Gets or sets the logger for the application.</summary>
    public required ILogger<InfiniFrameWebApplication> Logger { get; init; }
    /// <summary>Gets or sets the underlying ASP.NET Core <see cref="WebApplication" />.</summary>
    public required WebApplication WebApp { get; init; }
    /// <summary>Gets or sets the lazy factory for the associated window.</summary>
    public required Lazy<IInfiniFrameWindow> LazyWindow { private get; init; }
    /// <summary>Gets the associated InfiniFrame window instance.</summary>
    public IInfiniFrameWindow Window => LazyWindow.Value;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <summary>
    ///     Creates a new <see cref="InfiniFrameWebApplicationBuilder" /> with default ASP.NET Core and InfiniFrame
    ///     window builder services.
    /// </summary>
    /// <param name="args">Command-line arguments passed to the ASP.NET Core host builder.</param>
    /// <returns>An <see cref="InfiniFrameWebApplicationBuilder" /> for further configuration.</returns>
    public static InfiniFrameWebApplicationBuilder CreateBuilder(params string[] args)
        => new InfiniFrameWebApplicationBuilder {
            WebApp = WebApplication.CreateBuilder(args),
            WindowBuilder = InfiniFrameWindowBuilder.Create()
        }.Initialize();

    /// <summary>
    ///     Runs the web application and window, blocking until the window is closed.
    /// </summary>
    public void Run() {
        RunAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    ///     Runs the web application and window asynchronously, waiting for the window to close before stopping the server.
    /// </summary>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that completes when the window has been closed and the web app has stopped.</returns>
    public async Task RunAsync(CancellationToken ct = default) {
        Task runTask = WebApp.RunAsync(ct);
        try {
            await Window.WaitForCloseAsync(ct);
        }
        finally {
            await StopWebAppAsync(CancellationToken.None);
            await ObserveHostRunCompletionAsync(runTask);
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

        var builder = WebApp.Services.GetRequiredService<IInfiniFrameWindowBuilder>();
        builder.RegisterWindowClosingHandler((_, _) => ClosingHandler());
        builder.RegisterWindowClosingRequestedHandler(_ => ClosingHandler());
        return this;

        WindowClosingResult ClosingHandler() {
            StopWebApp();
            // return false else the window will be not be closed (see old InfiniFrame code why)
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
        await StopWebAppAsync(ct);
        Window.Close();
    }

    private void StopWebApp() {
        StopWebAppAsync().GetAwaiter().GetResult();
    }

    private async Task StopWebAppAsync(CancellationToken ct = default) {
        if (Interlocked.Exchange(ref _shutdownStarted, 1) != 0) return;

        try {
            await WebApp.StopAsync(ct);
        }
        catch (Exception e) when (ExceptionsUtility.IsNonFatalException(e)) {
            Logger.LogError(e, "Error stopping web app");
        }
    }

    private static async Task ObserveHostRunCompletionAsync(Task runTask) {
        try {
            await runTask;
        }
        catch (OperationCanceledException) {
            // Host shutdown cancellation is expected.
        }
    }
}
