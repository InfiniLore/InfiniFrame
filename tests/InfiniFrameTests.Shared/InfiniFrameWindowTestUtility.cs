// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using System.Collections.Concurrent;

namespace InfiniFrameTests.Shared;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class InfiniFrameWindowTestUtility {
    public required IInfiniFrameWindow Window { get; init; }
    public static ConcurrentBag<InfiniFrameWindowTestUtility> Utilities { get; } = new();
    private Task? _windowTask;
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    private InfiniFrameWindowTestUtility() {}

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
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
            Window = windowBuilder.Build(parent: Utilities.ElementAtOrDefault(0)?.Window)
        };

        utility._windowTask = Task.Run(utility.Window.WaitForClose, utility._cancellationTokenSource.Token);

        Utilities.Add(utility);
        return utility;
    }

    public void Cleanup() {
        try {
            Window.Close();
            _cancellationTokenSource.Cancel();
            _windowTask?.Wait(TimeSpan.FromSeconds(1));
        }
        catch (Exception) {
            // Ignore
        }
    }
}
