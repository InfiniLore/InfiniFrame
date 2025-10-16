// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.InfiniFrame;

namespace Tests.Shared;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowTestUtility : IDisposable {
    public required IInfiniFrameWindow Window { get; init; }
    
    private InfiniFrameWindowTestUtility() { }

    public static InfiniFrameWindowTestUtility Create(Action<IInfiniFrameWindowBuilder>? builder = null) {
        var windowBuilder = InfiniFrameWindowBuilder.Create();
        
        windowBuilder.SetStartUrl("https://localhost/");
        
        builder?.Invoke(windowBuilder);

        var utility =  new InfiniFrameWindowTestUtility {
            Window = windowBuilder.Build()
        };

        _ = Task.Run(utility.Window.WaitForClose);

        return utility; 
    }
    
    public void Dispose() {
        try {
            Window.Close();
        }
        catch (Exception) {
            // Ignore
        }
        finally {
            GC.SuppressFinalize(this);    
        }
    }
}
