
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace Tests.Shared.Photino;
using InfiniLore.Photino.NET;
using InfiniLore.Photino.NET.Server;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowServerTestUtility : IDisposable {
    public required IPhotinoWindow Window { get; init; }
    public required PhotinoServer Server { get; init; }
    private readonly Thread _windowThread;
    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    
    private WindowServerTestUtility(Thread windowThread) { 
        _windowThread = windowThread;
    }
    
    public static WindowServerTestUtility Create(
        Action<PhotinoServerBuilder>? serverBuilder = null,
        Action<IPhotinoWindowBuilder>? windowBuilder = null
    ) {
        var creationSignal = new ManualResetEventSlim();
        WindowServerTestUtility? utility = null;
        Exception? creationException = null;
        
        var windowThread = new Thread(() => {
            try {
                var photinoServerBuilder = PhotinoServerBuilder.Create();
                serverBuilder?.Invoke(photinoServerBuilder);

                PhotinoServer photinoServer = photinoServerBuilder.Build();
                
                photinoServer.MapPhotinoJsEndpoints();
                photinoServer.Run();

                IPhotinoWindowBuilder wb = photinoServer.GetAttachedWindowBuilder();
                windowBuilder?.Invoke(wb);
                
                IPhotinoWindow window = wb.Build();
                
                utility = new WindowServerTestUtility(Thread.CurrentThread) {
                    Window = window,
                    Server = photinoServer
                };
                
                // Signal that creation is complete
                creationSignal.Set();
                
                // Run the message loop on this thread
                window.WaitForClose();
            }
            catch (Exception ex) {
                creationException = ex;
                creationSignal.Set();
            }
        }) {
            IsBackground = false // Keep the thread alive
        };
        
        // Set apartment state for Windows compatibility
        if(OperatingSystem.IsWindows()) windowThread.SetApartmentState(ApartmentState.STA);
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
            
                Server.Stop();
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