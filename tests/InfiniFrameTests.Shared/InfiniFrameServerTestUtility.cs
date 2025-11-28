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
    private Thread? _windowThread;
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public static InfiniFrameServerTestUtility Create(
        Action<WebApplicationBuilder>? appBuilder = null,
        Action<IInfiniFrameWindowBuilder>? windowBuilder = null
    ) {
        var creationSignal = new ManualResetEventSlim();
        var readySignal = new ManualResetEventSlim();
        InfiniFrameServerTestUtility? utility = null;
        Exception? creationException = null;

        var windowThread = new Thread(() => {
            try {
                InfiniFrameWebApplicationBuilder builder = InfiniFrameWebApplication.CreateBuilder();
                builder.WebApp.WebHost.UseStaticWebAssets();

                appBuilder?.Invoke(builder.WebApp);

                windowBuilder?.Invoke(builder.Window);

                InfiniFrameWebApplication application = builder.Build();

                application.WebApp.Lifetime.ApplicationStarted.Register(() => readySignal.Set());

                #if NET8_0
                application.WebApp.UseStaticFiles();
                #else
                application.WebApp.UseStaticFiles();
                application.WebApp.MapStaticAssets();
                #endif

                utility = new InfiniFrameServerTestUtility {
                    Window = application.Window,
                    WebApplication = application.WebApp
                };

                creationSignal.Set();
                application.Run();
            }
            catch (Exception ex) {
                creationException = ex;
                creationSignal.Set();
                readySignal.Set();
            }
        }) {
            IsBackground = false
        };

        
        // Set apartment state for Windows compatibility
        if (OperatingSystem.IsWindows()) windowThread.SetApartmentState(ApartmentState.STA);
        windowThread.Start();

        // Wait for the window and server to be created
        creationSignal.Wait();

        if (creationException != null) throw new InvalidOperationException("Failed to create window and server", creationException);
        if (utility == null) throw new InvalidOperationException("Window utility was not created");
        utility!._windowThread = windowThread;
        
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (!readySignal.Wait(TimeSpan.FromSeconds(10))) {
            throw new TimeoutException("Web application failed to start within the timeout period");
        }

        return utility;
    }

    public void Dispose() {
        try {
            if (!_cancellationTokenSource.IsCancellationRequested) {
                _cancellationTokenSource.Cancel();

                WebApplication.StopAsync(_cancellationTokenSource.Token).Wait(TimeSpan.FromSeconds(5));
                Window.Close();

                if (_windowThread is not null && !_windowThread.Join(TimeSpan.FromSeconds(5))) {
                    _windowThread.Interrupt();
                }
            }

            _cancellationTokenSource.Dispose();
        }
        catch (Exception) {
            // Ignore cleanup exceptions
        }
        finally {
            GC.SuppressFinalize(this);
        }
    }
}
