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

        Window.RegisterWindowClosingHandler((sender, _) => {
            if (sender is not IInfiniFrameWindow window) return false;

            try {
                StopWebApp();
                return true;
            }
            catch (Exception e) {
                window.Logger.LogError(e, "Error stopping web app");
                throw;
            }
        });

        Window.WaitForClose();
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
