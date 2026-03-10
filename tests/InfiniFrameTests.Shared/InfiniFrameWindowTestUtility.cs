// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;
using JetBrains.Annotations;

namespace InfiniFrameTests.Shared;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[MustDisposeResource]
public sealed class InfiniFrameWindowTestUtility : IDisposable {
    public required IInfiniFrameWindow Window { get; init; }
    private Task? _messageLoopTask;

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    private InfiniFrameWindowTestUtility() {}

    [MustDisposeResource]
    public static InfiniFrameWindowTestUtility Create(CancellationToken cancellationToken = default)
        => Create(null, cancellationToken);
    
    [MustDisposeResource]
    public static InfiniFrameWindowTestUtility Create(
        Action<IInfiniFrameWindowBuilder>? builder,
        CancellationToken cancellationToken = default
    ) {
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

        utility._messageLoopTask = Task.Run(utility.Window.WaitForClose, cancellationToken);

        return utility;
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public void Dispose() {
        try {
            Window.Close();
            _messageLoopTask?.Wait(TimeSpan.FromSeconds(5));
        }
        catch (Exception) {
            // Ignore
        }
    }
}
