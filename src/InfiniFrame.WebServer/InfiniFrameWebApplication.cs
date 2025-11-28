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
        // WebApp.Lifetime.ApplicationStopping.Register(Stop);

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
        Window.RegisterWindowClosingHandler((_,_) => ClosingHandler());
        Window.RegisterWindowClosingRequestedHandler((_,_) => ClosingHandler());
        return this;
        
        bool ClosingHandler() {
            try {
                _ = Task.Run(() => {
                    StopWebApp();
                    return Task.CompletedTask;
                });
            }
            catch (Exception e) {
                Window.Logger.LogError(e, "Error stopping web app");
            }
            return false;
        }
    }

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
