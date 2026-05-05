// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.WebServer;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWebApplication {
    private int _shutdownStarted;
    
    public required WebApplication WebApp { get; init; }
    public required Lazy<IInfiniFrameWindow> LazyWindow { private get; init; }
    public IInfiniFrameWindow Window => LazyWindow.Value;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public static InfiniFrameWebApplicationBuilder CreateBuilder(params string[] args)
        => new InfiniFrameWebApplicationBuilder {
            WebApp = WebApplication.CreateBuilder(args),
            WindowBuilder = InfiniFrameWindowBuilder.Create()
        }.Initialize();

    public void Run() {
        RunAsync().GetAwaiter().GetResult();
    }

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
        catch (Exception e) when (IsNonFatalException(e)) {
            Window.Logger.LogError(e, "Error stopping web app");
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

    private static bool IsNonFatalException(Exception exception)
        => exception is not (OutOfMemoryException or AccessViolationException);
}
