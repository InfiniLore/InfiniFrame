// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniLore.Photino.NET;

namespace Tests.InfiniLore.Photino.NET;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowStateUtility : IDisposable {
    public required IPhotinoWindow Window { get; set; }

    public static WindowStateUtility Create(Action<IPhotinoWindowBuilder>? builder = null) {
        var windowBuilder = PhotinoWindowBuilder.Create();
        
        windowBuilder.SetStartUrl("https://localhost/");
        
        builder?.Invoke(windowBuilder);

        var utility =  new WindowStateUtility() {
            Window = windowBuilder.Build()
        };
        
        _ = Task.Run(utility.Window.WaitForClose);

        return utility; 
    }
    
    public void Dispose() {
        Window.Close();
        GC.SuppressFinalize(this);
    }
}
