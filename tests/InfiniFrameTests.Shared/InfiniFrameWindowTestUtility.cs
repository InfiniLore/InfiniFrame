// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniFrameTests.Shared;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowTestUtility : IDisposable {
    public required IInfiniFrameWindow Window { get; init; }

    private InfiniFrameWindowTestUtility() {}

    public static InfiniFrameWindowTestUtility Create(Action<IInfiniFrameWindowBuilder>? builder = null) {
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

        var utility = new InfiniFrameWindowTestUtility {
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
