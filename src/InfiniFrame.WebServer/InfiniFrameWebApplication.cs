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

    public void Stop() {
        _ = WebApp.StopAsync();
        _webAppThread?.Join();
        
        Window.Close();
    }
}
