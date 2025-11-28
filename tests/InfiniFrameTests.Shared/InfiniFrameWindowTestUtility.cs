// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniFrameTests.Shared;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowTestUtility {
    public static IInfiniFrameWindow ParentWindow { get; private set; } = null!;
    
    public required IInfiniFrameWindow Window { get; init; }
    public static List<InfiniFrameWindowTestUtility> Utilities { get; } = new();
    
    private InfiniFrameWindowTestUtility() {}

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public static void DefineParentWindow(IInfiniFrameWindow parentWindow) {
        ParentWindow = parentWindow;
    }

    public static InfiniFrameWindowTestUtility Create(Action<IInfiniFrameWindowBuilder>? builder = null) {
        var windowBuilder = InfiniFrameWindowBuilder.Create();

        // windowBuilder.SetStartUrl("https://localhost/");
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

        var utility = new InfiniFrameWindowTestUtility {
            Window = windowBuilder.Build()
        };

        _ = Task.Run(utility.Window.WaitForClose);

        Utilities.Add(utility);
        return utility;
    }

    public void Cleanup() {
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
