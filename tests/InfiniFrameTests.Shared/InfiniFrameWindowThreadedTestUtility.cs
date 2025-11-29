// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniFrameTests.Shared;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowThreadedTestUtility : IDisposable {
    public required IInfiniFrameWindow Window { get; init; }
    
    private readonly Thread _windowThread;

    private InfiniFrameWindowThreadedTestUtility(Thread windowThread) {
        _windowThread = windowThread;
    }

    public static InfiniFrameWindowThreadedTestUtility Create(Action<IInfiniFrameWindowBuilder>? builder = null) {
        var creationSignal = new ManualResetEventSlim();
        InfiniFrameWindowThreadedTestUtility? utility = null;
        Exception? creationException = null;
        
        var windowThread = new Thread(() => {
            try {
                var windowBuilder = InfiniFrameWindowBuilder.Create();
                windowBuilder.SetStartString("""
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <meta charset="UTF-8">
                    </head>
                    <body>
                    </body>
                    </html>
                    """);

                builder?.Invoke(windowBuilder);
                
                utility = new InfiniFrameWindowThreadedTestUtility(Thread.CurrentThread) {
                    Window = windowBuilder.Build()
                };

                creationSignal.Set();
                utility.Window.WaitForClose();
            }
            catch (Exception ex) {
                creationException = ex;
                creationSignal.Set();
            }
        }) {
            IsBackground = false
        };

        // Set apartment state for Windows compatibility
        if (OperatingSystem.IsWindows()) 
            windowThread.SetApartmentState(ApartmentState.STA);
        
        windowThread.Start();

        // Wait for window creation
        creationSignal.Wait();
        
        // Give a bit more time for the window to fully initialize
        Thread.Sleep(2000);

        if (creationException != null) 
            throw new InvalidOperationException("Failed to create window", creationException);
        
        return utility ?? throw new InvalidOperationException("Window utility was not created");
    }

    public void Dispose() {
        try {
            Window.Close();
            
            if (_windowThread.Join(TimeSpan.FromSeconds(5))) return;
            _windowThread.Interrupt();
            _windowThread.Join(TimeSpan.FromSeconds(1));
        }
        catch (Exception) {
            // Ignore disposal exceptions
        }
        finally {
            GC.SuppressFinalize(this);
        }
    }
}