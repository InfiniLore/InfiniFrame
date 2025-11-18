// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using InfiniFrame.WebServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace InfiniFrameTests.Shared;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameServerTestUtility : IDisposable {
    public required IInfiniFrameWindow Window { get; init; }
    public required WebApplication WebApplication { get; init; }
    private readonly Thread _windowThread;
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    private InfiniFrameServerTestUtility(Thread windowThread) {
        _windowThread = windowThread;
    }

    public static InfiniFrameServerTestUtility Create(
        Action<WebApplicationBuilder>? appBuilder = null,
        Action<IInfiniFrameWindowBuilder>? windowBuilder = null
    ) {
        var creationSignal = new ManualResetEventSlim();
        InfiniFrameServerTestUtility? utility = null;
        Exception? creationException = null;
        
        var windowThread = new Thread(() => {
            try {
                InfiniFrameWebApplicationBuilder builder = InfiniFrameWebApplication.CreateBuilder();
                builder.WebApp.WebHost.UseStaticWebAssets();
                
                appBuilder?.Invoke(builder.WebApp);
                
                windowBuilder?.Invoke(builder.Window);
        
                InfiniFrameWebApplication application = builder.Build();
                application.WebApp.UseDefaultFiles();
                application.WebApp.MapStaticAssets();
                
                utility = new InfiniFrameServerTestUtility(Thread.CurrentThread) {
                    Window = application.Window,
                    WebApplication = application.WebApp
                };

                // Signal that creation is complete
                creationSignal.Set();

                // Run the message loop on this thread
                application.Run();
            }
            catch (Exception ex) {
                creationException = ex;
                creationSignal.Set();
            }
        }) {
            IsBackground = false// Keep the thread alive
        };

        // Set apartment state for Windows compatibility
        if (OperatingSystem.IsWindows()) windowThread.SetApartmentState(ApartmentState.STA);
        windowThread.Start();

        // Wait for the window and server to be created
        creationSignal.Wait();

        if (creationException != null) throw new InvalidOperationException("Failed to create window and server", creationException);
        if (utility == null) throw new InvalidOperationException("Window utility was not created");

        // Give a bit more time for the window to fully initialize
        Thread.Sleep(2000);

        return utility;
    }

    public void Dispose() {
        try {
            if (!_cancellationTokenSource.IsCancellationRequested) {
                _cancellationTokenSource.Cancel();

                Window.Close();

                // Give the window thread time to close gracefully
                if (!_windowThread.Join(TimeSpan.FromSeconds(5))) {
                    // Force abort if it doesn't close gracefully
                    _windowThread.Interrupt();
                }

                WebApplication.StopAsync(_cancellationTokenSource.Token).Wait();
            }

            _cancellationTokenSource.Dispose();
        }
        catch (Exception) {
            // Ignore
        }
        finally {
            GC.SuppressFinalize(this);
        }
    }
}
