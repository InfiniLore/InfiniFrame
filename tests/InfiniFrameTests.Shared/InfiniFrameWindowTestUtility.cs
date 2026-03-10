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
    private int _disposed;

    private const string StartString = """
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset="UTF-8">
        </head>
        <body>
        </body>
        </html>
        """;

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
        cancellationToken.ThrowIfCancellationRequested();

        var windowBuilder = InfiniFrameWindowBuilder.Create();

        windowBuilder.SetStartString(StartString);

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
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;

        try {
            Window.Close();
        }
        catch (Exception) {
            // Ignore shutdown exceptions during test cleanup.
        }

        try {
            TimeSpan timeout = TimeSpan.FromSeconds(TimeoutUtility.DefaultTimeout);
            _messageLoopTask?.Wait(timeout);
        }
        catch (Exception) {
            // Ignore shutdown exceptions during test cleanup.
        }
    }
}
