// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace InfiniFrame.WebServer;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWebApplication {
    public required WebApplication WebApp { get; init; }
    public required Lazy<IInfiniFrameWindow> LazyWindow { private get; init; }
    public IInfiniFrameWindow Window => LazyWindow.Value;

    private Thread? _webAppThread;

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public static InfiniFrameWebApplicationBuilder CreateBuilder(params string[] args)
        => new InfiniFrameWebApplicationBuilder {
            WebApp = WebApplication.CreateBuilder(args),
            Window = InfiniFrameWindowBuilder.Create()
        }.Initialize();

    public void Run() {
        _webAppThread = new Thread(WebApp.Run);
        _webAppThread.Start();

        Window.WaitForClose();
    }

    /// <summary>
    /// Configures the application to automatically handle server shutdown when the associated
    /// window is closing or a close request is initiated. This method registers event handlers
    /// that trigger graceful stopping of the web application upon user-initiated window close actions.
    /// By invoking this method, the application ensures the proper release of resources and
    /// termination of processes tied to the web server during window closure.
    /// </summary>
    /// <returns>
    /// Returns the current instance of <see cref="InfiniFrameWebApplication"/> to enable method chaining.
    /// </returns>
    public InfiniFrameWebApplication UseAutoServerClose() {
        if (LazyWindow.IsValueCreated) {
            Window.RegisterWindowClosingHandler((_,_) => ClosingHandler());
            Window.RegisterWindowClosingRequestedHandler(_ => ClosingHandler());
            return this;    
        }

        var builder = WebApp.Services.GetRequiredService<IInfiniFrameWindowBuilder>();
        builder.RegisterWindowClosingHandler((_,_) => ClosingHandler());
        builder.RegisterWindowClosingRequestedHandler(_ => ClosingHandler());
        return this;
    
        bool ClosingHandler() {
            // Start web app shutdown in background - don't block UI thread
            Task.Run(async () => {
                try {
                    await WebApp.StopAsync();
                    
                    if (_webAppThread is null) return;
                    if (_webAppThread.Join(TimeSpan.FromSeconds(5))) return;
                    
                    _webAppThread.Interrupt();
                }
                catch (Exception e) {
                    Window.Logger.LogError(e, "Error stopping web app");
                }
            });
            // return false else the window will be not be closed (see old Photino code why)
            return false;
        }
    }

    /// <summary>
    /// Stops the web application and closes the associated application window.
    /// This method ensures that both the server instance and the user interface
    /// are gracefully terminated.
    /// </summary>
    public void Stop() {
        StopWebApp();
        Window.Close();
    }

    private void StopWebApp() {
        _ = WebApp.StopAsync();

        if (_webAppThread is not null && !_webAppThread.Join(TimeSpan.FromSeconds(5))) {
            _webAppThread.Interrupt();
        }
    }
}
