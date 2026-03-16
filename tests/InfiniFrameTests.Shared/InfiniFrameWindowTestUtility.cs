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

    private Thread? _windowThread;
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
        Action<IInfiniFrameWindowBuilder>? builder = null,
        CancellationToken cancellationToken = default
    ) {
        cancellationToken.ThrowIfCancellationRequested();

        var windowBuilder = InfiniFrameWindowBuilder.Create();
        windowBuilder.SetStartString(StartString);
        builder?.Invoke(windowBuilder);

        var windowSource = new TaskCompletionSource<IInfiniFrameWindow>(TaskCreationOptions.RunContinuationsAsynchronously);

        var thread = new Thread(() => {
            try {
                IInfiniFrameWindow window = windowBuilder.Build();
                windowSource.SetResult(window);
                window.WaitForClose();
            }
            catch (Exception ex) {
                windowSource.TrySetException(ex);
            }
        }) {
            IsBackground = true
        };

        if (OperatingSystem.IsWindows())
            thread.SetApartmentState(ApartmentState.STA);

        thread.Start();

        var utility = new InfiniFrameWindowTestUtility {
            Window = windowSource.Task.GetAwaiter().GetResult(),
            _windowThread = thread
        };

        return utility;
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public void Dispose() {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
            return;

        try {
            Window.Close();
        }
        catch {
            // ignored
        }

        try {
            if (_windowThread == null)
                return;

            if (!_windowThread.Join(TimeSpan.FromSeconds(5)))
                _windowThread.Interrupt();
        }
        catch {
            // ignored
        }
    }
}
