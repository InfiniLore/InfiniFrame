// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace Tests.InfiniLore.Photino.NET.TestUtilities;
using global::InfiniLore.Photino.NET;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class WindowTestUtility : IDisposable {
    public required IPhotinoWindow Window { get; set; }
    
    public static List<IPhotinoWindow> Windows { get; } = new List<IPhotinoWindow>();

    public static WindowTestUtility Create(Action<IPhotinoWindowBuilder>? builder = null) {
        var windowBuilder = PhotinoWindowBuilder.Create();
        
        windowBuilder.SetStartUrl("https://localhost/");
        
        builder?.Invoke(windowBuilder);

        var utility =  new WindowTestUtility {
            Window = windowBuilder.Build()
        };
        
        _ = Task.Run(utility.Window.WaitForClose);
        
        Windows.Add(utility.Window);

        return utility; 
    }
    
    public void Dispose() {
        Window.Close();
        GC.SuppressFinalize(this);
    }
}
