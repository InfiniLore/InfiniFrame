// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.Photino.NET;

namespace Tests.Photino.NET.TestUtilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowTestUtility : IDisposable {
    public required IPhotinoWindow Window { get; init; }
    private Thread? WaitThread { get; set; }

    public static WindowTestUtility Create(Action<IPhotinoWindowBuilder>? builder = null) {
        var windowBuilder = PhotinoWindowBuilder.Create();
        
        windowBuilder.SetStartUrl("https://localhost/");
        
        builder?.Invoke(windowBuilder);

        var utility =  new WindowTestUtility {
            Window = windowBuilder.Build()
        };
        
        utility.WaitThread = new Thread(utility.Window.WaitForClose) {
            IsBackground = true,
            Name = "WindowTestUtility"
        };
        utility.WaitThread.Start();

        return utility; 
    }
    
    public void Dispose() {
        Window.Close();

        try {
            if (WaitThread is not { IsAlive: true }) return;
            if (WaitThread.Join(TimeSpan.FromMilliseconds(100))) return;
            WaitThread.Interrupt();
        }
        catch (Exception ex) {
            Console.WriteLine($"[WindowTestUtility] Dispose error: {ex}");
        }
        finally {
            GC.SuppressFinalize(this);    
        }
    }
}
